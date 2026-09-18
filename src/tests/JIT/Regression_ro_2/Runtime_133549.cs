// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;

public static class Runtime_133549
{
    [ConditionalFact(typeof(Sse), nameof(Sse.IsSupported))]
    public static void TestEntryPoint()
    {
        Assert.Equal(0x7F800001u, Mask(Vector128.Create(uint.MaxValue).AsSingle()));
    }

    // Keep the mask separate from the assertion so CSE does not prevent embedded broadcasting.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static uint Mask(Vector128<float> value)
    {
        return Sse.And(value, Vector128.Create(0x7F800001u).AsSingle()).AsUInt32().GetElement(0);
    }
}
