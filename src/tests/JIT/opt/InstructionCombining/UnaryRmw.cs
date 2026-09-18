// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Xunit;

public class UnaryRmw
{
    [Fact]
    public static void PreferOperands()
    {
        Assert.Equal(4, NegatePair(1, 2, 3, 4));
        Assert.Equal(4, ComplementPair(1, 2, 3, 4));
        Assert.Equal(0x04000000, ByteSwapPair(1, 2, 3, 4));
        Assert.Equal(0x0400, ByteSwap16Pair(1, 2, 3, 4));
        Assert.Equal(3UL, DividePair(3, 4, 6, 8));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0x12345678)]
    public static void IntegerOperations(int value)
    {
        Assert.Equal(unchecked(-value), Negate(value));
        Assert.Equal(~value, Complement(value));
        Assert.Equal(unchecked(-value) ^ value, NegateWithLiveSource(value));
        Assert.Equal(unchecked(~value - value), ComplementWithLiveSource(value));
        Assert.Equal(BinaryPrimitives.ReverseEndianness(value), ByteSwap(value));
        Assert.Equal(BinaryPrimitives.ReverseEndianness((ushort)value), ByteSwap16((ushort)value));
        Assert.Equal(BinaryPrimitives.ReverseEndianness(value), ByteSwapMemory(ref value));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    [InlineData(0x123456789ABCDEFL)]
    public static void LongOperations(long value)
    {
        Assert.Equal(unchecked(-value), NegateLong(value));
        Assert.Equal(~value, ComplementLong(value));
        Assert.Equal(BinaryPrimitives.ReverseEndianness(value), ByteSwapLong(value));
        Assert.Equal((ulong)value / 7, DivideBySeven((ulong)value));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(long.MinValue)]
    [InlineData(0x3FF0000000000000L)]
    [InlineData(0x7FF0000000000000L)]
    [InlineData(unchecked((long)0xFFF0000000000000))]
    [InlineData(0x7FF8123456789ABCL)]
    [InlineData(unchecked((long)0xFFF8123456789ABC))]
    public static void FloatingOperations(long bits)
    {
        double value = BitConverter.Int64BitsToDouble(bits);
        Assert.Equal(bits ^ long.MinValue, BitConverter.DoubleToInt64Bits(NegateDouble(value)));
        Assert.Equal(bits & long.MaxValue, BitConverter.DoubleToInt64Bits(AbsDouble(value)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    [InlineData(0x3F800000)]
    [InlineData(0x7F800000)]
    [InlineData(unchecked((int)0xFF800000))]
    [InlineData(0x7FC12345)]
    [InlineData(unchecked((int)0xFFC12345))]
    public static void SingleOperations(int bits)
    {
        float value = BitConverter.Int32BitsToSingle(bits);
        Assert.Equal(bits ^ int.MinValue, BitConverter.SingleToInt32Bits(NegateSingle(value)));
        Assert.Equal(bits & int.MaxValue, BitConverter.SingleToInt32Bits(AbsSingle(value)));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Negate(int value) => -value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Complement(int value) => ~value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NegateWithLiveSource(int value) => -value ^ value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ComplementWithLiveSource(int value) => ~value - value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long NegateLong(long value) => -value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long ComplementLong(long value) => ~value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ByteSwap(int value) => BinaryPrimitives.ReverseEndianness(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ushort ByteSwap16(ushort value) => BinaryPrimitives.ReverseEndianness(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long ByteSwapLong(long value) => BinaryPrimitives.ReverseEndianness(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ByteSwapMemory(ref int value) => BinaryPrimitives.ReverseEndianness(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong DivideBySeven(ulong value) => value / 7;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static double NegateDouble(double value) => -value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static double AbsDouble(double value) => Math.Abs(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float NegateSingle(float value) => -value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float AbsSingle(float value) => MathF.Abs(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int NegatePair(int a, int b, int c, int d)
    {
        // X64-WINDOWS: neg
        // X64-WINDOWS: add [[REG:[a-z0-9]+]], {{[a-z0-9]+}}
        // X64-WINDOWS-NEXT: neg [[REG]]
        return -(a + b) ^ -(c + d);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ComplementPair(int a, int b, int c, int d)
    {
        // X64-WINDOWS: not
        // X64-WINDOWS: add [[REG:[a-z0-9]+]], {{[a-z0-9]+}}
        // X64-WINDOWS-NEXT: not [[REG]]
        return ~(a + b) - ~(c + d);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ByteSwapPair(int a, int b, int c, int d)
    {
        // X64-WINDOWS: bswap
        // X64-WINDOWS: add [[REG:[a-z0-9]+]], {{[a-z0-9]+}}
        // X64-WINDOWS-NEXT: bswap [[REG]]
        return BinaryPrimitives.ReverseEndianness(a + b) ^ BinaryPrimitives.ReverseEndianness(c + d);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ByteSwap16Pair(int a, int b, int c, int d)
    {
        // X64-WINDOWS: add
        // X64-WINDOWS-NEXT: ror
        return BinaryPrimitives.ReverseEndianness((ushort)(a + b)) ^ BinaryPrimitives.ReverseEndianness((ushort)(c + d));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong DividePair(ulong a, ulong b, ulong c, ulong d)
    {
        return (a + b) / 7 + (c + d) / 7;
    }
}
