// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        private const int CopyToThreshold = 8;

        private static void CopyTail(ReadOnlySpan<nuint> source, Span<nuint> dest, int start)
        {
            source[start..].CopyTo(dest[start..]);
        }

        public static void Add(ReadOnlySpan<nuint> left, nuint right, Span<nuint> bits)
        {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(bits.Length == left.Length + 1);

            if (Environment.Is64BitProcess)
            {
                Add<Int128>(left, bits, ref MemoryMarshal.GetReference(bits), startIndex: 0, initialCarry: right);
            }
            else
            {
                Add<long>(left, bits, ref MemoryMarshal.GetReference(bits), startIndex: 0, initialCarry: (long)right);
            }
        }

        public static void Add<TOverflow>(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(bits.Length == left.Length + 1);

            // Switching to managed references helps eliminating
            // index bounds check for all buffers.
            ref nuint resultPtr = ref MemoryMarshal.GetReference(bits);
            ref nuint rightPtr = ref MemoryMarshal.GetReference(right);
            ref nuint leftPtr = ref MemoryMarshal.GetReference(left);

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // While calculating z_i = a_i + b_i we take care of overflow:
            // Since a_i + b_i + c <= 2(2^BitsPerElement - 1) + 1 = 2^BitsPerElement, our carry c
            // has always the value 1 or 0; hence, we're safe here.

            do
            {
                carry += Widen<TOverflow>(Unsafe.Add(ref leftPtr, i));
                carry += Widen<TOverflow>(Unsafe.Add(ref rightPtr, i));

                Unsafe.Add(ref resultPtr, i) = Narrow(carry);

                carry >>= BigInteger.BitsPerElement;
                i++;
            }
            while (i < right.Length);

            Add(left, bits, ref resultPtr, startIndex: i, initialCarry: carry);
        }

        private static void AddSelf<TOverflow>(Span<nuint> left, ReadOnlySpan<nuint> right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= right.Length);

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            // Switching to managed references helps eliminating
            // index bounds check...
            ref nuint leftPtr = ref MemoryMarshal.GetReference(left);

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // Same as above, but we're writing the result directly to a and
            // stop execution, if we're out of b and c is already 0.

            for ( ; i < right.Length; i++)
            {
                TOverflow digit = Widen<TOverflow>(Unsafe.Add(ref leftPtr, i)) + carry + Widen<TOverflow>(right[i]);
                Unsafe.Add(ref leftPtr, i) = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }
            for ( ; (carry != TOverflow.Zero) && (i < left.Length); i++)
            {
                TOverflow digit = Widen<TOverflow>(left[i]) + carry;
                left[i] = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            Debug.Assert(carry == TOverflow.Zero);
        }

        public static void Subtract<TOverflow>(ReadOnlySpan<nuint> left, nuint right, Span<nuint> bits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= 1);
            Debug.Assert(left[0] >= right || left.Length >= 2);
            Debug.Assert(bits.Length == left.Length);

            Subtract(left, bits, ref MemoryMarshal.GetReference(bits), startIndex: 0, initialCarry: -Widen<TOverflow>(right));
        }

        public static void Subtract<TOverflow>(ReadOnlySpan<nuint> left, ReadOnlySpan<nuint> right, Span<nuint> bits)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(Compare(left, right) >= 0);
            Debug.Assert(bits.Length == left.Length);

            // Switching to managed references helps eliminating
            // index bounds check for all buffers.
            ref nuint resultPtr = ref MemoryMarshal.GetReference(bits);
            ref nuint rightPtr = ref MemoryMarshal.GetReference(right);
            ref nuint leftPtr = ref MemoryMarshal.GetReference(left);

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // While calculating z_i = a_i + b_i we take care of overflow:
            // Since a_i + b_i + c <= 2(2^BitsPerElement - 1) + 1 = 2^BitsPerElement, our carry c
            // has always the value 1 or 0; hence, we're safe here.

            do
            {
                carry += Widen<TOverflow>(Unsafe.Add(ref leftPtr, i));
                carry -= Widen<TOverflow>(Unsafe.Add(ref rightPtr, i));

                Unsafe.Add(ref resultPtr, i) = Narrow(carry);

                carry >>= BigInteger.BitsPerElement;
                i++;
            } while (i < right.Length);

            Subtract(left, bits, ref resultPtr, startIndex: i, initialCarry: carry);
        }

        private static void SubtractSelf<TOverflow>(Span<nuint> left, ReadOnlySpan<nuint> right)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));
            Debug.Assert(left.Length >= right.Length);

            // Assertion failing per https://github.com/dotnet/runtime/issues/97780
            // Debug.Assert(Compare(left, right) >= 0);

            int i = 0;
            TOverflow carry = TOverflow.Zero;

            // Switching to managed references helps eliminating
            // index bounds check...
            ref nuint leftPtr = ref MemoryMarshal.GetReference(left);

            // Executes the "grammar-school" algorithm for computing z = a - b.
            // Same as above, but we're writing the result directly to a and
            // stop execution, if we're out of b and c is already 0.

            for (; i < right.Length; i++)
            {
                TOverflow digit = Widen<TOverflow>(Unsafe.Add(ref leftPtr, i)) + carry - Widen<TOverflow>(right[i]);
                Unsafe.Add(ref leftPtr, i) = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            for (; carry != TOverflow.Zero && i < left.Length; i++)
            {
                TOverflow digit = Widen<TOverflow>(left[i]) + carry;
                left[i] = Narrow(digit);
                carry = digit >> BigInteger.BitsPerElement;
            }

            // Assertion failing per https://github.com/dotnet/runtime/issues/97780
            // Debug.Assert(carry == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add<TOverflow>(ReadOnlySpan<nuint> left, Span<nuint> bits, ref nuint resultPtr, int startIndex, TOverflow initialCarry)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            // Executes the addition for one big and one BitsPerElement-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the BitsPerElement-bit integer, since it's a single element.

            int i = startIndex;
            TOverflow carry = initialCarry;

            if (left.Length <= CopyToThreshold)
            {
                for (; i < left.Length; i++)
                {
                    carry += Widen<TOverflow>(left[i]);
                    Unsafe.Add(ref resultPtr, i) = Narrow(carry);
                    carry >>= BigInteger.BitsPerElement;
                }

                Unsafe.Add(ref resultPtr, left.Length) = Narrow(carry);
            }
            else
            {
                for (; i < left.Length;)
                {
                    carry += Widen<TOverflow>(left[i]);
                    Unsafe.Add(ref resultPtr, i) = Narrow(carry);
                    i++;
                    carry >>= BigInteger.BitsPerElement;

                    // Once carry is set to 0 it can not be 1 anymore.
                    // So the tail of the loop is just the movement of argument values to result span.
                    if (carry == TOverflow.Zero)
                    {
                        break;
                    }
                }

                Unsafe.Add(ref resultPtr, left.Length) = Narrow(carry);

                if (i < left.Length)
                {
                    CopyTail(left, bits, i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Subtract<TOverflow>(ReadOnlySpan<nuint> left, Span<nuint> bits, ref nuint resultPtr, int startIndex, TOverflow initialCarry)
            where TOverflow : unmanaged, IBinaryInteger<TOverflow>, ISignedNumber<TOverflow>
        {
            Debug.Assert(Unsafe.SizeOf<TOverflow>() == (Unsafe.SizeOf<nuint>() * 2));

            // Executes the addition for one big and one BitsPerElement-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the BitsPerElement-bit integer, since it's a single element.

            int i = startIndex;
            TOverflow carry = initialCarry;

            if (left.Length <= CopyToThreshold)
            {
                for (; i < left.Length; i++)
                {
                    carry += Widen<TOverflow>(left[i]);
                    Unsafe.Add(ref resultPtr, i) = Narrow(carry);
                    carry >>= BigInteger.BitsPerElement;
                }
            }
            else
            {
                for (; i < left.Length;)
                {
                    carry += Widen<TOverflow>(left[i]);
                    Unsafe.Add(ref resultPtr, i) = Narrow(carry);
                    i++;
                    carry >>= BigInteger.BitsPerElement;

                    // Once carry is set to 0 it can not be 1 anymore.
                    // So the tail of the loop is just the movement of argument values to result span.
                    if (carry == TOverflow.Zero)
                    {
                        break;
                    }
                }

                if (i < left.Length)
                {
                    CopyTail(left, bits, i);
                }
            }
        }
    }
}
