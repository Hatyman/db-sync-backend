using System;
using System.Text.Json.Serialization;
using MccSoft.DbSyncApp.Domain;

namespace MccSoft.DbSyncApp.App.Features.Products.Dto
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ProductType ProductType { get; set; }

        public DateOnly LastStockUpdatedAt { get; set; }

        public ProductListItemDto() { }
    }
}
