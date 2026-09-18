// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;

public static class PermuteToShuffle
{
    [ConditionalTheory(typeof(Avx), nameof(Avx.IsSupported))]
    [InlineData(false)]
    [InlineData(true)]
    public static void AllControls(bool specialValues)
    {
        Vector256<float> singles = specialValues
            ? Vector256.Create(0x80000000u, 0u, 1u, 0x7F800000u, 0xFF800000u, 0x7FC12345u, 0x7FA12345u, 0xFFC54321u).AsSingle()
            : Vector256.Create(1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f);
        Vector256<double> doubles = specialValues
            ? Vector256.Create(0x8000000000000000ul, 1ul, 0x7FF8123456789ABCul, 0x7FF0123456789ABCul).AsDouble()
            : Vector256.Create(1d, 2d, 3d, 4d);

        for (int control = 0; control <= byte.MaxValue; control++)
        {
            Vector128<float> single128 = PermuteSingle128(singles.GetLower(), Vector128<float>.Zero, (byte)control);
            Vector256<float> single256 = PermuteSingle256(singles, Vector256<float>.Zero, (byte)control);
            Vector128<double> double128 = PermuteDouble128(doubles.GetLower(), Vector128<double>.Zero, (byte)control);
            Vector256<double> double256 = PermuteDouble256(doubles, Vector256<double>.Zero, (byte)control);

            for (int i = 0; i < Vector256<float>.Count; i++)
            {
                int index = (i & ~3) + ((control >> (2 * (i & 3))) & 3);
                uint expected = singles.AsUInt32().GetElement(index);
                Assert.Equal(expected, single256.AsUInt32().GetElement(i));
                if (i < Vector128<float>.Count)
                {
                    Assert.Equal(expected, single128.AsUInt32().GetElement(i));
                }
            }

            for (int i = 0; i < Vector256<double>.Count; i++)
            {
                int index = (i & ~1) + ((control >> i) & 1);
                ulong expected = doubles.AsUInt64().GetElement(index);
                Assert.Equal(expected, double256.AsUInt64().GetElement(i));
                if (i < Vector128<double>.Count)
                {
                    Assert.Equal(expected, double128.AsUInt64().GetElement(i));
                }
            }
        }

        Vector128<float> singlesLower = singles.GetLower();
        Vector128<double> doublesLower = doubles.GetLower();
        Vector128<uint> constantSingle128 = ConstantSingle128(singlesLower, Vector128<float>.Zero).AsUInt32();
        Vector256<uint> constantSingle256 = ConstantSingle256(singles, Vector256<float>.Zero).AsUInt32();
        Vector128<ulong> constantDouble128 = ConstantDouble128(doublesLower, Vector128<double>.Zero).AsUInt64();
        Vector256<ulong> constantDouble256 = ConstantDouble256(doubles, Vector256<double>.Zero).AsUInt64();
        Vector128<uint> memorySingle128 = MemorySingle128(ref singlesLower).AsUInt32();
        Vector256<uint> memorySingle256 = MemorySingle256(ref singles).AsUInt32();
        Vector128<ulong> memoryDouble128 = MemoryDouble128(ref doublesLower).AsUInt64();
        Vector256<ulong> memoryDouble256 = MemoryDouble256(ref doubles).AsUInt64();

        for (int i = 0; i < Vector256<float>.Count; i++)
        {
            Assert.Equal(singles.AsUInt32().GetElement(i ^ 2), constantSingle256.GetElement(i));
            Assert.Equal(constantSingle256.GetElement(i), memorySingle256.GetElement(i));
            if (i < Vector128<float>.Count)
            {
                Assert.Equal(singles.AsUInt32().GetElement(i ^ 1), constantSingle128.GetElement(i));
                Assert.Equal(constantSingle128.GetElement(i), memorySingle128.GetElement(i));
            }
        }

        for (int i = 0; i < Vector256<double>.Count; i++)
        {
            Assert.Equal(doubles.AsUInt64().GetElement(i ^ 1), constantDouble256.GetElement(i));
            Assert.Equal(constantDouble256.GetElement(i), memoryDouble256.GetElement(i));
            if (i < Vector128<double>.Count)
            {
                Assert.Equal(doubles.AsUInt64().GetElement(i ^ 1), constantDouble128.GetElement(i));
                Assert.Equal(constantDouble128.GetElement(i), memoryDouble128.GetElement(i));
            }
        }

        Vector128<float>[] values = new Vector128<float>[12];
        Vector128<uint> expectedPressure = Vector128<uint>.Zero;
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = singlesLower ^ Vector128.Create((uint)(i + 1), 1u << i, 3u << i, 5u << i).AsSingle();
            expectedPressure ^= Vector128.Create(
                values[i].AsUInt32().GetElement(1), values[i].AsUInt32().GetElement(0),
                values[i].AsUInt32().GetElement(3), values[i].AsUInt32().GetElement(2));
        }

