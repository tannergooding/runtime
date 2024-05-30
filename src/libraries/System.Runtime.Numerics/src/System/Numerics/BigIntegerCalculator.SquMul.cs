// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        internal static
#if !DEBUG
        // Mutable for unit testing...
        readonly
#endif
        int SquareThreshold = BigInteger.BitsPerElement;

        public static void Square<TOverflow>(ReadOnlySpan<nuint> value, Span<nuint> bits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(bits.Length == (value.Length + value.Length));

            // Executes different algorithms for computing z = a * a
            // based on the actual length of a. If a is "small" enough
            // we stick to the classic "grammar-school" method; for the
            // rest we switch to implementations with less complexity
            // albeit more overhead (which needs to pay off!).

            // NOTE: useful thresholds needs some "empirical" testing,
            // which are smaller in DEBUG mode for testing purpose.

            if (value.Length < SquareThreshold)
            {
                // Switching to managed references helps eliminating
                // index bounds check...
                ref nuint resultPtr = ref MemoryMarshal.GetReference(bits);

                // Squares the bits using the "grammar-school" method.
                // Envisioning the "rhombus" of a pen-and-paper calculation
                // we see that computing z_i+j += a_j * a_i can be optimized
                // since a_j * a_i = a_i * a_j (we're squaring after all!).
                // Thus, we directly get z_i+j += 2 * a_j * a_i + c.

                // ATTENTION: an ordinary multiplication is safe, because
                // z_i+j + a_j * a_i + c <= 2(2^BitsPerElement - 1) + (2^BitsPerElement - 1)^2 =
                // = 2^BitsPerOverflow - 1 (which perfectly matches with TOverflow!). But
                // here we would need an BitsPerOverflow + 1... Hence, we split these
                // operation and do some extra shifts.

                for (int i = 0; i < value.Length; i++)
                {
                    TOverflow carry = TOverflow.Zero;
                    nuint v = value[i];

                    for (int j = 0; j < i; j++)
                    {
                        TOverflow digit1 = Widen<TOverflow>(Unsafe.Add(ref resultPtr, i + j)) + carry;
                        TOverflow digit2 = Widen<TOverflow>(value[j]) * Widen<TOverflow>(v);
                        Unsafe.Add(ref resultPtr, i + j) = Narrow(digit1 + (digit2 << 1));
                        carry = (digit2 + (digit1 >> 1)) >> (BigInteger.BitsPerElement - 1);
                    }

                    TOverflow digits = Widen<TOverflow>(v);

                    digits *= digits;
                    digits += carry;

                    Unsafe.Add(ref resultPtr, i + i) = Narrow(digits);
                    Unsafe.Add(ref resultPtr, i + i + 1) = Narrow(digits >> BigInteger.BitsPerElement);
                }
            }
            else
            {
                // Based on the Toom-Cook multiplication we split value
                // into two smaller values, doing recursive squaring.
                // The special form of this multiplication, where we
                // split both operands into two operands, is also known
                // as the Karatsuba algorithm...

                // https://en.wikipedia.org/wiki/Toom-Cook_multiplication
                // https://en.wikipedia.org/wiki/Karatsuba_algorithm

                // Say we want to compute z = a * a ...

                // ... we need to determine our new length (just the half)
                int n = value.Length >> 1;
                int n2 = n << 1;

                // ... split value like a = (a_1 << n) + a_0
                ReadOnlySpan<nuint> valueLow = value[..n];
                ReadOnlySpan<nuint> valueHigh = value[n..];

                // ... prepare our result array (to reuse its memory)
                Span<nuint> bitsLow = bits[..n2];
                Span<nuint> bitsHigh = bits[n2..];

                // ... compute z_0 = a_0 * a_0 (squaring again!)
                Square<TOverflow>(valueLow, bitsLow);

                // ... compute z_2 = a_1 * a_1 (squaring again!)
                Square<TOverflow>(valueHigh, bitsHigh);

                int foldLength = valueHigh.Length + 1;
                nuint[]? foldFromPool = null;
                Span<nuint> fold = ((uint)foldLength <= StackAllocThreshold
                                 ? stackalloc nuint[StackAllocThreshold]
                                 : foldFromPool = ArrayPool<nuint>.Shared.Rent(foldLength))[..foldLength];
                fold.Clear();

                int coreLength = foldLength + foldLength;
                nuint[]? coreFromPool = null;
                Span<nuint> core = ((uint)coreLength <= StackAllocThreshold
                                 ? stackalloc nuint[StackAllocThreshold]
                                 : coreFromPool = ArrayPool<nuint>.Shared.Rent(coreLength))[..coreLength];
                core.Clear();

                // ... compute z_a = a_1 + a_0 (call it fold...)
                if (Environment.Is64BitProcess)
                {
                    Add<Int128>(valueHigh, valueLow, fold);
                }
                else
                {
                    Add<long>(valueHigh, valueLow, fold);
                }

                // ... compute z_1 = z_a * z_a - z_0 - z_2
                Square<TOverflow>(fold, core);

                if (foldFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(foldFromPool);
                }

                if (Environment.Is64BitProcess)
                {
                    SubtractCore<Int128>(bitsHigh, bitsLow, core);

                    // ... and finally merge the result! :-)
                    AddSelf<Int128>(bits[n..], core);
                }
                else
                {
                    SubtractCore<long>(bitsHigh, bitsLow, core);

                    // ... and finally merge the result! :-)
                    AddSelf<long>(bits[n..], core);
                }

                if (coreFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(coreFromPool);
                }
            }
        }

        public static void Multiply<TOverflow>(ReadOnlySpan<nuint> left, nuint right, Span<nuint> bits)
                where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(bits.Length == left.Length + 1);

            // Executes the multiplication for one big and one BitsPerElement-bit integer.
            // Since every step holds the already slightly familiar equation
            // a_i * b + c <= 2^BitsPerElement - 1 + (2^BitsPerElement - 1)^2 < 2^BitsPerOverflow - 1,
            // we are safe regarding to overflows.

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            for (; i < left.Length; i++)
            {
                TOverflow digits = (Widen<TOverflow>(left[i]) * Widen<TOverflow>(right)) + carry;
                bits[i] = Narrow(digits);
                carry = digits >> BigInteger.BitsPerElement;
            }
            bits[i] = Narrow(carry);
        }

        internal static
