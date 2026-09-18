// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Xunit;

public class BlockStoreVzeroupper
{
    [Fact]
    public static void TestEntryPoint()
    {
        byte[] source = new byte[64];
        byte[] destination = new byte[64];
        source.AsSpan().Fill(42);

        Memmove16(source, destination);
        Assert.Equal(source.AsSpan(0, 16).ToArray(), destination.AsSpan(0, 16).ToArray());

        int expected = Vector256.IsHardwareAccelerated ? 1 : -1;
        Assert.Equal(expected, Memmove40(source, destination));
        if (Vector256.IsHardwareAccelerated)
        {
            Assert.Equal(source.AsSpan(0, 40).ToArray(), destination.AsSpan(0, 40).ToArray());
        }

        Assert.Equal(expected, Memmove64(source, destination));
        if (Vector256.IsHardwareAccelerated)
        {
            Assert.Equal(source, destination);
        }

        Assert.Equal(expected, Init32(ref destination[0]));
        if (Vector256.IsHardwareAccelerated)
        {
            Assert.All(destination.AsSpan(0, 32).ToArray(), value => Assert.Equal((byte)0, value));
            Assert.All(destination.AsSpan(32).ToArray(), value => Assert.Equal((byte)42, value));
        }

        destination.AsSpan().Fill(42);
        Assert.Equal(expected, Init64(ref destination[0]));
        if (Vector256.IsHardwareAccelerated)
        {
            Assert.All(destination, value => Assert.Equal((byte)0, value));
        }

        FragmentedStruct fragmented = new FragmentedStruct { A = 1, B = 2, Reference = new object(), C = 3, D = 4 };
        InitFragmented(ref fragmented);
        Assert.Equal(default(FragmentedStruct), fragmented);

        WideStruct wide = new WideStruct { A = 1, B = 2, C = 3, D = 4, Reference = new object() };
        Assert.Equal(expected, InitWide(ref wide));
        if (Vector256.IsHardwareAccelerated)
        {
            Assert.Equal(default(WideStruct), wide);
        }

        MemmoveThenPInvoke(source, destination);
        if (OperatingSystem.IsWindows() && Vector256.IsHardwareAccelerated)
        {
            Assert.Equal(source.AsSpan(0, 40).ToArray(), destination.AsSpan(0, 40).ToArray());
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Memmove16(byte[] source, Span<byte> destination)
    {
        // X64-NOT: vzeroupper
        // X64: ret
        source.AsSpan(0, 16).CopyTo(destination);
    }

    // The -1 return identifies targets without 256-bit SIMD. Unlike a zero return,
    // it cannot be confused with clearing a register during block initialization.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Memmove40(byte[] source, Span<byte> destination)
    {
        // X64: {{vzeroupper|mov +eax, -1}}
        // X64: ret
        if (Vector256.IsHardwareAccelerated)
        {
            source.AsSpan(0, 40).CopyTo(destination);
            return 1;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Memmove64(byte[] source, Span<byte> destination)
    {
        // X64: {{vzeroupper|mov +eax, -1}}
        // X64: ret
        if (Vector256.IsHardwareAccelerated)
        {
            source.AsSpan(0, 64).CopyTo(destination);
            return 1;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Init32(ref byte destination)
    {
        // X64: {{vzeroupper|mov +eax, -1}}
        // X64: ret
        if (Vector256.IsHardwareAccelerated)
        {
            Unsafe.InitBlockUnaligned(ref destination, 0, 32);
            return 1;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Init64(ref byte destination)
    {
        // X64: {{vzeroupper|mov +eax, -1}}
        // X64: ret
        if (Vector256.IsHardwareAccelerated)
        {
            Unsafe.InitBlockUnaligned(ref destination, 0, 64);
            return 1;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitFragmented(ref FragmentedStruct destination)
    {
        // X64-NOT: vzeroupper
        // X64: ret
        destination = default;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int InitWide(ref WideStruct destination)
    {
        // X64: {{vzeroupper|mov +eax, -1}}
        // X64: ret
        if (Vector256.IsHardwareAccelerated)
        {
            destination = default;
            return 1;
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static uint MemmoveThenPInvoke(byte[] source, Span<byte> destination)
    {
        // X64-WINDOWS-NOT: vzeroupper
        // X64-WINDOWS: {{vmovdqu.*ymm|xor +eax, eax}}
        // X64-WINDOWS-NOT: call
        // X64-WINDOWS: {{vzeroupper|ret}}
        if (OperatingSystem.IsWindows() && Vector256.IsHardwareAccelerated)
        {
            source.AsSpan(0, 40).CopyTo(destination);
            return GetTickCount();
        }

        return 0;
    }

    [DllImport("kernel32.dll")]
    private static extern uint GetTickCount();

    [StructLayout(LayoutKind.Explicit)]
    private struct FragmentedStruct
    {
        [FieldOffset(0)]
        public long A;
        [FieldOffset(8)]
        public long B;
        [FieldOffset(16)]
        public object Reference;
        [FieldOffset(24)]
        public long C;
        [FieldOffset(32)]
        public long D;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WideStruct
    {
        public long A, B, C, D;
        public object Reference;
    }
}
