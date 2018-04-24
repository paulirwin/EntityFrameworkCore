// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Query
{
    public class CosmosSqlEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        public CosmosSqlEntityQueryableExpressionVisitorFactory()
        {
        }

        public ExpressionVisitor Create(EntityQueryModelVisitor entityQueryModelVisitor, IQuerySource querySource)
        {
            return new CosmosSqlEntityQueryableExpressionVisitor(
                entityQueryModelVisitor,
                querySource);
        }
    }
}
