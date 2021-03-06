using System.Collections.Generic;

namespace MccSoft.DbSyncApp.Domain;

public class Box : ISyncableEntity
{
    public Box() { }
    public string Id { get; set; }
    public bool IsFull { get; set; }
    public int? Count { get; set; }
    public string Color { get; set; }
    public Product Product { get; set; }
    public List<Sale> Sales { get; set; }
}
