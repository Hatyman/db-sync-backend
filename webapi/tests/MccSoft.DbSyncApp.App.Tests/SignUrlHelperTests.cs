using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using MccSoft.DbSyncApp.App.Features.Products;
using MccSoft.DbSyncApp.App.Services.Authentication;
using MccSoft.DbSyncApp.App.Utils;
using MccSoft.DbSyncApp.TestUtils.Factories;
using MccSoft.WebApi.SignedUrl;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MccSoft.DbSyncApp.App.Tests
{
    public class SignUrlHelperTests : AppServiceTestBase<SignUrlHelper>
    {
        public SignUrlHelperTests()
        {
            Sut = InitializeService(
                (retryHelper, db) =>
                    new SignUrlHelper(
                        new OptionsWrapper<SignUrlOptions>(
                            new SignUrlOptions() { Secret = "1234567890123456" }
                        ),
                        _userAccessorMock.Object
                    )
            );
        }

        [Fact]
        public void GenerateSignedUrl()
        {
            _userAccessorMock.Setup(x => x.GetUserId()).Returns("5");

            var signature = Sut.GenerateSignature();
            var result = Sut.IsSignatureValid(signature, out var claims);
            result.Should().BeTrue();
            claims.FindFirstValue("id").Should().Be("5");
        }
    }
}
