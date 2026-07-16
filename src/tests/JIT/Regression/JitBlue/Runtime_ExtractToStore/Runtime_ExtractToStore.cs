// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Validates that extracting a 128/256-bit lane directly into a memory store
// (which lowering contains into a single vextract[if]128/x4 [mem], ymm/zmm, imm)
// produces the correct stored bytes for the folded and non-folded (imm == 0) paths.

namespace Runtime_ExtractToStore;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xunit;

public class Runtime_ExtractToStore
{
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreLane_Int(Vector256<int> v, byte imm, ref Vector128<int> dst)
    {
        dst = imm == 0 ? Avx2.ExtractVector128(v, 0) : Avx2.ExtractVector128(v, 1);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreLane_Float(Vector256<float> v, byte imm, ref Vector128<float> dst)
    {
        dst = imm == 0 ? Avx.ExtractVector128(v, 0) : Avx.ExtractVector128(v, 1);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreGetUpper_Long(Vector256<long> v, ref Vector128<long> dst)
    {
        dst = v.GetUpper();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreExtract256_Long(Vector512<long> v, byte imm, ref Vector256<long> dst)
    {
        dst = imm == 0 ? Avx512F.ExtractVector256(v, 0) : Avx512F.ExtractVector256(v, 1);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreExtract256_Int_GetUpper(Vector512<int> v, ref Vector256<int> dst)
    {
        dst = v.GetUpper();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreExtract128FromZmm_Int(Vector512<int> v, byte lane, ref Vector128<int> dst)
    {
        switch (lane)
        {
            case 0: dst = Avx512F.ExtractVector128(v, 0); break;
            case 1: dst = Avx512F.ExtractVector128(v, 1); break;
            case 2: dst = Avx512F.ExtractVector128(v, 2); break;
            default: dst = Avx512F.ExtractVector128(v, 3); break;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static void StoreExtract128FromZmm_Double(Vector512<double> v, byte lane, ref Vector128<double> dst)
    {
        switch (lane)
        {
            case 0: dst = Avx512F.ExtractVector128(v, 0); break;
            case 1: dst = Avx512F.ExtractVector128(v, 1); break;
            case 2: dst = Avx512F.ExtractVector128(v, 2); break;
            default: dst = Avx512F.ExtractVector128(v, 3); break;
        }
    }

    [Fact]
    public static void TestEntryPoint()
    {
        if (Avx2.IsSupported)
        {
            Vector256<int> vi = Vector256.Create(10, 11, 12, 13, 14, 15, 16, 17);

            Vector128<int> di = default;
            StoreLane_Int(vi, 1, ref di);
            Assert.Equal(Vector128.Create(14, 15, 16, 17), di);

            StoreLane_Int(vi, 0, ref di);
            Assert.Equal(Vector128.Create(10, 11, 12, 13), di);

            Vector256<float> vf = Vector256.Create(1f, 2, 3, 4, 5, 6, 7, 8);
            Vector128<float> df = default;
            StoreLane_Float(vf, 1, ref df);
            Assert.Equal(Vector128.Create(5f, 6, 7, 8), df);

            StoreLane_Float(vf, 0, ref df);
            Assert.Equal(Vector128.Create(1f, 2, 3, 4), df);

            Vector256<long> vl = Vector256.Create(100L, 101, 102, 103);
            Vector128<long> dl = default;
            StoreGetUpper_Long(vl, ref dl);
            Assert.Equal(Vector128.Create(102L, 103), dl);
        }

        if (Avx512F.IsSupported)
        {
            Vector512<long> vl = Vector512.Create(0L, 1, 2, 3, 4, 5, 6, 7);
            Vector256<long> dl = default;

            StoreExtract256_Long(vl, 1, ref dl);
            Assert.Equal(Vector256.Create(4L, 5, 6, 7), dl);

            StoreExtract256_Long(vl, 0, ref dl);
            Assert.Equal(Vector256.Create(0L, 1, 2, 3), dl);

            Vector512<int> vi = Vector512.Create(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
            Vector256<int> di = default;
            StoreExtract256_Int_GetUpper(vi, ref di);
            Assert.Equal(Vector256.Create(8, 9, 10, 11, 12, 13, 14, 15), di);

            Vector128<int> d128 = default;
            for (byte lane = 0; lane < 4; lane++)
            {
                StoreExtract128FromZmm_Int(vi, lane, ref d128);
                int b = lane * 4;
                Assert.Equal(Vector128.Create(b, b + 1, b + 2, b + 3), d128);
            }

            Vector512<double> vd = Vector512.Create(0d, 1, 2, 3, 4, 5, 6, 7);
            Vector128<double> dd = default;
            for (byte lane = 0; lane < 4; lane++)
            {
                StoreExtract128FromZmm_Double(vd, lane, ref dd);
                double b = lane * 2;
                Assert.Equal(Vector128.Create(b, b + 1), dd);
            }
        }
    }
}
