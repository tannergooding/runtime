// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        public static nuint Gcd(nuint left, nuint right)
        {
            // Executes the classic Euclidean algorithm.
            // https://en.wikipedia.org/wiki/Euclidean_algorithm

            while (right != 0)
            {
                nuint temp = left % right;
                left = right;
                right = temp;
            }

            return left;
        }

        public static TOverflow Gcd<TOverflow>(TOverflow left, TOverflow right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            // Same as above, but for BitsPerOverflow-bit values.

            while (right > Widen<TOverflow>(nuint.MaxValue))
            {
                TOverflow temp = left % right;
                left = right;
                right = temp;
            }

            if (right != TOverflow.Zero)
            {
                return Widen<TOverflow>(Gcd(Narrow(right), Narrow(left % right)));
            }
            return left;
        }

        public static nuint Gcd(ReadOnlySpan<nuint> left, nuint right)
        {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right != 0);

            // A common divisor cannot be greater than right;
            // we compute the remainder and continue above...

            nuint temp;

            if (Environment.Is64BitProcess)
            {
                temp = Remainder<UInt128>(left, right);
            }
            else
            {
                temp = Remainder<ulong>(left, right);
            }
            return Gcd(right, temp);
        }

        public static void Gcd(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> result)
        {
            Debug.Assert(left.Length >= 2);
            Debug.Assert(right.Length >= 2);
            Debug.Assert(Compare(left, right) >= 0);
            Debug.Assert(result.Length == left.Length);

            left.CopyTo(result);

            nuint[]? rightCopyFromPool = null;
            Span<nuint> rightCopy = (right.Length <= StackAllocThreshold
                                  ? stackalloc nuint[StackAllocThreshold]
                                  : rightCopyFromPool = ArrayPool<nuint>.Shared.Rent(right.Length))[..right.Length];
            right.CopyTo(rightCopy);

            if (Environment.Is64BitProcess)
            {
                Gcd<UInt128>(result, rightCopy);
            }
            else
            {
                Gcd<ulong>(result, rightCopy);
            }

            if (rightCopyFromPool != null)
            {
                ArrayPool<nuint>.Shared.Return(rightCopyFromPool);
            }
        }

        private static void Gcd<TOverflow>(Span<nuint> left, Span<nuint> right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 2);
            Debug.Assert(right.Length >= 2);
            Debug.Assert(left.Length >= right.Length);

            Span<nuint> result = left;   //keep result buffer untouched during computation

            // Executes Lehmer's gcd algorithm, but uses the most
            // significant bits to work with BitsPerOverflow-bit (not BitsPerElement-bit) values.
            // Furthermore we're using an optimized version due to Jebelean.

            // http://cacr.uwaterloo.ca/hac/about/chap14.pdf (see 14.4.2)
            // ftp://ftp.risc.uni-linz.ac.at/pub/techreports/1992/92-69.ps.gz

            while (right.Length > 2)
            {
                ExtractDigits(left, right, out TOverflow x, out TOverflow y);

                nuint a = 1U, b = 0U;
                nuint c = 0U, d = 1U;

                int iteration = 0;

                // Lehmer's guessing
                while (y != TOverflow.Zero)
                {
                    TOverflow q, r, s, t;

                    // Odd iteration
                    q = x / y;

                    if (q > Widen<TOverflow>(nuint.MaxValue))
                    {
                        break;
                    }

                    r = Widen<TOverflow>(a) + (q * Widen<TOverflow>(c));
                    s = Widen<TOverflow>(b) + (q * Widen<TOverflow>(d));
                    t = x - (q * y);

                    if ((r > Widen<TOverflow>((nuint)nint.MaxValue)) || (s > Widen<TOverflow>((nuint)nint.MaxValue)))
                    {
                        break;
                    }

                    if ((t < s) || ((t + r) > (y - Widen<TOverflow>(c))))
                    {
                        break;
                    }

                    a = Narrow(r);
                    b = Narrow(s);
                    x = t;

                    ++iteration;

                    if (x == Widen<TOverflow>(b))
                    {
                        break;
                    }

                    // Even iteration
                    q = y / x;

                    if (q > Widen<TOverflow>(nuint.MaxValue))
                    {
                        break;
                    }

                    r = Widen<TOverflow>(d) + (q * Widen<TOverflow>(b));
                    s = Widen<TOverflow>(c) + (q * Widen<TOverflow>(a));
                    t = y - (q * x);

                    if ((r > Widen<TOverflow>((nuint)nint.MaxValue)) || (s > Widen<TOverflow>((nuint)nint.MaxValue)))
                    {
                        break;
                    }

                    if ((t < s) || ((t + r) > (x - Widen<TOverflow>(b))))
                    {
                        break;
                    }

                    d = Narrow(r);
                    c = Narrow(s);
                    y = t;

                    ++iteration;

                    if (y == Widen<TOverflow>(c))
                    {
                        break;
                    }
                }

                if (b == 0)
                {
                    // Euclid's step
                    left = left[..Reduce(left, right)];

                    Span<nuint> temp = left;
                    left = right;
                    right = temp;
                }
                else
                {
                    // Lehmer's step
                    int count;

                    if (Environment.Is64BitProcess)
                    {
                        count = LehmerCore<Int128>(left, right, a, b, c, d);
                    }
                    else
                    {
                        count = LehmerCore<long>(left, right, (long)a, (long)b, (long)c, (long)d);
                    }

                    left = left[..Refresh(left, count)];
                    right = right[..Refresh(right, count)];

                    if (iteration % 2 == 1)
                    {
                        // Ensure left is larger than right
                        Span<nuint> temp = left;
                        left = right;
                        right = temp;
                    }
                }
            }

            if (right.Length > 0)
            {
                // Euclid's step
                Reduce(left, right);

                TOverflow x = Widen<TOverflow>(right[0]);
                TOverflow y = Widen<TOverflow>(left[0]);

                if (right.Length > 1)
                {
                    x = Create(right[1], x);
                    y = Create(left[1], y);
                }

                left = left[..Overwrite(left, Gcd(x, y))];
                right.Clear();
            }

            left.CopyTo(result);
        }

        private static int Overwrite<TOverflow>(Span<nuint> buffer, TOverflow value)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(buffer.Length >= 2);

            if (buffer.Length > 2)
            {
                // Ensure leading zeros in little-endian
                buffer[2..].Clear();
            }

            nuint lo = Narrow(value);
            nuint hi = Narrow(value >> BigInteger.BitsPerElement);

            buffer[1] = hi;
            buffer[0] = lo;

            return (hi != 0) ? 2 : ((lo != 0) ? 1 : 0);
        }

        private static void ExtractDigits<TOverflow>(ReadOnlySpan<nuint> xBuffer, ReadOnlySpan<nuint> yBuffer, out TOverflow x, out TOverflow y)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(xBuffer.Length >= 3);
            Debug.Assert(yBuffer.Length >= 3);
            Debug.Assert(xBuffer.Length >= yBuffer.Length);

            // Extracts the most significant bits of x and y,
            // but ensures the quotient x / y does not change!

            TOverflow xh = Widen<TOverflow>(xBuffer[^1]);
            TOverflow xm = Widen<TOverflow>(xBuffer[^2]);
            TOverflow xl = Widen<TOverflow>(xBuffer[^3]);

            TOverflow yh, ym, yl;

            // arrange the bits
            switch (xBuffer.Length - yBuffer.Length)
            {
                case 0:
                {
                    yh = Widen<TOverflow>(yBuffer[^1]);
                    ym = Widen<TOverflow>(yBuffer[^2]);
                    yl = Widen<TOverflow>(yBuffer[^3]);
                    break;
                }

                case 1:
                {
                    yh = TOverflow.Zero;
                    ym = Widen<TOverflow>(yBuffer[^1]);
                    yl = Widen<TOverflow>(yBuffer[^2]);
                    break;
                }

                case 2:
                {
                    yh = TOverflow.Zero;
                    ym = TOverflow.Zero;
                    yl = Widen<TOverflow>(yBuffer[^1]);
                    break;
                }

                default:
                {
                    yh = TOverflow.Zero;
                    ym = TOverflow.Zero;
                    yl = TOverflow.Zero;
                    break;
                }
            }

            // Use all the bits but one, see [hac] 14.58 (ii)
            int z = BitOperations.LeadingZeroCount(Narrow(xh));

            x = ((xh << (BigInteger.BitsPerElement + z)) | (xm << z) | (xl >> (BigInteger.BitsPerElement - z))) >> 1;
            y = ((yh << (BigInteger.BitsPerElement + z)) | (ym << z) | (yl >> (BigInteger.BitsPerElement - z))) >> 1;

            Debug.Assert(x >= y);
        }

        private static int LehmerCore<TOverflow>(Span<nuint> x, Span<nuint> y, TOverflow a, TOverflow b, TOverflow c, TOverflow d)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(x.Length >= 1);
            Debug.Assert(y.Length >= 1);
            Debug.Assert(x.Length >= y.Length);
            Debug.Assert(a <= Widen<TOverflow>((nuint)nint.MaxValue) && (b <= Widen<TOverflow>((nuint)nint.MaxValue)));
            Debug.Assert(c <= Widen<TOverflow>((nuint)nint.MaxValue) && (d <= Widen<TOverflow>((nuint)nint.MaxValue)));

            // Executes the combined calculation of Lehmer's step.

            int length = y.Length;

            TOverflow xCarry = TOverflow.Zero;
            TOverflow yCarry = TOverflow.Zero;

            for (int i = 0; i < length; i++)
            {
                TOverflow xDigit = (a * Widen<TOverflow>(x[i])) - (b * Widen<TOverflow>(y[i])) + xCarry;
                TOverflow yDigit = (d * Widen<TOverflow>(y[i])) - (c * Widen<TOverflow>(x[i])) + yCarry;

                xCarry = xDigit >> BigInteger.BitsPerElement;
                yCarry = yDigit >> BigInteger.BitsPerElement;

                x[i] = Narrow(xDigit);
                y[i] = Narrow(yDigit);
            }

            return length;
        }

        private static int Refresh(Span<nuint> bits, int maxLength)
        {
            Debug.Assert(bits.Length >= maxLength);

            if (bits.Length > maxLength)
            {
                // Ensure leading zeros
                bits[maxLength..].Clear();
            }

            return ActualLength(bits[..maxLength]);
        }
    }
}