#if !DEBUG
        // Mutable for unit testing...
        readonly
#endif
        int MultiplyKaratsubaThreshold = BigInteger.BitsPerElement;

        public static void Multiply(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits)
        {
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(bits.Length >= (left.Length + right.Length));
            Debug.Assert(bits.Trim(0u).IsEmpty);
            Debug.Assert(MultiplyKaratsubaThreshold >= 2);

            // Executes different algorithms for computing z = a * b
            // based on the actual length of b. If b is "small" enough
            // we stick to the classic "grammar-school" method; for the
            // rest we switch to implementations with less complexity
            // albeit more overhead (which needs to pay off!).

            // NOTE: useful thresholds needs some "empirical" testing,
            // which are smaller in DEBUG mode for testing purpose.

            if (right.Length < MultiplyKaratsubaThreshold)
            {
                if (Environment.Is64BitProcess)
                {
                    Naive<UInt128>(left, right, bits);
                }
                else
                {

                    Naive<ulong>(left, right, bits);
                }
                return;
            }

            //                                            upper           lower
            // A=   |               |               | a1 = a[n..2n] | a0 = a[0..n] |
            // B=   |               |               | b1 = b[n..2n] | b0 = b[0..n] |

            // Result
            // z0=  |               |               |            a0 * b0            |
            // z1=  |               |       a1 * b0 + a0 * b1       |               |
            // z2=  |            a1 * b1            |               |               |

            // z1 = a1 * b0 + a0 * b1
            //    = (a0 + a1) * (b0 + b1) - a0 * b0 - a1 * b1
            //    = (a0 + a1) * (b0 + b1) - z0 - z2


            // Based on the Toom-Cook multiplication we split left/right
            // into two smaller values, doing recursive multiplication.
            // The special form of this multiplication, where we
            // split both operands into two operands, is also known
            // as the Karatsuba algorithm...

            // https://en.wikipedia.org/wiki/Toom-Cook_multiplication
            // https://en.wikipedia.org/wiki/Karatsuba_algorithm

            // Say we want to compute z = a * b ...

            // ... we need to determine our new length (just the half)
            int n = (left.Length + 1) >> 1;

            if (right.Length <= n)
            {
                // ... split left like a = (a_1 << n) + a_0
                ReadOnlySpan<nuint> leftLow = left[..n];
                ReadOnlySpan<nuint> leftHigh = left[n..];
                Debug.Assert(leftLow.Length >= leftHigh.Length);

                // ... prepare our result array (to reuse its memory)
                Span<nuint> bitsLow = bits[..(n + right.Length)];
                Span<nuint> bitsHigh = bits[n..];

                // ... compute low
                Multiply(leftLow, right, bitsLow);

                int carryLength = right.Length;
                nuint[]? carryFromPool = null;
                Span<nuint> carry = ((uint)carryLength <= StackAllocThreshold
                                  ? stackalloc nuint[StackAllocThreshold]
                                  : carryFromPool = ArrayPool<nuint>.Shared.Rent(carryLength))[..carryLength];

                Span<nuint> carryOrig = bits.Slice(n, right.Length);
                carryOrig.CopyTo(carry);
                carryOrig.Clear();

                // ... compute high
                if (leftHigh.Length < right.Length)
                {
                    MultiplyKaratsuba(right, leftHigh, bitsHigh[..(leftHigh.Length + right.Length)], (right.Length + 1) >> 1);
                }
                else
                {
                    Multiply(leftHigh, right, bitsHigh[..(leftHigh.Length + right.Length)]);
                }

                if (Environment.Is64BitProcess)
                {
                    AddSelf<Int128>(bitsHigh, carry);
                }
                else
                {
                    AddSelf<long>(bitsHigh, carry);
                }

                if (carryFromPool != null)
                {
                    ArrayPool<nuint>.Shared.Return(carryFromPool);
                }
            }
            else
            {
                MultiplyKaratsuba(left, right, bits, n);
            }

            static void MultiplyKaratsuba(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits, int n)
            {
                Debug.Assert(left.Length >= right.Length);
                Debug.Assert(((2 * n) - left.Length) is 0 or 1);
                Debug.Assert(right.Length > n);
                Debug.Assert(bits.Length >= left.Length + right.Length);

                if (right.Length < MultiplyKaratsubaThreshold)
                {
                    if (Environment.Is64BitProcess)
                    {
                        Naive<UInt128>(left, right, bits);
                    }
                    else
                    {

                        Naive<ulong>(left, right, bits);
                    }
                }
                else
                {
                    // ... split left like a = (a_1 << n) + a_0
                    ReadOnlySpan<nuint> leftLow = left[..n];
                    ReadOnlySpan<nuint> leftHigh = left[n..];

                    // ... split right like b = (b_1 << n) + b_0
                    ReadOnlySpan<nuint> rightLow = right[..n];
                    ReadOnlySpan<nuint> rightHigh = right[n..];

                    // ... prepare our result array (to reuse its memory)
                    Span<nuint> bitsLow = bits[..(n + n)];
                    Span<nuint> bitsHigh = bits[(n + n)..];

                    Debug.Assert(leftLow.Length >= leftHigh.Length);
                    Debug.Assert(rightLow.Length >= rightHigh.Length);
                    Debug.Assert(bitsLow.Length >= bitsHigh.Length);

                    // ... compute z_0 = a_0 * b_0 (multiply again)
                    MultiplyKaratsuba(leftLow, rightLow, bitsLow, (leftLow.Length + 1) >> 1);

                    // ... compute z_2 = a_1 * b_1 (multiply again)
                    Multiply(leftHigh, rightHigh, bitsHigh);

                    int foldLength = n + 1;
                    nuint[]? leftFoldFromPool = null;
                    Span<nuint> leftFold = ((uint)foldLength <= StackAllocThreshold
                                         ? stackalloc nuint[StackAllocThreshold]
                                         : leftFoldFromPool = ArrayPool<nuint>.Shared.Rent(foldLength))[..foldLength];
                    leftFold.Clear();

                    nuint[]? rightFoldFromPool = null;
                    Span<nuint> rightFold = ((uint)foldLength <= StackAllocThreshold
                                          ? stackalloc nuint[StackAllocThreshold]
                                          : rightFoldFromPool = ArrayPool<nuint>.Shared.Rent(foldLength))[..foldLength];
                    rightFold.Clear();

                    if (Environment.Is64BitProcess)
                    {
                        // ... compute z_a = a_1 + a_0 (call it fold...)
                        Add<Int128>(leftLow, leftHigh, leftFold);

                        // ... compute z_b = b_1 + b_0 (call it fold...)
                        Add<Int128>(rightLow, rightHigh, rightFold);
                    }
                    else
                    {
                        // ... compute z_a = a_1 + a_0 (call it fold...)
                        Add<long>(leftLow, leftHigh, leftFold);

                        // ... compute z_b = b_1 + b_0 (call it fold...)
                        Add<long>(rightLow, rightHigh, rightFold);
                    }

                    int coreLength = foldLength + foldLength;
                    nuint[]? coreFromPool = null;
                    Span<nuint> core = ((uint)coreLength <= StackAllocThreshold
                                     ? stackalloc nuint[StackAllocThreshold]
                                     : coreFromPool = ArrayPool<nuint>.Shared.Rent(coreLength))[..coreLength];
                    core.Clear();

                    // ... compute z_ab = z_a * z_b
                    MultiplyKaratsuba(leftFold, rightFold, core, (leftFold.Length + 1) >> 1);

                    if (leftFoldFromPool != null)
                    {
                        ArrayPool<nuint>.Shared.Return(leftFoldFromPool);
                    }

                    if (rightFoldFromPool != null)
                    {
                        ArrayPool<nuint>.Shared.Return(rightFoldFromPool);
                    }

                    // ... compute z_1 = z_a * z_b - z_0 - z_2 = a_0 * b_1 + a_1 * b_0
                    if (Environment.Is64BitProcess)
                    {
                        SubtractCore<Int128>(bitsLow, bitsHigh, core);

                        Debug.Assert(ActualLength(core) <= left.Length + 1);

                        // ... and finally merge the result! :-)
                        AddSelf<Int128>(bits[n..], core[..ActualLength(core)]);
                    }
                    else
                    {
                        SubtractCore<long>(bitsLow, bitsHigh, core);

                        Debug.Assert(ActualLength(core) <= left.Length + 1);

                        // ... and finally merge the result! :-)
                        AddSelf<long>(bits[n..], core[..ActualLength(core)]);
                    }

                    if (coreFromPool != null)
                    {
                        ArrayPool<nuint>.Shared.Return(coreFromPool);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void Naive<TOverflow>(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits)
                where TOverflow : unmanaged, IBinaryInteger<TOverflow>, IUnsignedNumber<TOverflow>
            {
                Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
                Debug.Assert(right.Length < MultiplyKaratsubaThreshold);

                // Switching to managed references helps eliminating
                // index bounds check...
                ref nuint resultPtr = ref MemoryMarshal.GetReference(bits);

                // Multiplies the bits using the "grammar-school" method.
                // Envisioning the "rhombus" of a pen-and-paper calculation
                // should help getting the idea of these two loops...
                // The inner multiplication operations are safe, because
                // z_i+j + a_j * b_i + c <= 2(2^BitsPerElement - 1) + (2^BitsPerElement - 1)^2 =
                // = 2^BitsPerOverflow - 1 (which perfectly matches with TOverflow!).

                for (int i = 0; i < right.Length; i++)
                {
                    nuint rv = right[i];
                    TOverflow carry = TOverflow.Zero;

                    for (int j = 0; j < left.Length; j++)
                    {
                        ref nuint elementPtr = ref Unsafe.Add(ref resultPtr, i + j);
                        TOverflow digits = Widen<TOverflow>(elementPtr) + carry + (Widen<TOverflow>(left[j]) * Widen<TOverflow>(rv));
                        elementPtr = Narrow(digits);
                        carry = digits >> BigInteger.BitsPerElement;
                    }

                    Unsafe.Add(ref resultPtr, i + left.Length) = Narrow(carry);
                }
            }
        }

        private static void SubtractCore<TOverflow>(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> core)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(core.Length >= left.Length);

            // Executes a special subtraction algorithm for the multiplication,
            // which needs to subtract two different values from a core value,
            // while core is always bigger than the sum of these values.

            // NOTE: we could do an ordinary subtraction of course, but we spare
            // one "run", if we do this computation within a single one...

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            // Switching to managed references helps eliminating
            // index bounds check...
            ref nuint leftPtr = ref MemoryMarshal.GetReference(left);
            ref nuint corePtr = ref MemoryMarshal.GetReference(core);

            for (; i < right.Length; i++)
            {
                TOverflow digit = Widen<TOverflow>(Unsafe.Add(ref corePtr, i)) + carry - Widen<TOverflow>(Unsafe.Add(ref leftPtr, i)) - Widen<TOverflow>(right[i]);
                Unsafe.Add(ref corePtr, i) = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            for (; i < left.Length; i++)
            {
                TOverflow digit = Widen<TOverflow>(Unsafe.Add(ref corePtr, i)) + carry - Widen<TOverflow>(left[i]);
                Unsafe.Add(ref corePtr, i) = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            for (; (carry != TOverflow.Zero) && (i < core.Length); i++)
            {
                TOverflow digit = Widen<TOverflow>(core[i]) + carry;
                core[i] = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }
        }
    }
}
