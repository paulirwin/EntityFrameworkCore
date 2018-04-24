using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Cosmos.Sql.Extensions;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class CosmosSqlTestHelpers : TestHelpers
    {
        protected CosmosSqlTestHelpers()
        {
        }

        public static CosmosSqlTestHelpers Instance { get; } = new CosmosSqlTestHelpers();

        public override IServiceCollection AddProviderServices(IServiceCollection services)
        {
            return services.AddEntityFrameworkCosmosSql();
        }

        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmosSql(
                new Uri("http://localhost"),
                "dummy",
                "fake");
        }
    }
}
