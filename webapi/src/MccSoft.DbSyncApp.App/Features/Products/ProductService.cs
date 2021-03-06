using System.Linq;
using System.Threading.Tasks;
using MccSoft.DbSyncApp.App.Features.Products.Dto;
using MccSoft.DbSyncApp.App.Utils;
using MccSoft.DbSyncApp.Domain;
using MccSoft.DbSyncApp.Persistence;
using MccSoft.PersistenceHelpers;
using MccSoft.WebApi.Pagination;
using MccSoft.WebApi.Patching;
using Microsoft.EntityFrameworkCore;
using NeinLinq;

namespace MccSoft.DbSyncApp.App.Features.Products
{
    public class ProductService
    {
        private readonly DbSyncAppDbContext _dbContext;

        public ProductService(DbSyncAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductDto> Create(CreateProductDto dto)
        {
            var productId = await _dbContext.Database
                .CreateExecutionStrategy()
                .ExecuteAsync(
                    async () =>
                    {
                        await using var transaction = _dbContext.BeginTransaction();
                        var product = new Product(dto.Title)
                        {
                            ProductType = dto.ProductType,
                            LastStockUpdatedAt = dto.LastStockUpdatedAt
                        };
                        _dbContext.Products.Add(product);

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        return product.Id;
                    }
                );
            return await Get(productId);
        }

        public async Task<ProductDto> Patch(string id, PatchProductDto dto)
        {
            Product product = await _dbContext.Products.GetOne(ISyncableEntity.HasId<Product>(id));
            product.Update(dto);

            await _dbContext.SaveChangesAsync();

            return await Get(product.Id);
        }

        public async Task<PagedResult<ProductListItemDto>> Search(SearchProductDto search)
        {
            IQueryable<Product> query = _dbContext.Products;

            if (!string.IsNullOrEmpty(search.Search))
            {
                query = query.Where(x => x.Title.Contains(search.Search));
            }

            if (search.ProductType != null)
            {
                query = query.Where(x => x.ProductType == search.ProductType);
            }

            return await query
                .Select(x => x.ToProductListItemDto())
                .ToPagingListAsync(search, nameof(ProductListItemDto.Id));
        }

        public async Task<ProductDto> Get(string id)
        {
            return await _dbContext.Products.GetOne(
                ISyncableEntity.HasId<Product>(id),
                x => x.ToProductDto()
            );
        }

        public async Task Delete(string id)
        {
            var product = await _dbContext.Products.GetOne(ISyncableEntity.HasId<Product>(id));

            _dbContext.Products.Remove(product);

            await _dbContext.SaveChangesAsync();
        }
    }
}
