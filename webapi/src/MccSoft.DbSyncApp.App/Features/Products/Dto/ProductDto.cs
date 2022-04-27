using System;
using MccSoft.DbSyncApp.Domain;
using Newtonsoft.Json;

namespace MccSoft.DbSyncApp.App.Features.Products.Dto
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ProductType ProductType { get; set; }

        public DateOnly LastStockUpdatedAt { get; set; }
    }
}
