// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        public static void Divide<TOverflow>(ReadOnlySpan<nuint> left, nuint right, Span<nuint> quotient, out nuint remainder)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 1);
            Debug.Assert(quotient.Length == left.Length);

            // Executes the division for one big and one BitsPerElement-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the BitsPerElement-bit integer, since it's a single element.

            TOverflow carry = TOverflow.Zero;

            for (int i = left.Length - 1; i >= 0; i--)
            {
                TOverflow value = Create(carry, left[i]);
                TOverflow digit = value / Widen<TOverflow>(right);
                quotient[i] = Narrow(digit);
                carry = value - (digit * Widen<TOverflow>(right));
            }
            remainder = Narrow(carry);
        }

        public static void Divide<TOverflow>(ReadOnlySpan<nuint> left, nuint right, Span<nuint> quotient)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 1);
            Debug.Assert(quotient.Length == left.Length);

            // Same as above, but only computing the quotient.

            TOverflow carry = TOverflow.Zero;

            for (int i = left.Length - 1; i >= 0; i--)
            {
                TOverflow value = Create(carry, left[i]);
                TOverflow digit = value / Widen<TOverflow>(right);
                quotient[i] = Narrow(digit);
                carry = value - (digit * Widen<TOverflow>(right));
            }
        }

        public static nuint Remainder<TOverflow>(ReadOnlySpan<nuint> left, nuint right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 1);

            // Same as above, but only computing the remainder.
            TOverflow carry = TOverflow.Zero;

            for (int i = left.Length - 1; i >= 0; i--)
            {
                TOverflow value = Create(carry, left[i]);
                carry = value % Widen<TOverflow>(right);
            }

            return Narrow(carry);
        }

        public static void Divide(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> quotient, Span<nuint> remainder)
        {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(quotient.Length == (left.Length - right.Length + 1));
            Debug.Assert(remainder.Length == left.Length);

            left.CopyTo(remainder);

            if (Environment.Is64BitProcess)
            {
                Divide<UInt128>(remainder, right, quotient);
            }
            else
            {
                Divide<ulong>(remainder, right, quotient);
            }
        }

        public static void Divide(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> quotient)
        {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(quotient.Length == (left.Length - right.Length + 1));

            // Same as above, but only returning the quotient.

            nuint[]? leftCopyFromPool = null;

            // NOTE: left will get overwritten, we need a local copy
            // However, mutated left is not used afterwards, so use array pooling or stack alloc
            Span<nuint> leftCopy = (left.Length <= StackAllocThreshold
                                     ? stackalloc nuint[StackAllocThreshold]
                                     : leftCopyFromPool = ArrayPool<nuint>.Shared.Rent(left.Length))[..left.Length];
            left.CopyTo(leftCopy);

            if (Environment.Is64BitProcess)
            {
                Divide<UInt128>(leftCopy, right, quotient);
            }
            else
            {
                Divide<ulong>(leftCopy, right, quotient);
            }

            if (leftCopyFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(leftCopyFromPool);
            }
        }

        public static void Remainder(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> remainder)
        {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(remainder.Length >= left.Length);

            // Same as above, but only returning the remainder.

            left.CopyTo(remainder);

            if (Environment.Is64BitProcess)
            {
                Divide<UInt128>(remainder, right, default);
            }
            else
            {
                Divide<ulong>(remainder, right, default);
            }
        }

        private static void Divide<TOverflow>(Span<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert((bits.Length == (left.Length - right.Length + 1)) || (bits.Length == 0));

            // Executes the "grammar-school" algorithm for computing q = a / b.
            // Before calculating q_i, we get more bits into the highest bit
            // block of the divisor. Thus, guessing digits of the quotient
            // will be more precise. Additionally we'll get r = a % b.

            nuint divHi = right[^1];
            nuint divLo = (right.Length > 1) ? right[^2] : 0;

            // We measure the leading zeros of the divisor
            int shift = BitOperations.LeadingZeroCount(divHi);
            int backShift = BigInteger.BitsPerElement - shift;

            // And, we make sure the most significant bit is set
            if (shift > 0)
            {
                nuint divNx = (right.Length > 2) ? right[^3] : 0;

                divHi = (divHi << shift) | (divLo >> backShift);
                divLo = (divLo << shift) | (divNx >> backShift);
            }

            // Then, we divide all of the bits as we would do it using
            // pen and paper: guessing the next digit, subtracting, ...
            for (int i = left.Length; i >= right.Length; i--)
            {
                int n = i - right.Length;
                nuint t = (nuint)i < (nuint)left.Length ? left[i] : 0;

                TOverflow valHi = Create<TOverflow>(t, left[i - 1]);
                nuint valLo = i > 1 ? left[i - 2] : 0;

                // We shifted the divisor, we shift the dividend too
                if (shift > 0)
                {
                    nuint valNx = i > 2 ? left[i - 3] : 0;

                    valHi = (valHi << shift) | Widen<TOverflow>(valLo >> backShift);
                    valLo = (valLo << shift) | (valNx >> backShift);
                }

                // First guess for the current digit of the quotient,
                // which naturally must have only BitsPerElement bits...
                TOverflow digit = valHi / Widen<TOverflow>(divHi);

                if (digit > Widen<TOverflow>(nuint.MaxValue))
                {
                    digit = Widen<TOverflow>(nuint.MaxValue);
                }

                // Our first guess may be a little bit to big
                while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
                {
                    --digit;
                }

                if (digit > TOverflow.Zero)
                {
                    // Now it's time to subtract our current quotient
                    nuint carry = SubtractDivisor(left[n..], right, digit);

                    if (carry != t)
                    {
                        Debug.Assert(carry == (t + 1));

                        // Our guess was still exactly one too high
                        carry = AddDivisor<TOverflow>(left[n..], right);
                        --digit;

                        Debug.Assert(carry == 1);
                    }
                }

                // We have the digit!
                if ((uint)n < (uint)bits.Length)
                {
                    bits[n] = Narrow(digit);
                }

                if ((uint)i < (uint)left.Length)
                {
                    left[i] = 0;
                }
            }
        }

        private static nuint AddDivisor<TOverflow>(Span<nuint> left, ReadOnlySpan<nuint> right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= right.Length);

            // Repairs the dividend, if the last subtract was too much

            TOverflow carry = TOverflow.Zero;

            for (int i = 0; i < right.Length; i++)
            {
                ref nuint leftElement = ref left[i];
                TOverflow digit = Widen<TOverflow>(leftElement) + carry + Widen<TOverflow>(right[i]);

                leftElement = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            return Narrow(carry);
        }

        private static nuint SubtractDivisor<TOverflow>(Span<nuint> left, ReadOnlySpan<nuint> right, TOverflow q)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(q <= Widen<TOverflow>(nuint.MaxValue));

            // Combines a subtract and a multiply operation, which is naturally
            // more efficient than multiplying and then subtracting...

            TOverflow carry = TOverflow.Zero;

            for (int i = 0; i < right.Length; i++)
            {
                carry += Widen<TOverflow>(right[i]) * q;
                nuint digit = Narrow(carry);
                carry >>= BigInteger.BitsPerElement;
                ref nuint leftElement = ref left[i];
                if (leftElement < digit)
                    ++carry;
                leftElement = unchecked(leftElement - digit);
            }

            return Narrow(carry);
        }

        private static bool DivideGuessTooBig<TOverflow>(TOverflow q, TOverflow valHi, nuint valLo, nuint divHi, nuint divLo)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(q <= Widen<TOverflow>(nuint.MaxValue));

            // We multiply the two most significant limbs of the divisor
            // with the current guess for the quotient. If those are bigger
            // than the three most significant limbs of the current dividend
            // we return true, which means the current guess is still too big.

            TOverflow chkHi = Widen<TOverflow>(divHi) * q;
            TOverflow chkLo = Widen<TOverflow>(divLo) * q;

            chkHi += chkLo >> BigInteger.BitsPerElement;
            chkLo &= Widen<TOverflow>(nuint.MaxValue);

            return (chkHi >= valHi) && ((chkHi > valHi) || ((chkLo >= Widen<TOverflow>(valLo)) && (chkLo > Widen<TOverflow>(valLo))));
        }
    }
}
