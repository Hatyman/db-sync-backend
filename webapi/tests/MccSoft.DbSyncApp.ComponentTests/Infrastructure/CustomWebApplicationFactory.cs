﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using MccSoft.PersistenceHelpers.DomainEvents;
using MccSoft.DbSyncApp.App;
using MccSoft.DbSyncApp.Persistence;
using MccSoft.Testing.SqliteUtils;
using Moq;

namespace MccSoft.DbSyncApp.ComponentTests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        private readonly Action<IServiceCollection> _overrideServices;
        private readonly string _databaseFileName;

        public CustomWebApplicationFactory(
            Action<IServiceCollection> overrideServices,
            string databaseFileName
        )
        {
            _overrideServices = overrideServices;
            _databaseFileName = databaseFileName;
        }

        protected override IWebHostBuilder CreateWebHostBuilder() =>
            Program.CreateWebHostBuilder<TestStartup>(args: null).UseEnvironment("Test");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSolutionRelativeContentRoot(
                solutionRelativePath: "src/MccSoft.DbSyncApp.App",
                applicationBasePath: "../../../../../"
            );
            builder.ConfigureTestServices(_overrideServices);

            builder.ConfigureServices(
                services =>
                {
                    services.AddDomainEventsWithMediatR(typeof(Startup));
                    services.AddSqliteInMemory<DbSyncAppDbContext>(
                        _databaseFileName,
                        (builder, provider) => builder.AddDomainEventsInterceptors(provider)
                    );
                }
            );
        }
    }
}
