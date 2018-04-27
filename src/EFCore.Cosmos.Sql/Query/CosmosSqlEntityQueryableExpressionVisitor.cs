// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

            var queryCompilationContext = QueryModelVisitor.QueryCompilationContext;

            var entityType = queryCompilationContext.FindEntityType(_querySource)
                             ?? _model.FindEntityType(elementType);

            var collectionName = entityType.CosmosSql().CollectionName;
            var selectExpression = new SelectExpression(collectionName);
            var fromAlias = collectionName[0].ToString().ToLowerInvariant();

            selectExpression.FromExpressions.Add(new FromExpression(_querySource, fromAlias, entityType));

            return new QueryExpression(collectionName, selectExpression);
        }
    }

    public class QueryExpression : Expression
    {
        public QueryExpression(string collectionName, SelectExpression selectExpression)
        {
            CollectionName = collectionName;
            SelectExpression = selectExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;
        public override Type Type => typeof(object);

        public string CollectionName { get; }
        public SelectExpression SelectExpression { get; }
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
