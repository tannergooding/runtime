// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Threading;
using Arm = System.Runtime.Intrinsics.Arm;
using X86 = System.Runtime.Intrinsics.X86;

public static class GuardedInstructionSets
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool Supported() => X86.Aes.IsSupported || Arm.Aes.IsSupported;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<byte> Transform(Vector128<byte> value)
    {
        if (X86.Aes.IsSupported)
        {
            return X86.Aes.Encrypt(value, Vector128<byte>.Zero);
        }
        if (Arm.Aes.IsSupported)
        {
            return Arm.Aes.Encrypt(value, Vector128<byte>.Zero);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<byte> IndirectTransform(Vector128<byte> value)
    {
        if (X86.Aes.IsSupported)
        {
            Func<Vector128<byte>, Vector128<byte>, Vector128<byte>> encrypt = X86.Aes.Encrypt;
            return encrypt(value, Vector128<byte>.Zero);
        }
        if (Arm.Aes.IsSupported)
        {
            Func<Vector128<byte>, Vector128<byte>, Vector128<byte>> encrypt = Arm.Aes.Encrypt;
            return encrypt(value, Vector128<byte>.Zero);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<byte> UnguardedTransform(Vector128<byte> value) => X86.Aes.Encrypt(value, value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<byte> UnguardedIndirectTransform(Vector128<byte> value)
    {
        Func<Vector128<byte>, Vector128<byte>, Vector128<byte>> encrypt = X86.Aes.Encrypt;

        return encrypt(value, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Vector128<ulong> RemappedAbs(Vector128<long> value) => X86.Avx10v1.Abs(value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ulong Crc64(ulong value)
    {
        if (X86.Sse42.X64.IsSupported)
        {
            return X86.Sse42.X64.Crc32(0, value);
        }
        if (Arm.Crc32.Arm64.IsSupported)
        {
            return Arm.Crc32.Arm64.ComputeCrc32C(0, value);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int VolatileLoad(ref int value) => Volatile.Read(ref value);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void VolatileStore(ref int value, int replacement) => Volatile.Write(ref value, replacement);
}
