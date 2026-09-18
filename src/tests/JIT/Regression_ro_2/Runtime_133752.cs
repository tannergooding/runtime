// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Xunit;

public class Runtime_133752
{
    public static IEnumerable<object[]> TestData()
    {
        (uint Dividend, uint Divisor)[] inputs =
        [
            (0x7FFFFFFFu, 1u),
            (0x80000000u, 1u),
            (0x80000001u, 1u),
            (3000000000u, 1u),
            (uint.MaxValue, 1u),
            (uint.MaxValue, 2u),
            (uint.MaxValue, 3u),
            (uint.MaxValue, 0x80000000u),
            (uint.MaxValue, uint.MaxValue),
        ];

        foreach (int width in new[] { 128, 256, 512 })
        {
            foreach ((uint dividend, uint divisor) in inputs)
            {
                yield return new object[] { width, dividend, divisor };
            }
        }
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public static void TestEntryPoint(int width, uint dividend, uint divisor)
    {
        for (int mask = 0; mask < 16; mask++)
        {
            Vector128<uint> left = Vector128<uint>.Zero;
            Vector128<uint> right = Vector128<uint>.Zero;
            Vector128<uint> expected = Vector128<uint>.Zero;

            for (int lane = 0; lane < Vector128<uint>.Count; lane++)
            {
                uint numerator = (mask & (1 << lane)) != 0 ? dividend : (uint)(9 + lane);
                uint denominator = (mask & (1 << lane)) != 0 ? divisor : 3u;
                left = left.WithElement(lane, numerator);
                right = right.WithElement(lane, denominator);
                expected = expected.WithElement(lane, numerator / denominator);
            }

            if (width == 128)
            {
                Assert.Equal(expected, Divide(left, right));
                continue;
            }

            Vector256<uint> left256 = Vector256.Create(left, Vector128.Create(7u, uint.MaxValue, 3000000000u, 8u));
            Vector256<uint> right256 = Vector256.Create(right, Vector128.Create(7u, 1u, 1u, 2u));
            Vector256<uint> expected256 = Vector256.Create(expected, Vector128.Create(1u, uint.MaxValue, 3000000000u, 4u));
            if (width == 256)
            {
                Assert.Equal(expected256, Divide(left256, right256));
            }
            else
            {
                Assert.Equal(Vector512.Create(expected256, expected256),
                    Divide(Vector512.Create(left256, left256), Vector512.Create(right256, right256)));
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector128<uint> Divide(Vector128<uint> left, Vector128<uint> right)
    {
        return left / right;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector256<uint> Divide(Vector256<uint> left, Vector256<uint> right)
    {
        return left / right;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static Vector512<uint> Divide(Vector512<uint> left, Vector512<uint> right)
    {
        return left / right;
    }
}
