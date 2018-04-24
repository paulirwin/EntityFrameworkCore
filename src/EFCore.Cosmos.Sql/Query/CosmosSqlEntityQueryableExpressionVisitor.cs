// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Query
{
    public class CosmosSqlEntityQueryableExpressionVisitor : EntityQueryableExpressionVisitor
    {
        private readonly IQuerySource _querySource;

        public CosmosSqlEntityQueryableExpressionVisitor(
            EntityQueryModelVisitor entityQueryModelVisitor, IQuerySource querySource)
            : base(entityQueryModelVisitor)
        {
            _querySource = querySource;
        }

        protected override Expression VisitEntityQueryable([NotNull] Type elementType)
        {
            throw new NotImplementedException();
        }
    }
}
