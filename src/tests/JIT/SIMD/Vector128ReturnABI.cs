// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Validates that methods returning the intrinsic vector types (Vector128<T>) are
// returned correctly for the managed calling convention. On Windows x64 these are
// returned in XMM0 (matching the native __m128 ABI) rather than via a return buffer;
// on other targets they use the platform's existing register-based return ABI. The
// test exercises direct JIT-compiled calls, reflection invoke, and delegate dynamic
// invoke so both the JIT call-site handling and the VM invoke/CallDescrWorker paths
// are covered.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Xunit;

public static class Vector128ReturnABI
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<float> MakeSingle(float a, float b, float c, float d)
        => Vector128.Create(a, b, c, d);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<double> MakeDouble(double a, double b)
        => Vector128.Create(a, b);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<int> MakeInt32(int a, int b, int c, int d)
        => Vector128.Create(a, b, c, d);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<long> MakeInt64(long a, long b)
        => Vector128.Create(a, b);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<byte> MakeByte(byte a)
        => Vector128.Create(a);

    // Round-trips a Vector128<T> argument through a return value to exercise both the
    // argument-passing and register-return paths for the same call.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<float> AddOne(Vector128<float> v)
        => v + Vector128.Create(1.0f);

    private delegate Vector128<float> MakeSingleDelegate(float a, float b, float c, float d);

    [Fact]
    public static int TestEntryPoint()
    {
        // Direct managed calls.
        Assert.Equal(Vector128.Create(1.0f, 2.0f, 3.0f, 4.0f), MakeSingle(1, 2, 3, 4));
        Assert.Equal(Vector128.Create(1.0, 2.0), MakeDouble(1, 2));
        Assert.Equal(Vector128.Create(1, 2, 3, 4), MakeInt32(1, 2, 3, 4));
        Assert.Equal(Vector128.Create(1L, 2L), MakeInt64(1, 2));
        Assert.Equal(Vector128.Create((byte)7), MakeByte(7));

        // Argument + return round-trip.
        Assert.Equal(Vector128.Create(2.0f, 3.0f, 4.0f, 5.0f), AddOne(Vector128.Create(1.0f, 2.0f, 3.0f, 4.0f)));

        // Reflection invoke (exercises the VM invoke stub / CallDescrWorker return handling).
        MethodInfo mi = typeof(Vector128ReturnABI).GetMethod(nameof(MakeSingle))!;
        object? boxed = mi.Invoke(null, new object[] { 5.0f, 6.0f, 7.0f, 8.0f });
        Assert.Equal(Vector128.Create(5.0f, 6.0f, 7.0f, 8.0f), (Vector128<float>)boxed!);

        // Delegate dynamic invoke.
        var del = (MakeSingleDelegate)Delegate.CreateDelegate(typeof(MakeSingleDelegate), mi);
        object? dynResult = del.DynamicInvoke(9.0f, 10.0f, 11.0f, 12.0f);
        Assert.Equal(Vector128.Create(9.0f, 10.0f, 11.0f, 12.0f), (Vector128<float>)dynResult!);

        // Direct delegate call.
        Assert.Equal(Vector128.Create(13.0f, 14.0f, 15.0f, 16.0f), del(13, 14, 15, 16));

        return 100;
    }
}
