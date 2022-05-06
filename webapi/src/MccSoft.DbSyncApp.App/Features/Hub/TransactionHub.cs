using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper.Internal;
using MccSoft.DbSyncApp.App.Features.DbScheme.Dto;
using MccSoft.DbSyncApp.Domain;
using MccSoft.DbSyncApp.Persistence;
using MccSoft.PersistenceHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

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

    public override async Task OnConnectedAsync()
    {
        HttpContext? httpContext = Context.GetHttpContext();

        if (httpContext != null)
        {
            StringValues queryValues = httpContext.Request.Query["lastSyncTransactionId"];
            string? lastSyncTransactionId = queryValues.FirstOrDefault();

            var lastSyncTransaction = await _dbContext.Transactions.GetOneOrDefault(
                ISyncableEntity.HasId<Transaction>(lastSyncTransactionId)
            );

            IQueryable<Transaction> query = _dbContext.Transactions;

            if (!string.IsNullOrEmpty(lastSyncTransactionId) && lastSyncTransaction != null)
            {
                query = query.Where(
                    x => x.SyncDate >= lastSyncTransaction.SyncDate && x.Id != lastSyncTransactionId
                );
            }

            List<TransactionDto> transactionsToBeSynced = query
                .Select(
                    x =>
                        new TransactionDto()
                        {
                            Id = x.Id,
                            Changes = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(
                                x.Changes.ToString()
                            ),
                            ChangeType = x.ChangeType,
                            InstanceId = x.InstanceId,
                            CreationDate = x.CreationDate,
                            SyncDate = x.SyncDate,
                            TableName = x.TableName,
                            AssemblyName = "",
                            EntityFullName = "",
                        }
                )
                .ToList();

            await Clients.Caller.SendAsync("client-received", transactionsToBeSynced);
        }

        await base.OnConnectedAsync();
    }

    public async Task<string> Send(string message)
    {
        await Clients.All.SendAsync("test", message);

        return $"{message} Processed";
    }

    private static int _counter = 1;
    public async Task SendFakeTransactions()
    {
        var dateTime = DateTime.Now.ToUniversalTime();
        dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond).AddMilliseconds(212);

        var list = new List<TransactionDto>
        {
            // new TransactionDto()
            // {
            //     Id = $"fake-{_counter}",
            //     Changes = new Dictionary<string, dynamic>()
            //     {
            //         { "Id", $"fake-sale-{_counter}" },
            //         { "DateTime", DateTime.Now.ToUniversalTime().AddDays(_counter).ToJsonIso8601() }
            //     },
            //     AssemblyName =
            //         "MccSoft.DbSyncApp.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
            //     EntityFullName = "MccSoft.DbSyncApp.Domain.Sale",
            //     ChangeType = ChangeType.Insert,
            //     CreationDate = DateTime.Now,
            //     InstanceId = $"fake-sale-{_counter}",
            //     TableName = "Sales",
            //     SyncDate = DateTime.Now,
            // },
            // new TransactionDto()
            // {
            //     Id = $"fake-box-tr-{_counter}",
            //     Changes = new Dictionary<string, dynamic>() { { "IsFull", false } },
            //     AssemblyName =
            //         "MccSoft.DbSyncApp.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
            //     EntityFullName = "MccSoft.DbSyncApp.Domain.Box",
            //     ChangeType = ChangeType.Update,
            //     CreationDate = DateTime.Now,
            //     InstanceId = $"b7168640-098a-420f-9dcf-24eec036ce4f",
            //     TableName = "Boxes",
            //     SyncDate = DateTime.Now,
            // },
            // new TransactionDto()
            // {
            //     Id = $"fake-sale-tr-{_counter}",
            //     Changes = new Dictionary<string, dynamic>()
            //     {
            //         { "DateTime", $"2022-05-03T18:{_counter % 60}:00.000Z" },
            //         { "OptionalDateTime", $"2022-05-03T18:{_counter % 60}:01.111Z" },
            //     },
            //     AssemblyName =
            //         "MccSoft.DbSyncApp.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
            //     EntityFullName = "MccSoft.DbSyncApp.Domain.Sale",
            //     ChangeType = ChangeType.Update,
            //     CreationDate = DateTime.Now,
            //     InstanceId = $"149503c1-98c7-49da-b129-f157ca32c410",
            //     TableName = "Sales",
            //     SyncDate = DateTime.Now,
            // },
            new TransactionDto()
            {
                Id = $"fake-sale-tr-{_counter}",
                Changes = new Dictionary<string, dynamic>(),
                AssemblyName =
                    "MccSoft.DbSyncApp.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                EntityFullName = "MccSoft.DbSyncApp.Domain.Sale",
                ChangeType = ChangeType.Delete,
                CreationDate = DateTime.Now,
                InstanceId = $"4ddf4e37-5dca-407c-9637-357f5fc17d5a",
                TableName = "Sales",
                SyncDate = DateTime.Now,
            }
        };

        _counter += 1;
        await Clients.All.SendAsync("client-received", list);
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
                        DateTime.SpecifyKind(transaction.CreationDate, DateTimeKind.Utc),
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
                                        var castedValue = this.castValueToPropertyType(
                                            value,
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

                                    var castedValue = this.castValueToPropertyType(
                                        pair.Value,
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

    private dynamic? castValueToPropertyType(dynamic? value, Type propertyType)
    {
        if (value?.ToString() == null)
        {
            return null;
        }

        var castedValue = Convert.ChangeType(
            value?.ToString(),
            propertyType.IsNullableType() ? Nullable.GetUnderlyingType(propertyType)! : propertyType
        );

        if (
            (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            && castedValue != null
        )
        {
            DateTime dateTime = castedValue;
            castedValue = dateTime.ToUniversalTime();
        }

        return castedValue;
    }
}
