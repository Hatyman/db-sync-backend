using System.Threading.Tasks;
using MccSoft.DbSyncApp.App.Features.Products.Dto;
using MccSoft.WebApi.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MccSoft.DbSyncApp.App.Features.Products
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/products")]
    public class ProductController
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task<ProductDto> Create(CreateProductDto dto)
        {
            return await _productService.Create(dto);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task<ProductDto> Patch(string id, [FromBody] PatchProductDto dto)
        {
            return await _productService.Patch(id, dto);
        }

        [HttpDelete("")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        public async Task Delete(string id)
        {
            await _productService.Delete(id);
        }

        [HttpGet]
        public async Task<PagedResult<ProductListItemDto>> Search([FromQuery] SearchProductDto dto)
        {
            return await _productService.Search(dto);
        }

        [HttpGet("{id}")]
        public async Task<ProductDto> Get(string id)
        {
            return await _productService.Get(id);
        }
    }
}
