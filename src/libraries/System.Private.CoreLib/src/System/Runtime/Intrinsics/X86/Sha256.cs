// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to the x86 SHA256 hardware instructions via intrinsics
    /// </summary>
    [Intrinsic]
    public abstract partial class Sha256 : X86Base
    {
        internal Sha256() { }

        public static new bool IsSupported { get => IsSupported; }

        [Intrinsic]
        public new abstract class X64 : X86Base.X64
        {
            internal X64() { }

            public static new bool IsSupported { get => IsSupported; }
        }

        /// <summary>
        /// __m128i _mm_sha256msg1_epu32 (__m128i a, __m128i b)
        /// SHA256MSG1 xmm, xmm/m128
        /// </summary>
        public static Vector128<byte> MessageSchedule1(Vector128<byte> a, Vector128<byte> b) => MessageSchedule1(a, b);

        /// <summary>
        /// __m128i _mm_sha256msg2_epu32 (__m128i a, __m128i b)
        /// SHA256MSG2 xmm, xmm/m128
        /// </summary>
        public static Vector128<byte> MessageSchedule2(Vector128<byte> a, Vector128<byte> b) => MessageSchedule2(a, b);

        /// <summary>
        /// __m128i _mm_sha256rnds2_epu32 (__m128i a, __m128i b, __m128i k)
        /// SHA256RNDS2 xmm, xmm/m128, &lt;XMM0&gt;
        /// </summary>
        public static Vector128<byte> TwoRounds(Vector128<byte> a, Vector128<byte> b, Vector128<byte> k) => TwoRounds(a, b, k);
    }
}
