// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Intrinsics.X86
{
    /// <summary>
    /// This class provides access to the x86 SHA1 hardware instructions via intrinsics
    /// </summary>
    public abstract partial class Sha1 : X86Base
    {
        internal Sha1() { }

        public static new bool IsSupported { [Intrinsic] get { return false; } }

        public new abstract class X64 : X86Base.X64
        {
            internal X64() { }

            public static new bool IsSupported { [Intrinsic] get { return false; } }
        }

        /// <summary>
        /// __m128i _mm_sha1rnds4_epu32 (__m128i a, __m128i b, const int func)
        /// SHA1RNDS4 xmm, xmm/m128, imm8
        /// </summary>
        public static Vector128<byte> FourRounds(Vector128<byte> a, Vector128<byte> b, byte func) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm_sha1msg1_epu32 (__m128i a, __m128i b)
        /// SHA1MSG1 xmm, xmm/m128
        /// </summary>
        public static Vector128<byte> MessageSchedule1(Vector128<byte> a, Vector128<byte> b) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm_sha1msg2_epu32 (__m128i a, __m128i b)
        /// SHA1MSG2 xmm, xmm/m128
        /// </summary>
        public static Vector128<byte> MessageSchedule2(Vector128<byte> a, Vector128<byte> b) { throw new PlatformNotSupportedException(); }

        /// <summary>
        /// __m128i _mm_sha1nexte_epu32 (__m128i a, __m128i b)
        /// SHA1NEXTE xmm, xmm/m128
        /// </summary>
        public static Vector128<byte> NextE(Vector128<byte> a, Vector128<byte> b) { throw new PlatformNotSupportedException(); }
    }
}
