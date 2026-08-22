// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Numerics
{
    internal static partial class BigIntegerCalculator
    {
        internal interface IBitwiseOp
        {
            static abstract nuint Invoke(nuint x, nuint y);
        }

        internal struct BitwiseAndOp : IBitwiseOp
        {
            public static nuint Invoke(nuint x, nuint y) => x & y;
        }

        internal struct BitwiseOrOp : IBitwiseOp
        {
            public static nuint Invoke(nuint x, nuint y) => x | y;
        }

        internal struct BitwiseXorOp : IBitwiseOp
        {
            public static nuint Invoke(nuint x, nuint y) => x ^ y;
        }

        /// <summary>
        /// Applies a bitwise operation in two's complement, writing the result into <paramref name="result"/>.
        /// The caller is responsible for allocating <paramref name="result"/> with the correct length.
        /// </summary>
        /// <param name="left">Magnitude limbs of the left operand (empty if inline).</param>
        /// <param name="leftSign">The _sign field of the left operand; decoded via <see cref="BigInteger.IsNegativeSign"/>.</param>
        /// <param name="leftOffset">The logical index of the first stored left limb.</param>
        /// <param name="right">Magnitude limbs of the right operand (empty if inline).</param>
        /// <param name="rightSign">The _sign field of the right operand; decoded via <see cref="BigInteger.IsNegativeSign"/>.</param>
        /// <param name="rightOffset">The logical index of the first stored right limb.</param>
        /// <param name="result">Pre-allocated destination span for the result limbs.</param>
        public static void BitwiseOp<TOp>(
            ReadOnlySpan<nuint> left, int leftSign, int leftOffset,
            ReadOnlySpan<nuint> right, int rightSign, int rightOffset,
            Span<nuint> result)
            where TOp : struct, IBitwiseOp
        {
            bool leftNeg = BigInteger.IsNegativeSign(leftSign);
            bool rightNeg = BigInteger.IsNegativeSign(rightSign);

            int xLen = left.Length > 0 ? leftOffset + left.Length : 1;
            int yLen = right.Length > 0 ? rightOffset + right.Length : 1;
            nuint xInline = (nuint)leftSign;
            nuint yInline = (nuint)rightSign;

            // Borrow initialized to 1: two's complement is ~x + 1, so the first
            // limb gets the +1 via borrow, then it propagates through subsequent limbs.
            nuint xBorrow = 1, yBorrow = 1;

            for (int i = 0; i < result.Length; i++)
            {
                nuint xu = GetTwosComplementLimb(left, leftOffset, xInline, i, xLen, leftNeg, ref xBorrow);
                nuint yu = GetTwosComplementLimb(right, rightOffset, yInline, i, yLen, rightNeg, ref yBorrow);
                result[i] = TOp.Invoke(xu, yu);
            }
        }

        /// <summary>
        /// Returns the i-th limb of a value in two's complement representation,
        /// computed on-the-fly from the magnitude without allocating a temp buffer.
        /// For positive values, returns magnitude limbs with zero extension.
        /// For negative values, computes ~magnitude + 1 with carry propagation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static nuint GetTwosComplementLimb(ReadOnlySpan<nuint> bits, int offset, nuint inlineValue, int i, int len, bool isNegative, ref nuint borrow)
        {
            // Get the magnitude limb (or sign-extension beyond the value)
            nuint mag;
            if (bits.Length > 0)
            {
                int storedIndex = i - offset;
                mag = (uint)storedIndex < (uint)bits.Length ? bits[storedIndex] : 0;
            }
            else
            {
                // Inline value: the sign field holds the value directly, so the magnitude is its
                // absolute value. This is an inline path, never an encoded array-backed sign.
                mag = i == 0 ? (isNegative ? NumericsHelpers.Abs((int)inlineValue) : inlineValue) : 0;
            }

            if (!isNegative)
            {
                return (uint)i < (uint)len ? mag : 0;
            }

            // Two's complement: ~mag + borrow (borrow starts at 1 for the +1)
            nuint tc = ~mag + borrow;
            borrow = (tc < ~mag || (tc == 0 && borrow != 0)) ? (nuint)1 : 0;
            return (uint)i < (uint)len ? tc : nuint.MaxValue;
        }
    }
}
