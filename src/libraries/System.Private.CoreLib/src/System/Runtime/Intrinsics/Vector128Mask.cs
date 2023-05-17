// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics;

/// <summary>Provides a collection of static methods for creating, manipulating, and otherwise operating on masks for 64-bit vectors.</summary>
public static class Vector128Mask
{
    internal const int Size = 2;

    internal const int Alignment = 2;

    /// <summary>Gets a value that indicates whether mask operations for 64-bit vectors are subject to hardware acceleration through JIT intrinsic support.</summary>
    /// <value><see langword="true" /> if mask operations for 64-bit vectors are subject to hardware acceleration; otherwise, <see langword="false" />.</value>
    /// <remarks>Mask operations for 64-bit vector are subject to hardware acceleration on systems that support Single Instruction, Multiple Data (SIMD) instructions for 64-bit vectors.</remarks>
    public static bool IsHardwareAccelerated
    {
        [Intrinsic]
        get => IsHardwareAccelerated;
    }

    /// <summary>Computes the bitwise-and of a given vector mask and the ones complement of another vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to bitwise-and with <paramref name="right" />.</param>
    /// <param name="right">The vector mask that is ones-complemented before being bitwise-and with <paramref name="left" />.</param>
    /// <returns>The bitwise-and of <paramref name="left" /> and the ones-complement of <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> AndNot<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(left._value & ~right._value);
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    /// <summary>Computes the bitwise-and of two vector masks.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to bitwise-and with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to bitwise-and with <paramref name="left" />.</param>
    /// <returns>The bitwise-and of <paramref name="left" /> and <paramref name="right"/>.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> BitwiseAnd<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct => left & right;

    /// <summary>Computes the bitwise-or of two vector masks.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to bitwise-or with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to bitwise-or with <paramref name="left" />.</param>
    /// <returns>The bitwise-or of <paramref name="left" /> and <paramref name="right"/>.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> BitwiseOr<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct => left | right;

    /// <summary>Creates a new <see cref="Vector128Mask{T}" /> instance from the specified value.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="value">The value that vector mask will be initialized to.</param>
    /// <returns>A new <see cref="Vector128Mask{T}" /> initialized to <paramref name="value" />.</returns>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> Create<T>(ushort value)
        where T : struct => Unsafe.BitCast<ushort, Vector128Mask<T>>(value);

    /// <summary>Compares two vector masks to determine if included elements are equal.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to compare with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to compare with <paramref name="left" />.</param>
    /// <returns><c>true</c> if included elements in <paramref name="left" /> were equal to the included elements in <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct => left == right;

    /// <summary>Gets a bool indicating whether the element at the specified index is included in the vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to get the element inclusion from.</param>
    /// <param name="index">The index of the element to check for inclusion in the mask.</param>
    /// <returns><c>true</c> if the element at the <paramref name="index" /> is included in <paramref name="mask" />; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetElement<T>(this Vector128Mask<T> mask, int index)
        where T : struct
    {
        if ((uint)(index) >= (uint)(Vector128Mask<T>.Count))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
        }

