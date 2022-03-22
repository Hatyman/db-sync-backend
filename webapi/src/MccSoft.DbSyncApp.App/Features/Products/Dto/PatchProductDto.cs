using System;
using System.ComponentModel.DataAnnotations;
using MccSoft.DbSyncApp.Domain;
using MccSoft.WebApi.Patching.Models;

namespace MccSoft.DbSyncApp.App.Features.Products.Dto
{
    public class PatchProductDto : PatchRequest<Product>
    {
        [MinLength(3)]
        public string Title { get; set; }
        public ProductType ProductType { get; set; }

        public DateOnly LastStockUpdatedAt { get; set; }
    }
}
