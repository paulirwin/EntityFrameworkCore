// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Query
{
    public class CosmosSqlEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private readonly IModel _model;
        private readonly IQuerySource _querySource;

        public CosmosSqlEntityQueryableExpressionVisitor(
            IModel model,
            EntityQueryModelVisitor entityQueryModelVisitor, IQuerySource querySource)
            : base(entityQueryModelVisitor)
        {
            _model = model;
            _querySource = querySource;
        }

        protected override Expression VisitEntityQueryable([NotNull] Type elementType)
        {
            Check.NotNull(elementType, nameof(elementType));

            var entityType = _model.FindEntityType(elementType);

            var collectionName = entityType.CosmosSql().CollectionName;
            var selectExpression = new SelectExpression(collectionName);
            var fromAlias = collectionName[0].ToString().ToLowerInvariant();

            selectExpression.FromExpressions.Add(new FromExpression(_querySource, fromAlias, entityType));

            return new ShapedQueryExpression(collectionName, selectExpression, new EntityShaperExpression(_querySource, entityType));
        }
    }

    public class EntityShaperExpression : Expression
    {

        public EntityShaperExpression(IQuerySource querySource, IEntityType entityType)
        {
            QuerySource = querySource;
            EntityType = entityType;
        }
        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => EntityType.ClrType;

        public IQuerySource QuerySource { get; }
        public IEntityType EntityType { get; }
    }

    public class ShapedQueryExpression : Expression
    {
        public ShapedQueryExpression(string collectionName, SelectExpression selectExpression, EntityShaperExpression entityShaperExpression)
        {
            CollectionName = collectionName;
            SelectExpression = selectExpression;
            EntityShaperExpression = entityShaperExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => typeof(IEnumerable<>).MakeGenericType(EntityShaperExpression.Type);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return this;
        }

        public string CollectionName { get; }
        public SelectExpression SelectExpression { get; }
        public EntityShaperExpression EntityShaperExpression { get; }
    }

    public class SelectExpression : Expression
    {
        public SelectExpression(string collectionName)
        {
            CollectionName = collectionName;
        }
        public List<Expression> Projection { get; } = new List<Expression>();
        public List<FromExpression> FromExpressions { get; } = new List<FromExpression>();
        public Expression FilterExpression { get; set; }
        public string CollectionName { get; }
        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => typeof(object);

        public int AddToProjection(IProperty property, IQuerySource querySource)
        {
            var fromExpression = FromExpressions.Single(f => f.QuerySource == querySource);

            var column = new ColumnExpression(property.Name, property, fromExpression);

            Projection.Add(column);

            return Projection.Count - 1;
        }
    }

    public class FromExpression : Expression
    {
        public FromExpression(IQuerySource querySource, string alias, IEntityType entityType)
        {
            QuerySource = querySource;
            Alias = alias;
            EntityType = entityType;
        }

        public IQuerySource QuerySource { get; }
        public string Alias { get; }
        public IEntityType EntityType { get; }
        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => typeof(object);
    }

    public class ColumnExpression : Expression
    {
        public ColumnExpression(
            string name,
            IProperty property,
            FromExpression fromExpression)
        {
            Name = name;
            Property = property;
            FromExpression = fromExpression;
        }

        public string Name { get; }
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public IProperty Property { get; }
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        public FromExpression FromExpression { get; }
        public override Type Type => Property.ClrType;
        public override ExpressionType NodeType => ExpressionType.Extension;
    }
}
