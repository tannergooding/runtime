// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics
{
    internal static partial class Vector128
    {
        /// <summary>Reinterprets a <see cref="Vector{T}" /> as a new <see cref="Vector128{T}" />.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="value">The vector to reinterpret.</param>
        /// <returns><paramref name="value" /> reinterpreted as a new <see cref="Vector128{T}" />.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="value" /> (<typeparamref name="T" />) is not supported.</exception>
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> AsVector128<T>(this Vector<T> value)
        {
            Debug.Assert(Vector<T>.Count >= Vector128<T>.Count);
            ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();

            ref byte address = ref Unsafe.As<Vector<T>, byte>(ref value);
            return Unsafe.ReadUnaligned<Vector128<T>>(ref address);
        }

        /// <summary>Reinterprets a <see cref="Vector128{T}" /> as a new <see cref="Vector{T}" />.</summary>
        /// <typeparam name="T">The type of the elements in the vector.</typeparam>
        /// <param name="value">The vector to reinterpret.</param>
        /// <returns><paramref name="value" /> reinterpreted as a new <see cref="Vector128{T}" />.</returns>
        /// <exception cref="NotSupportedException">The type of <paramref name="value" /> (<typeparamref name="T" />) is not supported.</exception>
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> AsVector<T>(this Vector128<T> value)
        {
            Debug.Assert(Vector<T>.Count >= Vector128<T>.Count);
            ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();

            Vector<T> result = default;
            Unsafe.WriteUnaligned(ref Unsafe.As<Vector<T>, byte>(ref result), value);
            return result;
        }
    }
}
