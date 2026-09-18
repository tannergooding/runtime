// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;

public class Runtime_133753
{
    public static IEnumerable<object[]> TestData()
    {
        foreach (int width in new[] { 128, 256, 512 })
        {
            foreach (bool words in new[] { false, true })
            {
                foreach (bool saturate in new[] { false, true })
                {
                    yield return new object[] { width, words, saturate };
                }
            }
        }
    }

    [ConditionalTheory(typeof(AvxVnni.V512), nameof(AvxVnni.V512.IsSupported))]
    [MemberData(nameof(TestData))]
    public static void TestEntryPoint(int width, bool words, bool saturate)
    {
        (int Fallback, int Addend, short Left, short Right)[] inputs =
        [
            (7, 5, 0, 0),
            (7, 5, 2, 3),
            (0, 5, 2, 3),
            (7, int.MaxValue, 255, 127),
            (7, int.MinValue, 255, -128),
            (7, 0, short.MinValue, short.MinValue),
        ];

        foreach ((int fallback, int addend, short left, short right) in inputs)
        {
            long sum = addend + (words ? 2L * left * right : 4L * (byte)left * (sbyte)right);
            int active = saturate ? (int)Math.Clamp(sum, int.MinValue, int.MaxValue) : unchecked((int)sum);

            for (int pattern = 0; pattern < 4; pattern++)
            {
                Vector512<int> a = Vector512<int>.Zero;
                Vector512<int> b = Vector512<int>.Zero;
                Vector512<int> expected = Vector512<int>.Zero;

                for (int lane = 0; lane < Vector512<int>.Count; lane++)
                {
                    bool selected = pattern switch
                    {
                        0 => false,
                        1 => true,
                        2 => (lane & 1) == 0,
                        _ => (lane & 1) != 0,
                    };
                    b = b.WithElement(lane, selected ? 0 : 1);
                    expected = expected.WithElement(lane, selected ? active : fallback);
                }

                if (width == 128)
                {
                    Vector128<int> actual = words
                        ? Blend(Vector128.Create(fallback), Vector128.Create(addend), Vector128.Create(left),
                            Vector128.Create(right), a.GetLower().GetLower(), b.GetLower().GetLower(), saturate)
                        : Blend(Vector128.Create(fallback), Vector128.Create(addend), Vector128.Create((byte)left),
                            Vector128.Create((sbyte)right), a.GetLower().GetLower(), b.GetLower().GetLower(), saturate);
                    Assert.Equal(expected.GetLower().GetLower(), actual);
                }
                else if (width == 256)
                {
                    Vector256<int> actual = words
                        ? Blend(Vector256.Create(fallback), Vector256.Create(addend), Vector256.Create(left),
                            Vector256.Create(right), a.GetLower(), b.GetLower(), saturate)
                        : Blend(Vector256.Create(fallback), Vector256.Create(addend), Vector256.Create((byte)left),
                            Vector256.Create((sbyte)right), a.GetLower(), b.GetLower(), saturate);
                    Assert.Equal(expected.GetLower(), actual);
                }
                else
                {
                    Vector512<int> actual = words
                        ? Blend(Vector512.Create(fallback), Vector512.Create(addend), Vector512.Create(left),
                            Vector512.Create(right), a, b, saturate)
                        : Blend(Vector512.Create(fallback), Vector512.Create(addend), Vector512.Create((byte)left),
                            Vector512.Create((sbyte)right), a, b, saturate);
                    Assert.Equal(expected, actual);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector128<int> Blend(Vector128<int> fallback, Vector128<int> addend,
        Vector128<byte> left, Vector128<sbyte> right, Vector128<int> a, Vector128<int> b, bool saturate)
    {
        if (saturate)
        {
            return Vector128.ConditionalSelect(Vector128.Equals(a, b),
                AvxVnni.MultiplyWideningAndAddSaturate(addend, left, right), fallback);
        }

        return Vector128.ConditionalSelect(Vector128.Equals(a, b),
            AvxVnni.MultiplyWideningAndAdd(addend, left, right), fallback);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector128<int> Blend(Vector128<int> fallback, Vector128<int> addend,
        Vector128<short> left, Vector128<short> right, Vector128<int> a, Vector128<int> b, bool saturate)
    {
        if (saturate)
        {
            return Vector128.ConditionalSelect(Vector128.Equals(a, b),
                AvxVnni.MultiplyWideningAndAddSaturate(addend, left, right), fallback);
        }

        return Vector128.ConditionalSelect(Vector128.Equals(a, b),
            AvxVnni.MultiplyWideningAndAdd(addend, left, right), fallback);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector256<int> Blend(Vector256<int> fallback, Vector256<int> addend,
        Vector256<byte> left, Vector256<sbyte> right, Vector256<int> a, Vector256<int> b, bool saturate)
    {
        if (saturate)
        {
            return Vector256.ConditionalSelect(Vector256.Equals(a, b),
                AvxVnni.MultiplyWideningAndAddSaturate(addend, left, right), fallback);
        }

        return Vector256.ConditionalSelect(Vector256.Equals(a, b),
            AvxVnni.MultiplyWideningAndAdd(addend, left, right), fallback);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector256<int> Blend(Vector256<int> fallback, Vector256<int> addend,
        Vector256<short> left, Vector256<short> right, Vector256<int> a, Vector256<int> b, bool saturate)
    {
        if (saturate)
        {
            return Vector256.ConditionalSelect(Vector256.Equals(a, b),
                AvxVnni.MultiplyWideningAndAddSaturate(addend, left, right), fallback);
        }

        return Vector256.ConditionalSelect(Vector256.Equals(a, b),
            AvxVnni.MultiplyWideningAndAdd(addend, left, right), fallback);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector512<int> Blend(Vector512<int> fallback, Vector512<int> addend,
        Vector512<byte> left, Vector512<sbyte> right, Vector512<int> a, Vector512<int> b, bool saturate)
    {
        if (saturate)
        {
            return Avx512F.BlendVariable(fallback,
                AvxVnni.V512.MultiplyWideningAndAddSaturate(addend, left, right), Avx512F.CompareEqual(a, b));
        }

        return Avx512F.BlendVariable(fallback,
            AvxVnni.V512.MultiplyWideningAndAdd(addend, left, right), Avx512F.CompareEqual(a, b));
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector512<int> Blend(Vector512<int> fallback, Vector512<int> addend,
        Vector512<short> left, Vector512<short> right, Vector512<int> a, Vector512<int> b, bool saturate)
    {
        if (saturate)
        {
            return Avx512F.BlendVariable(fallback,
                AvxVnni.V512.MultiplyWideningAndAddSaturate(addend, left, right), Avx512F.CompareEqual(a, b));
        }

        return Avx512F.BlendVariable(fallback,
            AvxVnni.V512.MultiplyWideningAndAdd(addend, left, right), Avx512F.CompareEqual(a, b));
    }
}
