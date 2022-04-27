using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MccSoft.DbSyncApp.Domain;

public class Transaction : ISyncableEntity
{
    public Transaction(
        string id,
        string tableName,
        Dictionary<string, dynamic> changes,
        ChangeType changeType,
        string instanceId,
        DateTime creationDate,
        DateTime syncDate
    )
    {
        Id = id;
        TableName = tableName;
        Changes = changes;
        ChangeType = changeType;
        InstanceId = instanceId;
        CreationDate = creationDate;
        SyncDate = syncDate;
    }
    public Transaction() { }

    public string Id { get; set; }
    [Required]
    public string TableName { get; set; }
    [Column(TypeName = "json")]
    public object Changes { get; set; }
    public ChangeType ChangeType { get; set; }
    public string InstanceId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? SyncDate { get; set; }
}
