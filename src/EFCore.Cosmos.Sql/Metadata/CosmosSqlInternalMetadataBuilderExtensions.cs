// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata
{
    public static class CosmosSqlInternalMetadataBuilderExtensions
    {
        public static CosmosSqlEntityTypeBuilderAnnotations CosmosSql(
            [NotNull] this InternalEntityTypeBuilder builder,
            ConfigurationSource configurationSource)
            => new CosmosSqlEntityTypeBuilderAnnotations(builder, configurationSource);

        public static CosmosSqlModelBuilderAnnotations CosmosSql(
            [NotNull] this InternalModelBuilder builder,
            ConfigurationSource configurationSource)
            => new CosmosSqlModelBuilderAnnotations(builder, configurationSource);
    }
}
