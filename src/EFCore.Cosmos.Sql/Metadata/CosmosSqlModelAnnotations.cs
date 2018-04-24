// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata
{
    public class CosmosSqlModelAnnotations : ICosmosSqlModelAnnotations
    {
        public CosmosSqlModelAnnotations(IModel model)
            : this(new CosmosSqlAnnotations(model))
        {
        }

        protected CosmosSqlModelAnnotations(CosmosSqlAnnotations annotations) => Annotations = annotations;

        protected virtual CosmosSqlAnnotations Annotations { get; }

        protected virtual IModel Model => (IModel)Annotations.Metadata;

        public virtual string DefaultCollection
        {
            get => (string)Annotations.Metadata[CosmosSqlAnnotationNames.DefaultCollection] ?? "MilkyWay";
            [param: CanBeNull]
            set => SetDefaultCollection(value);
        }

        protected virtual bool SetDefaultCollection([CanBeNull] string value)
            => Annotations.SetAnnotation(
                CosmosSqlAnnotationNames.DefaultCollection,
                Check.NullButNotEmpty(value, nameof(value)));
    }
}
