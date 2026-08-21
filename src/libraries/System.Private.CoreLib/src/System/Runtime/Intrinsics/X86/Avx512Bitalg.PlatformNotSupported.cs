// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>Provides access to X86 AVX512BITALG hardware instructions via intrinsics</summary>
    [CLSCompliant(false)]
    public abstract class Avx512Bitalg : Avx512BW
    {
        internal Avx512Bitalg() { }

        /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
        /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
        /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
        public static new bool IsSupported { [Intrinsic] get { return false; } }

        /// <summary>Provides access to the x86 AVX512BITALG+VL hardware instructions via intrinsics.</summary>
        public new abstract class VL : Avx512BW.VL
        {
            internal VL() { }

            /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
            /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
            /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
            public static new bool IsSupported { [Intrinsic] get { return false; } }

            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi8 (__m128i a)</para>
            ///   <para>  VPOPCNTB xmm1 {k1}{z}, xmm2/m128</para>
            /// </summary>
            public static Vector128<sbyte> PopCount(Vector128<sbyte> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi8 (__m128i a)</para>
            ///   <para>  VPOPCNTB xmm1 {k1}{z}, xmm2/m128</para>
            /// </summary>
            public static Vector128<byte> PopCount(Vector128<byte> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi16 (__m128i a)</para>
            ///   <para>  VPOPCNTW xmm1 {k1}{z}, xmm2/m128</para>
            /// </summary>
            public static Vector128<short> PopCount(Vector128<short> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m128i _mm_popcnt_epi16 (__m128i a)</para>
            ///   <para>  VPOPCNTW xmm1 {k1}{z}, xmm2/m128</para>
            /// </summary>
            public static Vector128<ushort> PopCount(Vector128<ushort> value) { throw new PlatformNotSupportedException(); }

            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi8 (__m256i a)</para>
            ///   <para>  VPOPCNTB ymm1 {k1}{z}, ymm2/m256</para>
            /// </summary>
            public static Vector256<sbyte> PopCount(Vector256<sbyte> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi8 (__m256i a)</para>
            ///   <para>  VPOPCNTB ymm1 {k1}{z}, ymm2/m256</para>
            /// </summary>
            public static Vector256<byte> PopCount(Vector256<byte> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi16 (__m256i a)</para>
            ///   <para>  VPOPCNTW ymm1 {k1}{z}, ymm2/m256</para>
            /// </summary>
            public static Vector256<short> PopCount(Vector256<short> value) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__m256i _mm256_popcnt_epi16 (__m256i a)</para>
            ///   <para>  VPOPCNTW ymm1 {k1}{z}, ymm2/m256</para>
            /// </summary>
            public static Vector256<ushort> PopCount(Vector256<ushort> value) { throw new PlatformNotSupportedException(); }

            /// <summary>
            ///   <para>__mmask16 _mm_bitshuffle_epi64_mask (__m128i b, __m128i c)</para>
            ///   <para>  VPSHUFBITQMB k1, xmm2, xmm3/m128</para>
            /// </summary>
            public static Vector128<byte> ShuffleBits(Vector128<ulong> value, Vector128<byte> control) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__mmask16 _mm_bitshuffle_epi64_mask (__m128i b, __m128i c)</para>
            ///   <para>  VPSHUFBITQMB k1, xmm2, xmm3/m128</para>
            /// </summary>
            public static Vector128<sbyte> ShuffleBits(Vector128<long> value, Vector128<sbyte> control) { throw new PlatformNotSupportedException(); }

            /// <summary>
            ///   <para>__mmask32 _mm256_bitshuffle_epi64_mask (__m256i b, __m256i c)</para>
            ///   <para>  VPSHUFBITQMB k1, ymm2, ymm3/m256</para>
            /// </summary>
            public static Vector256<byte> ShuffleBits(Vector256<ulong> value, Vector256<byte> control) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__mmask32 _mm256_bitshuffle_epi64_mask (__m256i b, __m256i c)</para>
            ///   <para>  VPSHUFBITQMB k1, ymm2, ymm3/m256</para>
            /// </summary>
            public static Vector256<sbyte> ShuffleBits(Vector256<long> value, Vector256<sbyte> control) { throw new PlatformNotSupportedException(); }

            /// <summary>
            ///   <para>__mmask16 _mm_mask_bitshuffle_epi64_mask (__mmask16 k, __m128i b, __m128i c)</para>
            ///   <para>  VPSHUFBITQMB k1 {k2}, xmm2, xmm3/m128</para>
            /// </summary>
            public static Vector128<byte> MaskShuffleBits(Vector128<byte> mask, Vector128<ulong> value, Vector128<byte> control) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__mmask16 _mm_mask_bitshuffle_epi64_mask (__mmask16 k, __m128i b, __m128i c)</para>
            ///   <para>  VPSHUFBITQMB k1 {k2}, xmm2, xmm3/m128</para>
            /// </summary>
            public static Vector128<sbyte> MaskShuffleBits(Vector128<sbyte> mask, Vector128<long> value, Vector128<sbyte> control) { throw new PlatformNotSupportedException(); }

            /// <summary>
            ///   <para>__mmask32 _mm256_mask_bitshuffle_epi64_mask (__mmask32 k, __m256i b, __m256i c)</para>
            ///   <para>  VPSHUFBITQMB k1 {k2}, ymm2, ymm3/m256</para>
            /// </summary>
            public static Vector256<byte> MaskShuffleBits(Vector256<byte> mask, Vector256<ulong> value, Vector256<byte> control) { throw new PlatformNotSupportedException(); }
            /// <summary>
            ///   <para>__mmask32 _mm256_mask_bitshuffle_epi64_mask (__mmask32 k, __m256i b, __m256i c)</para>
            ///   <para>  VPSHUFBITQMB k1 {k2}, ymm2, ymm3/m256</para>
            /// </summary>
            public static Vector256<sbyte> MaskShuffleBits(Vector256<sbyte> mask, Vector256<long> value, Vector256<sbyte> control) { throw new PlatformNotSupportedException(); }
        }

        /// <summary>Provides access to the x86 AVX512BITALG hardware instructions, that are only available to 64-bit processes, via intrinsics.</summary>
        public new abstract class X64 : Avx512BW.X64
        {
            internal X64() { }

            /// <summary>Gets a value that indicates whether the APIs in this class are supported.</summary>
            /// <value><see langword="true" /> if the APIs are supported; otherwise, <see langword="false" />.</value>
            /// <remarks>A value of <see langword="false" /> indicates that the APIs will throw <see cref="PlatformNotSupportedException" />.</remarks>
            public static new bool IsSupported { [Intrinsic] get { return false; } }
        }

        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi8 (__m512i a)</para>
        ///   <para>  VPOPCNTB zmm1 {k1}{z}, zmm2/m512</para>
        /// </summary>
        public static Vector512<sbyte> PopCount(Vector512<sbyte> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi8 (__m512i a)</para>
        ///   <para>  VPOPCNTB zmm1 {k1}{z}, zmm2/m512</para>
        /// </summary>
        public static Vector512<byte> PopCount(Vector512<byte> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi16 (__m512i a)</para>
        ///   <para>  VPOPCNTW zmm1 {k1}{z}, zmm2/m512</para>
        /// </summary>
        public static Vector512<short> PopCount(Vector512<short> value) { throw new PlatformNotSupportedException(); }
        /// <summary>
        ///   <para>__m512i _mm512_popcnt_epi16 (__m512i a)</para>
        ///   <para>  VPOPCNTW zmm1 {k1}{z}, zmm2/m512</para>
        /// </summary>
        public static Vector512<ushort> PopCount(Vector512<ushort> value) { throw new PlatformNotSupportedException(); }

        /// <summary>
        ///   <para>__mmask64 _mm512_bitshuffle_epi64_mask (__m512i b, __m512i c)</para>
        ///   <para>  VPSHUFBITQMB k1, zmm2, zmm3/m512</para>
        /// </summary>
        public static Vector512<byte> ShuffleBits(Vector512<ulong> value, Vector512<byte> control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        ///   <para>__mmask64 _mm512_bitshuffle_epi64_mask (__m512i b, __m512i c)</para>
        ///   <para>  VPSHUFBITQMB k1, zmm2, zmm3/m512</para>
        /// </summary>
        public static Vector512<sbyte> ShuffleBits(Vector512<long> value, Vector512<sbyte> control) { throw new PlatformNotSupportedException(); }

        /// <summary>
        ///   <para>__mmask64 _mm512_mask_bitshuffle_epi64_mask (__mmask64 k, __m512i b, __m512i c)</para>
        ///   <para>  VPSHUFBITQMB k1 {k2}, zmm2, zmm3/m512</para>
        /// </summary>
        public static Vector512<byte> MaskShuffleBits(Vector512<byte> mask, Vector512<ulong> value, Vector512<byte> control) { throw new PlatformNotSupportedException(); }
        /// <summary>
        ///   <para>__mmask64 _mm512_mask_bitshuffle_epi64_mask (__mmask64 k, __m512i b, __m512i c)</para>
        ///   <para>  VPSHUFBITQMB k1 {k2}, zmm2, zmm3/m512</para>
        /// </summary>
        public static Vector512<sbyte> MaskShuffleBits(Vector512<sbyte> mask, Vector512<long> value, Vector512<sbyte> control) { throw new PlatformNotSupportedException(); }
    }
}
