// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Query
{
    public class CosmosSqlQueryModelVisitor : EntityQueryModelVisitor
    {
        private readonly EntityMaterializerSource _entityMaterializerSource;

        public CosmosSqlQueryModelVisitor([NotNull] EntityQueryModelVisitorDependencies dependencies,
            [NotNull] QueryCompilationContext queryCompilationContext,
            EntityMaterializerSource entityMaterializerSource,
            CosmosSqlQueryModelVisitor parentQueryModelVisitor)
            : base(dependencies, queryCompilationContext)
        {
            _entityMaterializerSource = entityMaterializerSource;
            ParentQueryModelVisitor = parentQueryModelVisitor;
        }

        public virtual CosmosSqlQueryModelVisitor ParentQueryModelVisitor { get; }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);

            if (ParentQueryModelVisitor == null)
            {
                // Inject shapers
                if (Expression is ShapedQueryExpression shapedQuery)
                {
                    var entityShaper = shapedQuery.EntityShaperExpression;

                    var materializerExpression = CreateMaterializer(
                        entityShaper.EntityType,
                        shapedQuery.SelectExpression,
                        (p, se) => se.AddToProjection(p, entityShaper.QuerySource),
                        out var indexMap);

                    var materializer = materializerExpression.Compile();
                }
            }
        }

        public class EntityShaper<TEntity>
            where TEntity : class
        {

        }

        private LambdaExpression CreateMaterializer(
            IEntityType entityType,
            SelectExpression selectExpression,
            Func<IProperty, SelectExpression, int> projectionAdder,
            out Dictionary<Type, int[]> typeIndexMap)
        {
            typeIndexMap = null;

            var materializationContextParameter
                = Expression.Parameter(typeof(MaterializationContext), "materializationContext");

            var indexMap = new int[entityType.PropertyCount()];

            foreach (var property in entityType.GetProperties())
            {
                indexMap[property.GetIndex()] = projectionAdder(property, selectExpression);
            }

            var materializer
                = _entityMaterializerSource
                    .CreateMaterializeExpression(
                        entityType, materializationContextParameter, indexMap);

            return Expression.Lambda(materializer, materializationContextParameter);
        }
    }
}
