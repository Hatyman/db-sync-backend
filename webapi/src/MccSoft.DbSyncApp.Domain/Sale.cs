using System;
using System.Collections.Generic;

namespace MccSoft.DbSyncApp.Domain;

public class Sale : ISyncableEntity
{
    public string Id { get; set; }

    public List<Product> Products { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime? OptionalDateTime { get; set; }

    public List<Box> Boxes { get; set; }

    /// <summary>
    /// Needed for Entity Framework, keep empty.
    /// </summary>
    public Sale() { }
}
