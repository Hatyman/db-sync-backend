using MccSoft.DbSyncApp.Domain;
using MccSoft.WebApi.Pagination;

namespace MccSoft.DbSyncApp.App.Features.Products.Dto
{
    public class SearchProductDto : PagedRequestDto
    {
        public string? Search { get; set; }
        public ProductType? ProductType { get; set; }
    }
}
