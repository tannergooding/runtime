// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        internal static
#if !DEBUG
        // Mutable for unit testing...
        readonly
#endif
        int StackAllocThreshold = 1024 / Unsafe.SizeOf<nuint>();

        public static int Compare(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right)
        {
            Debug.Assert((left.Length <= right.Length) || left[right.Length..].ContainsAnyExcept(0u));
            Debug.Assert((left.Length >= right.Length) || right[left.Length..].ContainsAnyExcept(0u));

            if (left.Length != right.Length)
            {
                return left.Length < right.Length ? -1 : 1;
            }

            // TODO: This could use a CommonSuffixLength algorithm that is vectorized
            int iv = left.Length;
            while (--iv >= 0 && left[iv] == right[iv]) ;

            if (iv < 0)
            {
                return 0;
            }
            return left[iv] < right[iv] ? -1 : 1;
        }

        private static int ActualLength(ReadOnlySpan<nuint> value)
        {
            // Since we're reusing memory here, the actual length
            // of a given value may be less then the array's length
            return value.LastIndexOfAnyExcept(0u) + 1;
        }

        private static int Reduce(Span<nuint> bits, ReadOnlySpan<nuint> modulus)
        {
            // Executes a modulo operation using the divide operation.

            if (bits.Length >= modulus.Length)
            {
                if (Environment.Is64BitProcess)
                {
                    Divide<UInt128>(bits, modulus, default);
                }
                else
                {
                    Divide<ulong>(bits, modulus, default);
                }
                return ActualLength(bits[..modulus.Length]);
            }
            return bits.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigInteger Create<TOverflow>(TOverflow value)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            if (Environment.Is64BitProcess)
            {
                return Unsafe.BitCast<TOverflow, Int128>(value);
            }
            else
            {
                return Unsafe.BitCast<TOverflow, long>(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOverflow Create<TOverflow>(nuint uHi, nuint uLo)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            if (Environment.Is64BitProcess)
            {
                return Unsafe.BitCast<UInt128, TOverflow>(new UInt128(uHi, uLo));
            }
            else
            {
                return Unsafe.BitCast<ulong, TOverflow>(((ulong)uHi << 32) | uLo);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOverflow Create<TOverflow>(TOverflow uHi, nuint uLo)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            return Create<TOverflow>(Narrow(uHi), uLo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOverflow Create<TOverflow>(nuint uHi, TOverflow uLo)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            return Create<TOverflow>(uHi, Narrow(uLo));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint Narrow<TOverflow>(TOverflow value)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            if (Environment.Is64BitProcess)
            {
                return (nuint)Unsafe.BitCast<TOverflow, UInt128>(value);
            }
            else
            {
                return (nuint)Unsafe.BitCast<TOverflow, ulong>(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOverflow Widen<TOverflow>(nint value)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            if (Environment.Is64BitProcess)
            {
                return Unsafe.BitCast<Int128, TOverflow>(value);
            }
            else
            {
                return Unsafe.BitCast<long, TOverflow>(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TOverflow Widen<TOverflow>(nuint value)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            if (Environment.Is64BitProcess)
            {
                return Unsafe.BitCast<UInt128, TOverflow>(value);
            }
            else
            {
                return Unsafe.BitCast<ulong, TOverflow>(value);
            }
        }
    }
}
