// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata
{
    public class CosmosSqlModelBuilderAnnotations : CosmosSqlModelAnnotations
    {
        public CosmosSqlModelBuilderAnnotations(
            InternalModelBuilder internalBuilder,
            ConfigurationSource configurationSource)
            : base(new CosmosSqlAnnotationsBuilder(internalBuilder, configurationSource))
        {
        }

        protected new virtual CosmosSqlAnnotationsBuilder Annotations => (CosmosSqlAnnotationsBuilder)base.Annotations;

        protected virtual InternalModelBuilder ModelBuilder
            => (InternalModelBuilder)Annotations.MetadataBuilder;

        public virtual bool HasDefaultCollection([CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, nameof(name));

            return SetDefaultCollection(name);
        }
    }
}
