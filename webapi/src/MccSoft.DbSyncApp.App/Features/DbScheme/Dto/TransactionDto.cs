using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MccSoft.DbSyncApp.Domain;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class TransactionDto
{
    public string Id { get; set; }
    [Required]
    public string TableName { get; set; }
    public Dictionary<string, dynamic> Changes { get; set; }
    public ChangeType ChangeType { get; set; }
    public string InstanceId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? SyncDate { get; set; }
}
