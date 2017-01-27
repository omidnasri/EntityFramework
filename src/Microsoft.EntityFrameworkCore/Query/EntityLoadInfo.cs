// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

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
        private readonly Dictionary<Type, int[]> _typeIndexMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityLoadInfo" /> struct.
        /// </summary>
        /// <param name="valueBuffer"> The row of data that represents this entity. </param>
        /// <param name="materializer"> The method to materialize the data into an entity instance. </param>
        /// <param name="typeIndexMap"> Dictionary containing mapping from property indexes to values in ValueBuffer. </param>
        public EntityLoadInfo(
            ValueBuffer valueBuffer,
            [NotNull] Func<ValueBuffer, object> materializer,
            [CanBeNull] Dictionary<Type, int[]> typeIndexMap = null)
        {
            // hot path
            Debug.Assert(materializer != null);

            ValueBuffer = valueBuffer;
            _materializer = materializer;
            _typeIndexMap = typeIndexMap;
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
        public ValueBuffer UpdateValueBuffer([NotNull] Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));

            if (_typeIndexMap == null || !_typeIndexMap.ContainsKey(clrType))
            {
                return ValueBuffer;
            }

            var indexMap = _typeIndexMap[clrType];
            var values = new List<object>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var i in indexMap)
            {
                values.Add(ValueBuffer[i]);
            }

            return new ValueBuffer(values);
        }
    }
}
