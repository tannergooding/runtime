// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Intrinsics;

// We mark certain methods with AggressiveInlining to ensure that the JIT will
// inline them. The JIT would otherwise not inline the method since it, at the
// point it tries to determine inline profitability, currently cannot determine
// that most of the code-paths will be optimized away as "dead code".
//
// We then manually inline cases (such as certain intrinsic code-paths) that
// will generate code small enough to make the AggressiveInlining profitable. The
// other cases (such as the software fallback) are placed in their own method.
// This ensures we get good codegen for the "fast-path" and allows the JIT to
// determine inline profitability of the other paths as it would normally.

/// <summary>Represents a mask for a 64-bit vector of a specified numeric type that is suitable for low-level optimization of parallel algorithms.</summary>
/// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(Vector128MaskDebugView<>))]
[StructLayout(LayoutKind.Sequential, Size = Vector128Mask.Size)]
public readonly unsafe struct Vector128Mask<T> : IEquatable<Vector128Mask<T>>
    where T : struct
{
    // This field allows the debug view to work https://github.com/dotnet/runtime/issues/9495)
    internal readonly ushort _value;

    /// <summary>Gets a new <see cref="Vector128Mask{T}" /> that includes all elements of a vector.</summary>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    public static Vector128Mask<T> AllBitsSet
    {
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
            return Unsafe.BitCast<ushort, Vector128Mask<T>>(ushort.MaxValue);
        }
    }

    /// <summary>Gets the number of <typeparamref name="T" /> that are in a <see cref="Vector128Mask{T}" />.</summary>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    public static int Count
    {
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
#pragma warning disable 8500 // sizeof of managed types
            return Vector128Mask.Size / sizeof(T);
#pragma warning restore 8500
        }
    }

    /// <summary>Gets <c>true</c> if <typeparamref name="T" /> is supported; otherwise, <c>false</c>.</summary>
    /// <returns><c>true</c> if <typeparamref name="T" /> is supported; otherwise, <c>false</c>.</returns>
    public static bool IsSupported
    {
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Vector128<T>.IsSupported;
        }
    }

    /// <summary>Gets a new <see cref="Vector128Mask{T}" /> that includes no elements of a vector.</summary>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    public static Vector128Mask<T> Zero
    {
        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
            return default;
        }
    }

    internal string DisplayString
    {
        get
        {
            return IsSupported ? ToString() : SR.NotSupported_Type;
        }
    }

    /// <summary>Gets a bool indicating whether the element at the specified index is included in the mask.</summary>
    /// <param name="index">The index of the element to check for inclusion in the mask.</param>
    /// <returns><c>true</c> if the element at the <paramref name="index" /> is included in the mask; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return this.GetElement(index);
        }
    }

    /// <summary>Computes the bitwise-and of two vector masks.</summary>
    /// <param name="left">The vector mask to bitwise-and with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to bitwise-and with <paramref name="left" />.</param>
    /// <returns>The bitwise-and of <paramref name="left" /> and <paramref name="right"/>.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator &(Vector128Mask<T> left, Vector128Mask<T> right)
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(left._value & right._value);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Computes the bitwise-or of two vector masks.</summary>
    /// <param name="left">The vector mask to bitwise-or with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to bitwise-or with <paramref name="left" />.</param>
    /// <returns>The bitwise-or of <paramref name="left" /> and <paramref name="right"/>.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator |(Vector128Mask<T> left, Vector128Mask<T> right)
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(left._value | right._value);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Compares two vector masks to determine if included elements are equal.</summary>
    /// <param name="left">The vector mask to compare with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to compare with <paramref name="left" />.</param>
    /// <returns><c>true</c> if included elements in <paramref name="left" /> were equal to the included elements in <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector128Mask<T> left, Vector128Mask<T> right)
    {
        // We only want to include bits relevant to the type

        ushort leftValue = (ushort)(left._value & ((1 << Count) - 1));
        ushort rightValue = (ushort)(right._value & ((1 << Count) - 1));

        return leftValue == rightValue;
    }

    /// <summary>Computes the exclusive-or of two vector masks.</summary>
    /// <param name="left">The vector mask to exclusive-or with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to exclusive-or with <paramref name="left" />.</param>
    /// <returns>The exclusive-or of <paramref name="left" /> and <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator ^(Vector128Mask<T> left, Vector128Mask<T> right)
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(left._value ^ right._value);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Compares two vector masks to determine if any elements are not equal.</summary>
    /// <param name="left">The vector mask to compare with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to compare with <paramref name="left" />.</param>
    /// <returns><c>true</c> if included elements in <paramref name="left" /> was not equal to the included elements in <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector128Mask<T> left, Vector128Mask<T> right)
    {
        // We only want to include bits relevant to the type

        ushort leftValue = (ushort)(left._value & ((1 << Count) - 1));
        ushort rightValue = (ushort)(right._value & ((1 << Count) - 1));

        return leftValue != rightValue;
    }

    /// <summary>Shifts a vector mask left by the specified amount.</summary>
    /// <param name="mask">The vector mask to be shifted.</param>
    /// <param name="shiftAmount">The number of bits by which to shift <paramref name="mask" />.</param>
    /// <returns>A vector mask that was shifted left by <paramref name="shiftAmount" />.</returns>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator <<(Vector128Mask<T> mask, int shiftAmount)
    {
        // We only want to include bits relevant to the type
        ushort value = (ushort)(mask._value & ((1 << Count) - 1));

        ushort result = (ushort)(value << shiftAmount);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Computes the ones-complement of a vector mask.</summary>
    /// <param name="mask">The vector mask whose ones-complement is to be computed.</param>
    /// <returns>A vector mask whose included elements are the ones-complement of the included elements in <paramref name="mask" />.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator ~(Vector128Mask<T> mask)
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(~mask._value);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Shifts (signed) a vector mask right by the specified amount.</summary>
    /// <param name="mask">The vector mask to be shifted.</param>
    /// <param name="shiftAmount">The number of bits by which to shift <paramref name="mask" />.</param>
    /// <returns>A vector mask that was shifted right by <paramref name="shiftAmount" />.</returns>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator >>(Vector128Mask<T> mask, int shiftAmount)
    {
        // We only want to include bits relevant to the type
        ushort value = (ushort)(mask._value & ((1 << Count) - 1));

        ushort result = (ushort)(value >> shiftAmount);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Shifts (unsigned) a vector mask right by the specified amount.</summary>
    /// <param name="mask">The vector mask to be shifted.</param>
    /// <param name="shiftAmount">The number of bits by which to shift <paramref name="mask" />.</param>
    /// <returns>A vector mask that was shifted right by <paramref name="shiftAmount" />.</returns>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> operator >>>(Vector128Mask<T> mask, int shiftAmount)
    {
        // We only want to include bits relevant to the type
        ushort value = (ushort)(mask._value & ((1 << Count) - 1));

        ushort result = (ushort)(value >>> shiftAmount);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Determines whether the specified object is equal to the current instance.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="Vector128Mask{T}" /> and is equal to the current instance; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => (obj is Vector128Mask<T> other) && Equals(other);

    /// <summary>Determines whether the specified <see cref="Vector128Mask{T}" /> is equal to the current instance.</summary>
    /// <param name="other">The <see cref="Vector128Mask{T}" /> to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="other" /> is equal to the current instance; otherwise, <c>false</c>.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector128Mask<T> other) => this == other;

    /// <summary>Gets the hash code for the instance.</summary>
    /// <returns>The hash code for the instance.</returns>
    /// <exception cref="NotSupportedException">The type of the vector (<typeparamref name="T" />) is not supported.</exception>
    public override int GetHashCode()
    {
        // We only want to include bits relevant to the type
        ushort value = (ushort)(_value & ((1 << Count) - 1));
        return value.GetHashCode();
    }

    /// <summary>Converts the current instance to an equivalent string representation.</summary>
    /// <returns>An equivalent string representation of the current instance.</returns>
    /// <exception cref="NotSupportedException">The type of the vector mask (<typeparamref name="T" />) is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => _value.ToString("B", CultureInfo.InvariantCulture);
}