        return mask.GetElementUnsafe(index);
    }

    /// <summary>Computes the number of leading zero bits in a vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose leading zero bits are to be counted.</param>
    /// <returns>The number of leading zero bits in <paramref name="mask" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort LeadingZeroCount<T>(Vector128Mask<T> mask)
        where T : struct
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        return ushort.LeadingZeroCount(mask._value);
    }

    /// <summary>Computes the ones-complement of a vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose ones-complement is to be computed.</param>
    /// <returns>A vector mask whose included elements are the ones-complement of the included elements in <paramref name="mask" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> OnesComplement<T>(Vector128Mask<T> mask)
        where T : struct => ~mask;

    /// <summary>Computes the number of bits that are set in a vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose set bits are to be counted.</param>
    /// <returns>The number of set bits in <paramref name="mask" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort PopCount<T>(Vector128Mask<T> mask)
        where T : struct
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        return ushort.PopCount(mask._value);
    }

    /// <summary>Shifts a vector mask left by the specified amount.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose to be shifted.</param>
    /// <param name="shiftAmount">The number of bits by which to shift <paramref name="mask"/>.</param>
    /// <returns>A vector mask that was shifted left by <paramref name="shiftAmount" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> ShiftLeft<T>(Vector128Mask<T> mask, int shiftAmount)
        where T : struct => mask << shiftAmount;

    /// <summary>Shifts (unsigned) a vector mask right by the specified amount.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose to be shifted.</param>
    /// <param name="shiftAmount">The number of bits by which to shift <paramref name="mask"/>.</param>
    /// <returns>A vector mask that was shifted right by <paramref name="shiftAmount" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> ShiftRightLogical<T>(Vector128Mask<T> mask, int shiftAmount)
        where T : struct => mask >> shiftAmount;

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{TTo}" />.</summary>
    /// <typeparam name="TFrom">The type of the elements included by the input vector mask.</typeparam>
    /// <typeparam name="TTo">The type of the elements included by the output vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{TTo}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="TFrom" />) or the type of the target (<typeparamref name="TTo" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<TTo> To<TFrom, TTo>(this Vector128Mask<TFrom> mask)
        where TFrom : struct
        where TTo : struct
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<TFrom>();
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<TTo>();

        return Unsafe.BitCast<Vector128Mask<TFrom>, Vector128Mask<TTo>>(mask);
    }

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Byte}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Byte}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<byte> ToByte<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, byte>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Double}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Double}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<double> ToDouble<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, double>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Int16}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Int16}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<short> ToInt16<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, short>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Int32}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Int32}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<int> ToInt32<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, int>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Int64}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Int64}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<long> ToInt64<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, long>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{IntPtr}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{IntPtr}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<nint> ToNInt<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, nint>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{UIntPtr}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{UIntPtr}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<nuint> ToNUInt<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, nuint>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{SByte}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{SByte}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<sbyte> ToSByte<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, sbyte>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{Single}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{Single}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<float> ToSingle<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, float>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{UInt16}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{UInt16}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<ushort> ToUInt16<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, ushort>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{UInt32}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{UInt32}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<uint> ToUInt32<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, uint>();

    /// <summary>Reinterprets a <see cref="Vector128Mask{TFrom}" /> as a new <see cref="Vector128Mask{UInt64}" />.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask to reinterpret.</param>
    /// <returns><paramref name="mask" /> reinterpreted as a new <see cref="Vector128Mask{UInt64}" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<ulong> ToUInt64<T>(this Vector128Mask<T> mask)
        where T : struct => mask.To<T, ulong>();

    /// <summary>Computes the number of trailing zero bits in a vector mask.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="mask">The vector mask whose trailing zero bits are to be counted.</param>
    /// <returns>The number of trailing zero bits in <paramref name="mask" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [CLSCompliant(false)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort TrailingZeroCount<T>(Vector128Mask<T> mask)
        where T : struct => ushort.TrailingZeroCount(mask._value);

    /// <summary>Creates a new <see cref="Vector128{T}" /> with the element at the specified index set to the specified value and the remaining elements set to the same value as that in the given vector.</summary>
    /// <typeparam name="T">The type of the elements in the vector.</typeparam>
    /// <param name="mask">The vector to get the remaining elements from.</param>
    /// <param name="index">The index of the element to set.</param>
    /// <param name="value">The value to set the element to.</param>
    /// <returns>A <see cref="Vector128{T}" /> with the value of the element at <paramref name="index" /> set to <paramref name="value" /> and the remaining elements set to the same value as that in <paramref name="mask" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    /// <exception cref="NotSupportedException">The type of <paramref name="mask" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> WithElement<T>(this Vector128Mask<T> mask, int index, bool value)
        where T : struct
    {
        if ((uint)(index) >= (uint)(Vector128Mask<T>.Count))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
        }

        Vector128Mask<T> result = mask;
        mask.SetElementUnsafe(index, value);
        return result;
    }

    /// <summary>Computes the exclusive-or of two vector masks.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to exclusive-or with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to exclusive-or with <paramref name="left" />.</param>
    /// <returns>The exclusive-or of <paramref name="left" /> and <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> Xor<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct => left ^ right;

    /// <summary>Computes the exclusive-or of two vector masks.</summary>
    /// <typeparam name="T">The type of the elements included by the vector mask.</typeparam>
    /// <param name="left">The vector mask to exclusive-or with <paramref name="right" />.</param>
    /// <param name="right">The vector mask to exclusive-or with <paramref name="left" />.</param>
    /// <returns>The exclusive-or of <paramref name="left" /> and <paramref name="right" />.</returns>
    /// <exception cref="NotSupportedException">The type of <paramref name="left" /> and <paramref name="right" /> (<typeparamref name="T" />) is not supported.</exception>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128Mask<T> Xnor<T>(Vector128Mask<T> left, Vector128Mask<T> right)
        where T : struct
    {
        ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
        ushort result = (ushort)(~(left._value ^ ~right._value));
        return Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetElementUnsafe<T>(in this Vector128Mask<T> mask, int index)
        where T : struct
    {
        Debug.Assert((index >= 0) && (index < Vector128Mask<T>.Count));
        return ((mask._value >> index) & 1) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SetElementUnsafe<T>(in this Vector128Mask<T> mask, int index, bool value)
        where T : struct
    {
        Debug.Assert((index >= 0) && (index < Vector128Mask<T>.Count));
        ushort result = (ushort)((mask._value & ~(1 << index)) | ((value ? 1 : 0) << index));
        Unsafe.AsRef(in mask) = Unsafe.BitCast<ushort, Vector128Mask<T>>(result);
    }
}
