using System.Collections.Generic;

namespace MccSoft.DbSyncApp.Domain;

public class Box
{
    protected Box() { }
    public string Id { get; set; }
    public bool IsFull { get; set; }
    public Product Product { get; set; }
    public List<Sale> Sales { get; set; }
}
