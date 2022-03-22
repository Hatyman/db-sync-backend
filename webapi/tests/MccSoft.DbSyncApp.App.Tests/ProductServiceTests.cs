using System.Threading.Tasks;
using FluentAssertions;
using MccSoft.DbSyncApp.App.Features.Products;
using MccSoft.DbSyncApp.App.Utils;
using MccSoft.DbSyncApp.TestUtils.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace MccSoft.DbSyncApp.App.Tests
{
    public class ProductServiceTests : AppServiceTestBase<ProductService>
    {
        private readonly DateTimeProvider _time = new();

        public ProductServiceTests()
        {
            var logger = new NullLogger<ProductService>();

            Sut = InitializeService((retryHelper, db) => new ProductService(db));
        }

        [Fact]
        public async Task Create()
        {
            var result = await Sut.Create(a.CreateProductDto("asd"));
            result.Title.Should().Be("asd");
        }
    }
}
