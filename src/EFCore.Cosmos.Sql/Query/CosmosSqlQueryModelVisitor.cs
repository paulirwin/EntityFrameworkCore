// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Query
{
    public class CosmosSqlQueryModelVisitor : EntityQueryModelVisitor
    {
        public CosmosSqlQueryModelVisitor([NotNull] EntityQueryModelVisitorDependencies dependencies,
            [NotNull] QueryCompilationContext queryCompilationContext,
            CosmosSqlQueryModelVisitor parentQueryModelVisitor)
            : base(dependencies, queryCompilationContext)
        {
            ParentQueryModelVisitor = parentQueryModelVisitor;
        }

        public virtual CosmosSqlQueryModelVisitor ParentQueryModelVisitor { get; }
    }
}
