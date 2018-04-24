// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class CosmosSqlBuilderExtensionsTest
    {
        [Fact]
        public void Can_set_collection_name()
        {
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder
                .Entity<Customer>()
                .ToCollection("Customizer");

            var entityType = modelBuilder.Model.FindEntityType(typeof(Customer));

            Assert.Equal("Customizer", entityType.CosmosSql().CollectionName);
        }

        [Fact]
        public void Can_set_collection_name_non_generic()
        {
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder
                .Entity(typeof(Customer))
                .ToCollection("Customizer");

            var entityType = modelBuilder.Model.FindEntityType(typeof(Customer));

            Assert.Equal("Customizer", entityType.CosmosSql().CollectionName);
        }

        [Fact]
        public void Can_set_default_collection()
        {
            var modelBuilder = CreateConventionModelBuilder();

            modelBuilder.HasDefaultCollection("Customizer");

            var model = modelBuilder.Model;

            Assert.Equal("Customizer", model.CosmosSql().DefaultCollection);
        }

        protected virtual ModelBuilder CreateConventionModelBuilder()
            => CosmosSqlTestHelpers.Instance.CreateConventionBuilder();

        private class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Guid AlternateId { get; set; }
        }
    }
}
