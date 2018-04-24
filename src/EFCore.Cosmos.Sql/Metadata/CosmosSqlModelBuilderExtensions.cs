// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata
{
    public static class CosmosSqlModelBuilderExtensions
    {
        public static ModelBuilder HasDefaultCollection(
            [NotNull] this ModelBuilder modelBuilder,
            [CanBeNull] string collection)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(collection, nameof(collection));

            modelBuilder.GetInfrastructure().CosmosSql(ConfigurationSource.Explicit).HasDefaultCollection(collection);

            return modelBuilder;
        }
    }
}
