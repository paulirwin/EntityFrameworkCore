// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata.Conventions
{
    public class CosmosSqlConventionSetBuilder : IConventionSetBuilder
    {
        public ConventionSet AddConventions(ConventionSet conventionSet)
        {
            conventionSet.EntityTypeAddedConventions.Add(new EntityTypeDiscriminatorConvention());

            return conventionSet;
        }
    }

    public class EntityTypeDiscriminatorConvention : IEntityTypeAddedConvention
    {
        public InternalEntityTypeBuilder Apply(InternalEntityTypeBuilder entityTypeBuilder)
        {
            var propertyBuilder = entityTypeBuilder.Property("Discriminator", typeof(string), ConfigurationSource.Convention);
            entityTypeBuilder.Metadata.CosmosSql().DiscriminatorProperty = propertyBuilder.Metadata;
            entityTypeBuilder.Metadata.CosmosSql().DiscriminatorValue = entityTypeBuilder.Metadata.ShortName();
            propertyBuilder.IsRequired(true, ConfigurationSource.Convention);
            propertyBuilder.AfterSave(PropertySaveBehavior.Throw, ConfigurationSource.Convention);
            propertyBuilder.HasValueGenerator(
                (property, entityType) => new DiscriminatorValueGenerator(entityType.CosmosSql().DiscriminatorValue),
                ConfigurationSource.Convention);

            return entityTypeBuilder;
        }
    }

    public class DiscriminatorValueGenerator : ValueGenerator
    {
        private readonly object _discriminator;

        public DiscriminatorValueGenerator(object discriminator)
        {
            _discriminator = discriminator;
        }

        protected override object NextValue([NotNull] EntityEntry entry) => _discriminator;

        public override bool GeneratesTemporaryValues => false;
    }
}
