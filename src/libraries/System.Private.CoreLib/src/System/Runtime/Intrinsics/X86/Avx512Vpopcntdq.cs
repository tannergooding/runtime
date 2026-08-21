// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>Provides access to X86 AVX512VPOPCNTDQ hardware instructions via intrinsics.</summary>
    [Intrinsic]
    [CLSCompliant(false)]
    public abstract class Avx512Vpopcntdq : Avx512DQ
    {
        internal Avx512Vpopcntdq() { }

        /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
        /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
        /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
        public static new bool IsSupported { get => IsSupported; }

        /// <summary>Provides access to the x86 AVX512VPOPCNTDQ+VL hardware instructions via intrinsics.</summary>
        [Intrinsic]
        public new abstract class VL : Avx512DQ.VL
        {
            internal VL() { }

            /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
            /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
            /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
            public static new bool IsSupported { get => IsSupported; }

            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi32 (__m128i a)</para>
            ///   <para>  VPOPCNTD xmm1 {k1}{z}, xmm2/m128/m32bcst</para>
            /// </summary>
            public static Vector128<int> PopCount(Vector128<int> value) => PopCount(value);
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi32 (__m128i a)</para>
            ///   <para>  VPOPCNTD xmm1 {k1}{z}, xmm2/m128/m32bcst</para>
            /// </summary>
            public static Vector128<uint> PopCount(Vector128<uint> value) => PopCount(value);
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi64 (__m128i a)</para>
            ///   <para>  VPOPCNTQ xmm1 {k1}{z}, xmm2/m128/m64bcst</para>
            /// </summary>
            public static Vector128<long> PopCount(Vector128<long> value) => PopCount(value);
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi64 (__m128i a)</para>
            ///   <para>  VPOPCNTQ xmm1 {k1}{z}, xmm2/m128/m64bcst</para>
            /// </summary>
            public static Vector128<ulong> PopCount(Vector128<ulong> value) => PopCount(value);

            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi32 (__m256i a)</para>
            ///   <para>  VPOPCNTD ymm1 {k1}{z}, ymm2/m256/m32bcst</para>
            /// </summary>
            public static Vector256<int> PopCount(Vector256<int> value) => PopCount(value);
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi32 (__m256i a)</para>
            ///   <para>  VPOPCNTD ymm1 {k1}{z}, ymm2/m256/m32bcst</para>
            /// </summary>
            public static Vector256<uint> PopCount(Vector256<uint> value) => PopCount(value);
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi64 (__m256i a)</para>
            ///   <para>  VPOPCNTQ ymm1 {k1}{z}, ymm2/m256/m64bcst</para>
            /// </summary>
            public static Vector256<long> PopCount(Vector256<long> value) => PopCount(value);
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi64 (__m256i a)</para>
            ///   <para>  VPOPCNTQ ymm1 {k1}{z}, ymm2/m256/m64bcst</para>
            /// </summary>
            public static Vector256<ulong> PopCount(Vector256<ulong> value) => PopCount(value);
        }

        /// <summary>Provides access to the x86 AVX512VPOPCNTDQ hardware instructions, that are only available to 64-bit processes, via intrinsics.</summary>
        [Intrinsic]
        public new abstract class X64 : Avx512DQ.X64
        {
            internal X64() { }

            /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
            /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
            /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
            public static new bool IsSupported { get => IsSupported; }
        }

        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi32 (__m512i a)</para>
        ///   <para>  VPOPCNTD zmm1 {k1}{z}, zmm2/m512/m32bcst</para>
        /// </summary>
        public static Vector512<int> PopCount(Vector512<int> value) => PopCount(value);
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi32 (__m512i a)</para>
        ///   <para>  VPOPCNTD zmm1 {k1}{z}, zmm2/m512/m32bcst</para>
        /// </summary>
        public static Vector512<uint> PopCount(Vector512<uint> value) => PopCount(value);
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi64 (__m512i a)</para>
        ///   <para>  VPOPCNTQ zmm1 {k1}{z}, zmm2/m512/m64bcst</para>
        /// </summary>
        public static Vector512<long> PopCount(Vector512<long> value) => PopCount(value);
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi64 (__m512i a)</para>
        ///   <para>  VPOPCNTQ zmm1 {k1}{z}, zmm2/m512/m64bcst</para>
        /// </summary>
        public static Vector512<ulong> PopCount(Vector512<ulong> value) => PopCount(value);
    }
}