        Assert.Equal(expectedPressure, RegisterPressure(values, Vector128<float>.Zero).AsUInt32());
    }

    // The XOR keeps the permute source in a register without evaluating NaNs.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> PermuteSingle128(Vector128<float> value, Vector128<float> mask, byte control)
    {
        return Avx.Permute(value ^ mask, control);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<float> PermuteSingle256(Vector256<float> value, Vector256<float> mask, byte control)
    {
        return Avx.Permute(value ^ mask, control);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<double> PermuteDouble128(Vector128<double> value, Vector128<double> mask, byte control)
    {
        return Avx.Permute(value ^ mask, control);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<double> PermuteDouble256(Vector256<double> value, Vector256<double> mask, byte control)
    {
        return Avx.Permute(value ^ mask, control);
    }

    // Run these disassembly checks separately with AVX enabled and without register or EVEX stress.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> ConstantSingle128(Vector128<float> value, Vector128<float> mask)
    {
        // X64: vshufps
        return Avx.Permute(value ^ mask, 0xB1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<float> ConstantSingle256(Vector256<float> value, Vector256<float> mask)
    {
        // X64: vshufps
        return Avx.Permute(value ^ mask, 0x4E);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<double> ConstantDouble128(Vector128<double> value, Vector128<double> mask)
    {
        // X64: vshufpd
        return Avx.Permute(value ^ mask, 0x01);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<double> ConstantDouble256(Vector256<double> value, Vector256<double> mask)
    {
        // X64: vshufpd
        return Avx.Permute(value ^ mask, 0x05);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> MemorySingle128(ref Vector128<float> value)
    {
        // X64: vpermilps
        return Avx.Permute(value, 0xB1);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<float> MemorySingle256(ref Vector256<float> value)
    {
        // X64: vpermilps
        return Avx.Permute(value, 0x4E);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<double> MemoryDouble128(ref Vector128<double> value)
    {
        // X64: vpermilpd
        return Avx.Permute(value, 0x01);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector256<double> MemoryDouble256(ref Vector256<double> value)
    {
        // X64: vpermilpd
        return Avx.Permute(value, 0x05);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> RegisterPressure(Vector128<float>[] values, Vector128<float> mask)
    {
        Vector128<float> v0 = Avx.Permute(values[0] ^ mask, 0xB1);
        Vector128<float> v1 = Avx.Permute(values[1] ^ mask, 0xB1);
        Vector128<float> v2 = Avx.Permute(values[2] ^ mask, 0xB1);
        Vector128<float> v3 = Avx.Permute(values[3] ^ mask, 0xB1);
        Vector128<float> v4 = Avx.Permute(values[4] ^ mask, 0xB1);
        Vector128<float> v5 = Avx.Permute(values[5] ^ mask, 0xB1);
        Vector128<float> v6 = Avx.Permute(values[6] ^ mask, 0xB1);
        Vector128<float> v7 = Avx.Permute(values[7] ^ mask, 0xB1);
        Vector128<float> v8 = Avx.Permute(values[8] ^ mask, 0xB1);
        Vector128<float> v9 = Avx.Permute(values[9] ^ mask, 0xB1);
        Vector128<float> v10 = Avx.Permute(values[10] ^ mask, 0xB1);
        Vector128<float> v11 = Avx.Permute(values[11] ^ mask, 0xB1);
        return v0 ^ v1 ^ v2 ^ v3 ^ v4 ^ v5 ^ v6 ^ v7 ^ v8 ^ v9 ^ v10 ^ v11;
    }

    [Theory]
    [InlineData(1f, 2f, 3f, 4f)]
    [InlineData(-1f, 0f, -3f, 2f)]
    public static void ReductionConsumers(float x, float y, float z, float w)
    {
        Vector128<float> value = Vector128.Create(x, y, z, w);
        float expected = ((x * x) + (y * y)) + ((z * z) + (w * w));
        Assert.Equal(expected, ScalarDot(value));
        Assert.Equal(Vector128.Create(expected), BroadcastDot(value));
        Assert.Equal(Vector128.Create(MathF.Sqrt(expected)), BroadcastSqrtDot(value));
        Assert.Equal(Vector128.Create(expected * 2), MixedDot(value));
        Assert.Equal((x + y) + (z + w), ScalarSum(value));
        Assert.Equal(Vector128.Create(y, x, w, z), GenericShuffle(value));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float ScalarDot(Vector128<float> value)
    {
        return Vector128.Dot(value, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> BroadcastDot(Vector128<float> value)
    {
        return Vector128.Create(Vector128.Dot(value, value));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> BroadcastSqrtDot(Vector128<float> value)
    {
        return Vector128.Create(MathF.Sqrt(Vector128.Dot(value, value)));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> MixedDot(Vector128<float> value)
    {
        float scalar = Vector128.Dot(value, value);
        Vector128<float> broadcast = Vector128.Create(Vector128.Dot(value, value));
        return broadcast + Vector128.Create(scalar);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float ScalarSum(Vector128<float> value)
    {
        return Vector128.Sum(value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Vector128<float> GenericShuffle(Vector128<float> value)
    {
        return Vector128.Shuffle(value, Vector128.Create(1, 0, 3, 2));
    }
}
