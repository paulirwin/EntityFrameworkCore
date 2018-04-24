// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata
{
    public class CosmosSqlEntityTypeBuilderAnnotations : CosmosSqlEntityTypeAnnotations
    {
        public CosmosSqlEntityTypeBuilderAnnotations(
            InternalEntityTypeBuilder internalBuilder,
            ConfigurationSource configurationSource)
            : base(new CosmosSqlAnnotationsBuilder(internalBuilder, configurationSource))
        {
        }

        protected new virtual CosmosSqlAnnotationsBuilder Annotations => (CosmosSqlAnnotationsBuilder)base.Annotations;

        protected virtual InternalEntityTypeBuilder EntityTypeBuilder
            => (InternalEntityTypeBuilder)Annotations.MetadataBuilder;

        public virtual bool ToCollection([CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, nameof(name));

            return SetCollectionName(name);
        }
    }
}
