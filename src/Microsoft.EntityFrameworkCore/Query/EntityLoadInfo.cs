// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     <para>
    ///         Information required to create an instance of an entity based on a row of data returned from a query.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public struct EntityLoadInfo
    {
        private readonly Func<ValueBuffer, object> _materializer;
        private readonly Func<Type, ValueBuffer, ValueBuffer> _remapValueBuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityLoadInfo" /> struct.
        /// </summary>
        /// <param name="valueBuffer"> The row of data that represents this entity. </param>
        /// <param name="materializer"> The method to materialize the data into an entity instance. </param>
        /// <param name="remapValueBuffer"> The method to re-map the valueBuffer into a valueBuffer representing current entity instance. </param>
        public EntityLoadInfo(
            ValueBuffer valueBuffer,
            [NotNull] Func<ValueBuffer, object> materializer,
            Func<Type, ValueBuffer, ValueBuffer> remapValueBuffer = null)
        {
            // hot path
            Debug.Assert(materializer != null);

            ValueBuffer = valueBuffer;
            _materializer = materializer;
            _remapValueBuffer = remapValueBuffer;
        }

        /// <summary>
        ///     Gets the row of data that represents this entity.
        /// </summary>
        public ValueBuffer ValueBuffer { get; }

        /// <summary>
        ///     Materializes the data into an entity instance.
        /// </summary>
        /// <returns> The entity instance. </returns>
        public object Materialize() => _materializer(ValueBuffer);

        /// <summary>
        ///     Remaps the row of data into row which represent current entity instance data.
        /// </summary>
        /// <returns> The entity instance. </returns>
        public ValueBuffer UpdateValueBuffer(Type clrType) => _remapValueBuffer?.Invoke(clrType, ValueBuffer) ?? ValueBuffer;
    }
}
