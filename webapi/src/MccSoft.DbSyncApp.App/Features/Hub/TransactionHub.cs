using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MccSoft.DbSyncApp.App.Features.DbScheme.Dto;
using MccSoft.DbSyncApp.Domain;
using MccSoft.DbSyncApp.Persistence;
using MccSoft.PersistenceHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MccSoft.DbSyncApp.App.Features.Hub;

public class TransactionHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly DbSyncAppDbContext _dbContext;

    private readonly ILogger<TransactionHub> _logger;

    public TransactionHub(DbSyncAppDbContext dbContext, ILogger<TransactionHub> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> Send(string message)
    {
        await Clients.All.SendAsync("test", message);

        return $"{message} Processed";
    }
    // private static Dictionary<string, >
    public async Task<List<string>> SyncTransactions(List<TransactionDto> transactions)
    {
        var syncedTransactionsId = new List<string>();
        var unSyncedTransactions = new List<Transaction>();
        var unSyncedTransactionDtos = new List<TransactionDto>();

        foreach (var transaction in transactions)
        {
            var existingTransaction = await _dbContext.Transactions.GetOneOrDefault(
                ISyncableEntity.HasId(transaction.Id)
            );
            if (existingTransaction == null)
            {
                unSyncedTransactionDtos.Add(transaction);
                unSyncedTransactions.Add(
                    new Transaction(
                        transaction.Id,
                        transaction.TableName,
                        transaction.Changes,
                        transaction.ChangeType,
                        transaction.InstanceId,
                        transaction.CreationDate.ToUniversalTime(),
                        DateTime.Now.ToUniversalTime()
                    )
                );
            }
            else
            {
                syncedTransactionsId.Add(transaction.Id);
            }
        }

        return await _dbContext.Database
            .CreateExecutionStrategy()
            .ExecuteAsync(
                async () =>
                {
                    var dbTransaction = await _dbContext.BeginTransactionAsync();

                    await dbTransaction.CreateSavepointAsync("Start");
                    // _dbContext.TryGetPropertyValue<DbSet<SyncableEntity>>(unSyncedTransactions[0].TableName).FirstOrDefault()
                    // AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetType($"{x.GetName().Name}.Box") != null)

                    for (int i = 0; i < unSyncedTransactions.Count; i++)
                    {
                        Transaction transaction = unSyncedTransactions[i];
                        TransactionDto transactionDto = unSyncedTransactionDtos[i];
                        Type classType =
                            Type.GetType(transactionDto.EntityFullName)
                            ?? Assembly
                                .Load(transactionDto.AssemblyName)
                                .GetType(transactionDto.EntityFullName)!;
                        var transactionChanges = transactionDto.Changes;

                        switch (transaction.ChangeType)
                        {
                            case ChangeType.Insert:
                                var item = Activator.CreateInstance(classType);
                                var properties = classType.GetProperties();
                                foreach (var property in properties)
                                {
                                    if (
                                        transactionChanges.TryGetValue(property.Name, out var value)
                                    )
                                    {
                                        /*if (value is JToken jToken)
                                        {
                                            jToken.Value<string>();
                                            //jToken.
                                        }*/
                                        var castedValue = Convert.ChangeType(
                                            value.ToString(),
                                            property.PropertyType
                                        );
                                        property.SetValue(item, castedValue);
                                    }
                                }

                                _dbContext.Add(item);
                                break;
                            case ChangeType.Update:
                                var itemToBeModified = _dbContext.Find(
                                    classType,
                                    transaction.InstanceId
                                );
                                foreach (KeyValuePair<string, dynamic> pair in transactionChanges)
                                {
                                    PropertyInfo property = classType.GetProperty(pair.Key);
                                    var castedValue = Convert.ChangeType(
                                        pair.Value.ToString(),
                                        property.PropertyType
                                    );
                                    property.SetValue(itemToBeModified, castedValue);
                                }
                                break;
                            case ChangeType.Delete:
                                var itemToBeDeleted = await _dbContext.FindAsync(
                                    classType,
                                    transaction.InstanceId
                                );
                                if (itemToBeDeleted != null)
                                {
                                    _dbContext.Remove(itemToBeDeleted);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _dbContext.Transactions.Add(transaction);
                        await dbTransaction.CreateSavepointAsync($"Transaction {i}");
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        await dbTransaction.CommitAsync();

                        syncedTransactionsId.AddRange(
                            unSyncedTransactions.Select(x => x.Id).ToList()
                        );
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Something went wrong on transactions save");
                    }

                    return syncedTransactionsId;
                }
            );
    }
}
