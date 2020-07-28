// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*============================================================
**
** Purpose: Some single-precision floating-point math operations
**
===========================================================*/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System
{
    public static partial class MathF
    {
        // Portions of the below code are ported from the amd/win-libm implementation provided here: https://github.com/amd/win-libm
        // The original source is Copyright (c) 2002-2019 Advanced Micro Devices, Inc. and is licensed to you under the MIT License.

        private const double PiOverTwo = 1.5707963267948966;
        private const double PiOverTwoPartOne = 1.5707963267341256;
        private const double PiOverTwoPartOneTail = 6.077100506506192E-11;
        private const double PiOverTwoPartTwo = 6.077100506303966E-11;
        private const double PiOverTwoPartTwoTail = 2.0222662487959506E-21;
        private const double PiOverFour = 0.7853981633974483;
        private const double TwoOverPi = 0.6366197723675814;
        private const double TwoPowNegSeven = 0.0078125;
        private const double TwoPowNegThirteen = 0.0001220703125;

        private const double C0 = -1.0 / 2.0;       // 1 / 2!
        private const double C1 = +1.0 / 24.0;      // 1 / 4!
        private const double C2 = -1.0 / 720.0;     // 1 / 6!
        private const double C3 = +1.0 / 40320.0;   // 1 / 8!
        private const double C4 = -1.0 / 3628800.0; // 1 / 10!

        private const double S1 = -1.0 / 6.0;       // 1 / 3!
        private const double S2 = +1.0 / 120.0;     // 1 / 5!
        private const double S3 = -1.0 / 5040.0;    // 1 / 7!
        private const double S4 = +1.0 / 362880.0;  // 1 / 9!

        private static readonly long[] PiBits = new long[]
        {
            0,
            5215,
            13000023176,
            11362338026,
            67174558139,
            34819822259,
            10612056195,
            67816420731,
            57840157550,
            19558516809,
            50025467026,
            25186875954,
            18152700886
        };

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Acos(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Acosh(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Asin(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Asinh(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Atan(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Atan2(float y, float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Atanh(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Cbrt(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Ceiling(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Cos(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Cosh(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Exp(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Floor(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float FusedMultiplyAdd(float x, float y, float z);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int ILogB(float x);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Log(float x);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Log2(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Log10(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Pow(float x, float y);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float ScaleB(float x, int n);

        public static float Sin(float x)
        {
            double result = x;

            if (float.IsFinite(x))
            {
                double ax = Math.Abs(x);

                if (ax <= PiOverFour)
                {
                    if (ax >= TwoPowNegSeven)
                    {
                        // Lsinf_fma3_compute_sinf_piby_4
                        result = SinTaylorSeriesFourIterations(x);
                    }
                    else if (ax >= TwoPowNegThirteen)
                    {
                        // Lsinf_fma3_compute_x_xxx_0_1666
                        result = SinTaylorSeriesOneIteration(x);
                    }
                    else
                    {
                        result = x;
                    }
                }
                else
                {
                    // Lsinf_fma3_range_reduce

                    int wasNegative = 0;

                    if (float.IsNegative(x))
                    {
                        x = -x;
                        wasNegative = 1;
                    }

                    int region;

                    if (x < 16000000.0)
                    {
                        // Reduce x to be in the range of -(PI / 4) to (PI / 4), inclusive

                        // This is done by subtracting multiples of (PI / 2). Double-precision
                        // isn't quite accurate enough and introduces some error, but we account
                        // for that using a tail value that helps account for this.

                        long axExp = BitConverter.DoubleToInt64Bits(ax) >> 52;

                        region = (int)(x * TwoOverPi + 0.5);
                        double piOverTwoCount = region;

                        double rHead = x - (piOverTwoCount * PiOverTwoPartOne);
                        double rTail = (piOverTwoCount * PiOverTwoPartOneTail);

                        double r = rHead - rTail;
                        long rExp = (BitConverter.DoubleToInt64Bits(r) << 1) >> 53;

                        if ((axExp - rExp) > 15)
                        {
                            // The remainder is pretty small compared with x, which implies that x is
                            // near a multiple of (PI / 2). That is, x matches the multiple to at least
                            // 15 bits and so we perform an additional fixup to account for any error

                            r = rHead;

                            rTail = (piOverTwoCount * PiOverTwoPartTwo);
                            rHead = r - rTail;
                            rTail = (piOverTwoCount * PiOverTwoPartTwoTail) - ((r - rHead) - rTail);

                            r = rHead - rTail;
                        }

                        if (rExp >= 0x3F2)      // r >= 2^-13
                        {
                            if ((region & 1) == 0)  // region 0 or 2
                            {
                                result = SinTaylorSeriesFourIterations(r);
                            }
                            else                    // region 1 or 3
                            {
                                result = CosTaylorSeriesFourIterations(r);
                            }
                        }
                        else if (rExp > 0x3DE)  // r > 1.1641532182693481E-10
                        {
                            if ((region & 1) == 0)  // region 0 or 2
                            {
                                result = SinTaylorSeriesOneIteration(r);
                            }
                            else                    // region 1 or 3
                            {
                                result = CosTaylorSeriesOneIteration(r);

                            }
                        }
                        else
                        {
                            if ((region & 1) == 0)  // region 0 or 2
                            {
                                result = r;
                            }
                            else                    // region 1 or 3
                            {
                                result = 1;
                            }
                        }
                    }
                    else
                    {
                        double r = ReduceForLargeInput(x, out region);

                        if ((region & 1) == 0)  // region 0 or 2
                        {
                            result = SinTaylorSeriesFourIterations(r);
                        }
                        else                    // region 1 or 3
                        {
                            result = CosTaylorSeriesFourIterations(r);
                        }
                    }

                    region >>= 1;

                    int tmp1 = region & wasNegative;

                    region = ~region;
                    wasNegative = ~wasNegative;

                    int tmp2 = region & wasNegative;

                    if (((tmp1 | tmp2) & 1) == 0)
                    {
                        // If the original region was 0/1 and arg is negative, then we negate the result.
                        // -or-
                        // If the original region was 2/3 and arg is positive, then we negate the result.

                        result = -result;
                    }
                }
            }

            return (float)result;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double CosTaylorSeriesOneIteration(double x1)
            {
                // 1 - (x^2 / 2!)
                double x2 = x1 * x1;
                return 1.0 + (x2 * C1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double CosTaylorSeriesFourIterations(double x1)
            {
                // 1 - (x^2 / 2!) + (x^4 / 4!) - (x^6 / 6!) + (x^8 / 8!) - (x^10 / 10!)

                double x2 = x1 * x1;
                double x4 = x2 * x2;

                return 1.0 + (x2 * C0) + (x4 * ((C1 + (x2 * C2)) + (x4 * (C3 + (x2 * C4)))));
            }

            static unsafe double ReduceForLargeInput(double x, out int region)
            {
                Debug.Assert(!double.IsNegative(x));

                // This method simulates multi-precision floating-point
                // arithmetic and is accurate for all 1 <= x < infinity

                const int BitsPerIteration = 36;
                long ux = BitConverter.DoubleToInt64Bits(x);

                int xExp = (int)(((ux & 0x7FF0000000000000) >> 52) - 1023);
                ux = ((ux & 0x000FFFFFFFFFFFFF) | 0x0010000000000000) >> 29;

                // Now ux is the mantissa bit pattern of x as a long integer
                long mask = 1;
                mask = (mask << BitsPerIteration) - 1;

                // Set first and last to the positions of the first and last chunks of (2 / PI) that we need
                int first = xExp / BitsPerIteration;
                int resultExp = xExp - (first * BitsPerIteration);

                // 120 is the theoretical maximum number of bits (actually
                // 115 for IEEE single precision) that we need to extract
                // from the middle of (2 / PI) to compute the reduced argument
                // accurately enough for our purposes

                int last = first + (120 / BitsPerIteration);

                // Unroll the loop. This is only correct because we know that bitsper is fixed as 36.

                long* result = stackalloc long[10];
                long u, carry;

                result[4] = 0;
                u = PiBits[last] * ux;

                result[3] = u & mask;
                carry = u >> BitsPerIteration;
                u = PiBits[last - 1] * ux + carry;

                result[2] = u & mask;
                carry = u >> BitsPerIteration;
                u = PiBits[last - 2] * ux + carry;

                result[1] = u & mask;
                carry = u >> BitsPerIteration;
                u = PiBits[first] * ux + carry;

                result[0] = u & mask;

                // Reconstruct the result
                int ltb = (int)((((result[0] << BitsPerIteration) | result[1]) >> (BitsPerIteration - 1 - resultExp)) & 7);

                long mantissa;
                long nextBits;

                // determ says whether the fractional part is >= 0.5
                bool determ = (ltb & 1) != 0;

                int i = 1;

                if (determ)
                {
                    // The mantissa is >= 0.5. We want to subtract it from 1.0 by negating all the bits
                    region = ((ltb >> 1) + 1) & 3;

                    mantissa = 1;
                    mantissa = ~(result[1]) & ((mantissa << (BitsPerIteration - resultExp)) - 1);

                    while (mantissa < 0x0000000000010000)
                    {
                        i++;
                        mantissa = (mantissa << BitsPerIteration) | (~(result[i]) & mask);
                    }

                    nextBits = (~(result[i + 1]) & mask);
                }
                else
                {
                    region = (ltb >> 1);

                    mantissa = 1;
                    mantissa = result[1] & ((mantissa << (BitsPerIteration - resultExp)) - 1);

                    while (mantissa < 0x0000000000010000)
                    {
                        i++;
                        mantissa = (mantissa << BitsPerIteration) | result[i];
                    }

                    nextBits = result[i + 1];
                }

                // Normalize the mantissa.
                // The shift value 6 here, determined by trial and error, seems to give optimal speed.

                int bc = 0;

                while (mantissa < 0x0000400000000000)
                {
                    bc += 6;
                    mantissa <<= 6;
                }

                while (mantissa < 0x0010000000000000)
                {
                    bc++;
                    mantissa <<= 1;
                }

                mantissa |= nextBits >> (BitsPerIteration - bc);

                int rExp = 52 + resultExp - bc - i * BitsPerIteration;

                // Put the result exponent rexp onto the mantissa pattern
                u = (rExp + 1023L) << 52;
                ux = (mantissa & 0x000FFFFFFFFFFFFF) | u;

                if (determ)
                {
                    // If we negated the mantissa we negate x too
                    ux |= unchecked((long)(0x8000000000000000));
                }

                x = BitConverter.Int64BitsToDouble(ux);

                // x is a double precision version of the fractional part of
                // (x * (2 / PI)). Multiply x by (PI / 2) in double precision
                // to get the reduced result.

                return x * PiOverTwo;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double SinTaylorSeriesOneIteration(double x1)
            {
                // x - (x^3 / 3!)

                double x2 = x1 * x1;
                double x3 = x2 * x1;

                return x1 + (x3 * S1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static double SinTaylorSeriesFourIterations(double x1)
            {
                // x - (x^3 / 3!) + (x^5 / 5!) - (x^7 / 7!) + (x^9 / 9!)

                double x2 = x1 * x1;
                double x3 = x2 * x1;
                double x4 = x2 * x2;

                return x1 + ((S1 + (x2 * S2) + (x4 * (S3 + (x2 * S4)))) * x3);
            }
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Sinh(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Sqrt(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Tan(float x);

        [Intrinsic]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float Tanh(float x);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float FMod(float x, float y);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe float ModF(float x, float* intptr);
    }
}
