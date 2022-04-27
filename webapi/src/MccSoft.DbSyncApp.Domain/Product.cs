using System;
using MccSoft.DomainHelpers;
using MccSoft.DomainHelpers.DomainEvents.Events;
using Microsoft.Extensions.Logging;

namespace MccSoft.DbSyncApp.Domain
{
    public class Product : BaseEntity, ISyncableEntity
    {
        private string _title;
        public string Id { get; set; }
        public ProductType ProductType { get; set; }

        public double PriceDouble { get; set; }
        public float PriceFloat { get; set; }

        public string SaleId { get; set; }
        public Sale Sale { get; set; }
        public string BoxId { get; set; }
        public Box Box { get; set; }
        public DateOnly LastStockUpdatedAt { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;

                AddEvent(LogDomainEvent.Info("Title changed to {title}", value));
                _title = value;
            }
        }

        /// <summary>
        /// Needed for Entity Framework, keep empty.
        /// </summary>
        public Product() { }

        /// <summary>
        /// Constructor to initialize User entity.
        /// </summary>
        public Product(string title)
        {
            Title = title;
        }
    }
}
