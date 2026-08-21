// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Intrinsics;

namespace System.Numerics.Tensors
{
    public static partial class TensorPrimitives
    {
        /// <summary>Computes the element-wise natural (base <c>e</c>) logarithm of numbers in the specified tensor plus 1.</summary>
        /// <param name="x">The tensor, represented as a span.</param>
        /// <param name="destination">The destination tensor, represented as a span.</param>
        /// <exception cref="ArgumentException">Destination is too short.</exception>
        /// <exception cref="ArgumentException"><paramref name="x"/> and <paramref name="destination"/> reference overlapping memory locations and do not begin at the same location.</exception>
        /// <remarks>
        /// <para>
        /// This method effectively computes <c><paramref name="destination" />[i] = <typeparamref name="T"/>.LogP1(<paramref name="x" />[i])</c>.
        /// </para>
        /// <para>
        /// If a value equals 0, the result stored into the corresponding destination location is the same signed zero.
        /// If a value is less than -1 or equal to <see cref="IFloatingPointIeee754{TSelf}.NaN"/>, the result stored into the corresponding destination location is set to NaN.
        /// If a value equals -1, the result stored into the corresponding destination location is set to <see cref="IFloatingPointIeee754{TSelf}.NegativeInfinity"/>.
        /// If a value is positive infinity, the result stored into the corresponding destination location is set to <see cref="IFloatingPointIeee754{TSelf}.PositiveInfinity"/>.
        /// Otherwise, its natural logarithm plus 1 is stored into the corresponding destination location.
        /// </para>
        /// <para>
        /// This method may call into the underlying C runtime or employ instructions specific to the current architecture. Exact results may differ between different
        /// operating systems or architectures.
        /// </para>
        /// </remarks>
        public static void LogP1<T>(ReadOnlySpan<T> x, Span<T> destination)
            where T : ILogarithmicFunctions<T>
        {
            if (typeof(T) == typeof(Half) && TryUnaryInvokeHalfAsInt16<T, LogP1Operator<float>>(x, destination))
            {
                return;
            }

            InvokeSpanIntoSpan<T, LogP1Operator<T>>(x, destination);
        }

        /// <summary>T.LogP1(x)</summary>
        private readonly struct LogP1Operator<T> : IUnaryOperator<T, T>
            where T : ILogarithmicFunctions<T>
        {
            public static bool Vectorizable => LogOperator<T>.Vectorizable;
            public static T Invoke(T x) => T.LogP1(x);

            public static Vector128<T> Invoke(Vector128<T> x)
            {
                if (typeof(T) == typeof(float))
                {
                    Vector128<float> xf = x.As<T, float>();
                    Vector128<float> tiny = Vector128.Create(5.960464477539063E-08f);

                    if (Vector128.LessThanAll(Vector128.Abs(xf), tiny))
                    {
                        return x;
                    }

                    Vector128<double> lower = Vector128.Log(Vector128<double>.One + Vector128.WidenLower(xf));
                    Vector128<double> upper = Vector128.Log(Vector128<double>.One + Vector128.WidenUpper(xf));
                    Vector128<float> result = Vector128.Narrow(lower, upper);
                    Vector128<float> tinyMask = Vector128.LessThan(Vector128.Abs(xf), tiny);
                    return Vector128.ConditionalSelect(tinyMask, xf, result).As<float, T>();
                }

                if (typeof(T) == typeof(double))
                {
                    return InvokeDouble(x.As<T, double>()).As<double, T>();
                }

                throw new NotSupportedException();
            }

            public static Vector256<T> Invoke(Vector256<T> x)
            {
                if (typeof(T) == typeof(float))
                {
                    Vector256<float> xf = x.As<T, float>();
                    Vector256<float> tiny = Vector256.Create(5.960464477539063E-08f);

                    if (Vector256.LessThanAll(Vector256.Abs(xf), tiny))
                    {
                        return x;
                    }

                    Vector256<double> lower = Vector256.Log(Vector256<double>.One + Vector256.WidenLower(xf));
                    Vector256<double> upper = Vector256.Log(Vector256<double>.One + Vector256.WidenUpper(xf));
                    Vector256<float> result = Vector256.Narrow(lower, upper);
                    Vector256<float> tinyMask = Vector256.LessThan(Vector256.Abs(xf), tiny);
                    return Vector256.ConditionalSelect(tinyMask, xf, result).As<float, T>();
                }

                if (typeof(T) == typeof(double))
                {
                    return InvokeDouble(x.As<T, double>()).As<double, T>();
                }

                throw new NotSupportedException();
            }

            public static Vector512<T> Invoke(Vector512<T> x)
            {
                if (typeof(T) == typeof(float))
                {
                    Vector512<float> xf = x.As<T, float>();
                    Vector512<float> tiny = Vector512.Create(5.960464477539063E-08f);

                    if (Vector512.LessThanAll(Vector512.Abs(xf), tiny))
                    {
                        return x;
                    }

                    Vector512<double> lower = Vector512.Log(Vector512<double>.One + Vector512.WidenLower(xf));
                    Vector512<double> upper = Vector512.Log(Vector512<double>.One + Vector512.WidenUpper(xf));
                    Vector512<float> result = Vector512.Narrow(lower, upper);
                    Vector512<float> tinyMask = Vector512.LessThan(Vector512.Abs(xf), tiny);
                    return Vector512.ConditionalSelect(tinyMask, xf, result).As<float, T>();
                }

                if (typeof(T) == typeof(double))
                {
                    return InvokeDouble(x.As<T, double>()).As<double, T>();
                }

                throw new NotSupportedException();
            }

            private static Vector128<double> InvokeDouble(Vector128<double> x)
            {
                Vector128<double> ax = Vector128.Abs(x);

                if (Vector128.LessThanAll(ax, Vector128.Create(1.1102230246251565E-16)))
                {
                    return x;
                }

                Vector128<double> result;
                Vector128<double> acceptedMask;

                if (Vector128.LessThanAll(ax, Vector128.Create(0.0625)))
                {
                    result = InvokeDoubleSmall(x, out acceptedMask);
                }
                else if (Vector128.GreaterThanOrEqualAll(ax, Vector128.Create(0.0625)))
                {
                    result = InvokeDoubleGeneral(x, out acceptedMask);
                }
                else
                {
                    Vector128<double> smallResult = InvokeDoubleSmall(x, out Vector128<double> smallAcceptedMask);
                    Vector128<double> generalResult = InvokeDoubleGeneral(x, out Vector128<double> generalAcceptedMask);
                    Vector128<double> smallMask = Vector128.LessThan(ax, Vector128.Create(0.0625));
                    result = Vector128.ConditionalSelect(smallMask, smallResult, generalResult);
                    acceptedMask = Vector128.BitwiseOr(smallAcceptedMask, generalAcceptedMask);
                }

                Vector128<double> repairMask = Vector128.OnesComplement(acceptedMask);

                if (!Vector128.EqualsAll(repairMask, Vector128<double>.Zero))
                {
                    for (int i = 0; i < Vector128<double>.Count; i++)
                    {
                        if (repairMask[i] != 0.0)
                        {
                            result = result.WithElement(i, double.LogP1(x[i]));
                        }
                    }
                }

                return result;
            }

            private static Vector256<double> InvokeDouble(Vector256<double> x)
            {
                Vector256<double> ax = Vector256.Abs(x);

                if (Vector256.LessThanAll(ax, Vector256.Create(1.1102230246251565E-16)))
                {
                    return x;
                }

                Vector256<double> result;
                Vector256<double> acceptedMask;

                if (Vector256.LessThanAll(ax, Vector256.Create(0.0625)))
                {
                    result = InvokeDoubleSmall(x, out acceptedMask);
                }
                else if (Vector256.GreaterThanOrEqualAll(ax, Vector256.Create(0.0625)))
                {
                    result = InvokeDoubleGeneral(x, out acceptedMask);
                }
                else
                {
                    Vector256<double> smallResult = InvokeDoubleSmall(x, out Vector256<double> smallAcceptedMask);
                    Vector256<double> generalResult = InvokeDoubleGeneral(x, out Vector256<double> generalAcceptedMask);
                    Vector256<double> smallMask = Vector256.LessThan(ax, Vector256.Create(0.0625));
                    result = Vector256.ConditionalSelect(smallMask, smallResult, generalResult);
                    acceptedMask = Vector256.BitwiseOr(smallAcceptedMask, generalAcceptedMask);
                }

                Vector256<double> repairMask = Vector256.OnesComplement(acceptedMask);

                if (!Vector256.EqualsAll(repairMask, Vector256<double>.Zero))
                {
                    for (int i = 0; i < Vector256<double>.Count; i++)
                    {
                        if (repairMask[i] != 0.0)
                        {
                            result = result.WithElement(i, double.LogP1(x[i]));
                        }
                    }
                }

                return result;
            }

            private static Vector512<double> InvokeDouble(Vector512<double> x)
            {
                Vector256<double> lower = InvokeDouble(x.GetLower());
                Vector256<double> upper = InvokeDouble(x.GetUpper());
                return Vector512.Create(lower, upper);
            }

            private static Vector128<double> InvokeDoubleSmall(Vector128<double> x, out Vector128<double> acceptedMask)
            {
                // This code is based on log1p from the CORE-MATH project.
                // Copyright (c) 2024-2025 Alexei Sibidanov.
                //
                // Licensed under the MIT License.
                // See THIRD-PARTY-NOTICES.TXT for the full license text

                Vector128<double> ax = Vector128.Abs(x);
                Vector128<double> x2 = x * x;

                Vector128<double> high0 = x;
                Vector128<double> low00 = x2 * (Vector128.Create(-0.50000000000005163) + (x * Vector128.Create(0.33333333333338494)));
                Vector128<double> low01 = x2 * (
                    (Vector128.Create(-0.49999999999999983) + (x * Vector128.Create(0.33333333333333309)))
                    + (x2 * (Vector128.Create(-0.25000001241764208) + (x * Vector128.Create(0.20000001241526807)))));
                Vector128<double> low0 = Vector128.ConditionalSelect(
                    Vector128.LessThan(ax, Vector128.Create(5.246838554739952E-07)),
                    low00,
                    low01);
                Vector128<double> error0 = x * Vector128.Create(7.453889935837843E-20);

                Vector128<double> halfNegativeX = x * Vector128.Create(-0.5);
                Vector128<double> high1 = Vector128.FusedMultiplyAdd(halfNegativeX, x, x);
                Vector128<double> low1 = Vector128.FusedMultiplyAdd(halfNegativeX, x, x - high1);
                Vector128<double> x3 = x2 * x;
                Vector128<double> x4 = x2 * x2;
                Vector128<double> poly = (
                    (Vector128.Create(0.33333333333333331) + (x * Vector128.Create(-0.25)))
                    + (x2 * (Vector128.Create(0.20000000000001175) + (x * Vector128.Create(-0.16666666666668725)))))
                    + (x4 * (
                        ((Vector128.Create(0.14285714283910433) + (x * Vector128.Create(-0.1249999999746985)))
                        + (x2 * (Vector128.Create(0.11111112429520291) + (x * Vector128.Create(-0.10000001594045592)))))
                        + (x4 * (
                            (Vector128.Create(0.090904146827106874) + (x * Vector128.Create(-0.083327964855293354)))
                            + (x2 * (Vector128.Create(0.077841308971290935) + (x * Vector128.Create(-0.072347204516044761))))))));
                low1 += x3 * poly;
                Vector128<double> error1 = x3 * Vector128.Create(3.5041414214731503E-16);

                Vector128<double> verySmallMask = Vector128.LessThan(ax, Vector128.Create(0.000244140625));
                Vector128<double> high = Vector128.ConditionalSelect(verySmallMask, high0, high1);
                Vector128<double> low = Vector128.ConditionalSelect(verySmallMask, low0, low1);
                Vector128<double> error = Vector128.ConditionalSelect(verySmallMask, error0, error1);
                Vector128<double> lower = high + (low - error);
                Vector128<double> upper = high + (low + error);
                Vector128<double> tinyMask = Vector128.LessThan(ax, Vector128.Create(1.1102230246251565E-16));
                Vector128<double> smallMask = Vector128.LessThan(ax, Vector128.Create(0.0625));
                acceptedMask = Vector128.BitwiseOr(tinyMask, Vector128.BitwiseAnd(smallMask, Vector128.Equals(lower, upper)));
                return Vector128.ConditionalSelect(tinyMask, x, lower);
            }

            private static Vector256<double> InvokeDoubleSmall(Vector256<double> x, out Vector256<double> acceptedMask)
            {
                // This code is based on log1p from the CORE-MATH project.
                // Copyright (c) 2024-2025 Alexei Sibidanov.
                //
                // Licensed under the MIT License.
                // See THIRD-PARTY-NOTICES.TXT for the full license text

                Vector256<double> ax = Vector256.Abs(x);
                Vector256<double> x2 = x * x;

                Vector256<double> high0 = x;
                Vector256<double> low00 = x2 * (Vector256.Create(-0.50000000000005163) + (x * Vector256.Create(0.33333333333338494)));
                Vector256<double> low01 = x2 * (
                    (Vector256.Create(-0.49999999999999983) + (x * Vector256.Create(0.33333333333333309)))
                    + (x2 * (Vector256.Create(-0.25000001241764208) + (x * Vector256.Create(0.20000001241526807)))));
                Vector256<double> low0 = Vector256.ConditionalSelect(
                    Vector256.LessThan(ax, Vector256.Create(5.246838554739952E-07)),
                    low00,
                    low01);
                Vector256<double> error0 = x * Vector256.Create(7.453889935837843E-20);

                Vector256<double> halfNegativeX = x * Vector256.Create(-0.5);
                Vector256<double> high1 = Vector256.FusedMultiplyAdd(halfNegativeX, x, x);
                Vector256<double> low1 = Vector256.FusedMultiplyAdd(halfNegativeX, x, x - high1);
                Vector256<double> x3 = x2 * x;
                Vector256<double> x4 = x2 * x2;
                Vector256<double> poly = (
                    (Vector256.Create(0.33333333333333331) + (x * Vector256.Create(-0.25)))
                    + (x2 * (Vector256.Create(0.20000000000001175) + (x * Vector256.Create(-0.16666666666668725)))))
                    + (x4 * (
                        ((Vector256.Create(0.14285714283910433) + (x * Vector256.Create(-0.1249999999746985)))
                        + (x2 * (Vector256.Create(0.11111112429520291) + (x * Vector256.Create(-0.10000001594045592)))))
                        + (x4 * (
                            (Vector256.Create(0.090904146827106874) + (x * Vector256.Create(-0.083327964855293354)))
                            + (x2 * (Vector256.Create(0.077841308971290935) + (x * Vector256.Create(-0.072347204516044761))))))));
                low1 += x3 * poly;
                Vector256<double> error1 = x3 * Vector256.Create(3.5041414214731503E-16);

                Vector256<double> verySmallMask = Vector256.LessThan(ax, Vector256.Create(0.000244140625));
                Vector256<double> high = Vector256.ConditionalSelect(verySmallMask, high0, high1);
                Vector256<double> low = Vector256.ConditionalSelect(verySmallMask, low0, low1);
                Vector256<double> error = Vector256.ConditionalSelect(verySmallMask, error0, error1);
                Vector256<double> lower = high + (low - error);
                Vector256<double> upper = high + (low + error);
                Vector256<double> tinyMask = Vector256.LessThan(ax, Vector256.Create(1.1102230246251565E-16));
                Vector256<double> smallMask = Vector256.LessThan(ax, Vector256.Create(0.0625));

                acceptedMask = Vector256.BitwiseOr(tinyMask, Vector256.BitwiseAnd(smallMask, Vector256.Equals(lower, upper)));
                return Vector256.ConditionalSelect(tinyMask, x, lower);
            }

            private static Vector128<double> InvokeDoubleGeneral(Vector128<double> x, out Vector128<double> acceptedMask)
            {
                Vector128<double> one = Vector128<double>.One;
                Vector128<double> value = one + x;
                Vector128<double> virtualX = value - one;
                Vector128<double> valueLow = (one - (value - virtualX)) + (x - virtualX);
                Vector128<long> exponent = Vector128.ShiftRightLogical(
                    Vector128.BitwiseAnd(value.AsUInt64(), Vector128.Create(0x7FF0_0000_0000_0000UL)),
                    52).AsInt64() - Vector128.Create(1023L);

                value = Vector128.BitwiseOr(
                    Vector128.BitwiseAnd(value.AsUInt64(), Vector128.Create(0x000F_FFFF_FFFF_FFFFUL)),
                    Vector128.Create(0x3FF0_0000_0000_0000UL)).AsDouble();
                Vector128<ulong> scaleBits = Vector128.ShiftLeft((Vector128.Create(1023L) - exponent).AsUInt64(), 52);
                valueLow *= scaleBits.AsDouble();

                Vector128<double> reduceMask = Vector128.GreaterThan(value, Vector128.Create(1.4142135623730951));
                value = Vector128.ConditionalSelect(reduceMask, value * Vector128.Create(0.5), value);
                valueLow = Vector128.ConditionalSelect(reduceMask, valueLow * Vector128.Create(0.5), valueLow);
                exponent -= reduceMask.AsInt64();

                Vector128<double> numerator = value - one;
                Vector128<double> denominator = value + one;
                Vector128<double> virtualOne = denominator - value;
                Vector128<double> denominatorLow = (value - (denominator - virtualOne)) + (one - virtualOne) + valueLow;
                Vector128<double> reducedHigh = numerator / denominator;
                Vector128<double> remainder = Vector128.FusedMultiplyAdd(-reducedHigh, denominator, numerator)
                                            + valueLow
                                            - (reducedHigh * denominatorLow);
                Vector128<double> reducedLow = remainder / denominator;
                Vector128<double> z2 = reducedHigh * reducedHigh;

                Vector128<double> poly = Vector128.FusedMultiplyAdd(Vector128.Create(0.037037037037037035), z2, Vector128.Create(0.04));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.043478260869565216));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.047619047619047616));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.05263157894736842));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.058823529411764705));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.06666666666666667));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.07692307692307693));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.09090909090909091));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.1111111111111111));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.14285714285714285));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.2));
                poly = Vector128.FusedMultiplyAdd(poly, z2, Vector128.Create(0.3333333333333333));

                Vector128<double> z3 = reducedHigh * z2;
                Vector128<double> z3Low = Vector128.FusedMultiplyAdd(reducedHigh, z2, -z3);
                Vector128<double> term = z3 * poly;
                Vector128<double> termLow = Vector128.FusedMultiplyAdd(z3, poly, -term) + (z3Low * poly);
                AddDoubleDouble(reducedHigh, reducedLow, term, termLow, out Vector128<double> logarithmHigh, out Vector128<double> logarithmLow);
                MultiplyDoubleDouble(
                    logarithmHigh,
                    logarithmLow,
                    Vector128.Create(2.0),
                    Vector128<double>.Zero,
                    out logarithmHigh,
                    out logarithmLow);

                Vector128<double> exponentDouble = Vector128.ConvertToDouble(exponent);
                MultiplyDoubleDouble(
                    exponentDouble,
                    Vector128<double>.Zero,
                    Vector128.Create(0.6931471805599453),
                    Vector128.Create(2.3190468138462996E-17),
                    out Vector128<double> exponentHigh,
                    out Vector128<double> exponentLow);
                AddDoubleDouble(
                    logarithmHigh,
                    logarithmLow,
                    exponentHigh,
                    exponentLow,
                    out logarithmHigh,
                    out logarithmLow);

                Vector128<long> absoluteExponent = Vector128.Abs(exponent);
                Vector128<double> error = Vector128.ConditionalSelect(
                    Vector128.GreaterThanOrEqual(absoluteExponent, Vector128.Create(32L)).AsDouble(),
                    Vector128.Create(6.938893903907228E-18),
                    Vector128.Create(3.469446951953614E-18));
                Vector128<double> lower = logarithmHigh + (logarithmLow - error);
                Vector128<double> upper = logarithmHigh + (logarithmLow + error);
                Vector128<double> finiteDomainMask = Vector128.BitwiseAnd(
                    Vector128.GreaterThan(x, Vector128.Create(-1.0)),
                    Vector128.LessThan(Vector128.Abs(x), Vector128.Create(double.PositiveInfinity)));
                Vector128<double> supportedExponentMask = Vector128.LessThan(exponent, Vector128.Create(1023L)).AsDouble();
                Vector128<double> generalMask = Vector128.GreaterThanOrEqual(Vector128.Abs(x), Vector128.Create(0.0625));

                acceptedMask = Vector128.BitwiseAnd(
                    Vector128.BitwiseAnd(finiteDomainMask, supportedExponentMask),
                    Vector128.BitwiseAnd(generalMask, Vector128.Equals(lower, upper)));
                return lower;
            }

            private static Vector256<double> InvokeDoubleGeneral(Vector256<double> x, out Vector256<double> acceptedMask)
            {
                Vector256<double> one = Vector256<double>.One;
                Vector256<double> value = one + x;
                Vector256<double> virtualX = value - one;
                Vector256<double> valueLow = (one - (value - virtualX)) + (x - virtualX);
                Vector256<long> exponent = Vector256.ShiftRightLogical(
                    Vector256.BitwiseAnd(value.AsUInt64(), Vector256.Create(0x7FF0_0000_0000_0000UL)),
                    52).AsInt64() - Vector256.Create(1023L);

                value = Vector256.BitwiseOr(
                    Vector256.BitwiseAnd(value.AsUInt64(), Vector256.Create(0x000F_FFFF_FFFF_FFFFUL)),
                    Vector256.Create(0x3FF0_0000_0000_0000UL)).AsDouble();
                Vector256<ulong> scaleBits = Vector256.ShiftLeft((Vector256.Create(1023L) - exponent).AsUInt64(), 52);
                valueLow *= scaleBits.AsDouble();

                Vector256<double> reduceMask = Vector256.GreaterThan(value, Vector256.Create(1.4142135623730951));
                value = Vector256.ConditionalSelect(reduceMask, value * Vector256.Create(0.5), value);
                valueLow = Vector256.ConditionalSelect(reduceMask, valueLow * Vector256.Create(0.5), valueLow);
                exponent -= reduceMask.AsInt64();

                Vector256<double> numerator = value - one;
                Vector256<double> denominator = value + one;
                Vector256<double> virtualOne = denominator - value;
                Vector256<double> denominatorLow = (value - (denominator - virtualOne)) + (one - virtualOne) + valueLow;
                Vector256<double> reducedHigh = numerator / denominator;
                Vector256<double> remainder = Vector256.FusedMultiplyAdd(-reducedHigh, denominator, numerator)
                                            + valueLow
                                            - (reducedHigh * denominatorLow);
                Vector256<double> reducedLow = remainder / denominator;
                Vector256<double> z2 = reducedHigh * reducedHigh;

                Vector256<double> poly = Vector256.FusedMultiplyAdd(Vector256.Create(0.037037037037037035), z2, Vector256.Create(0.04));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.043478260869565216));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.047619047619047616));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.05263157894736842));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.058823529411764705));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.06666666666666667));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.07692307692307693));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.09090909090909091));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.1111111111111111));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.14285714285714285));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.2));
                poly = Vector256.FusedMultiplyAdd(poly, z2, Vector256.Create(0.3333333333333333));

                Vector256<double> z3 = reducedHigh * z2;
                Vector256<double> z3Low = Vector256.FusedMultiplyAdd(reducedHigh, z2, -z3);
                Vector256<double> term = z3 * poly;
                Vector256<double> termLow = Vector256.FusedMultiplyAdd(z3, poly, -term) + (z3Low * poly);
                AddDoubleDouble(reducedHigh, reducedLow, term, termLow, out Vector256<double> logarithmHigh, out Vector256<double> logarithmLow);
                MultiplyDoubleDouble(
                    logarithmHigh,
                    logarithmLow,
                    Vector256.Create(2.0),
                    Vector256<double>.Zero,
                    out logarithmHigh,
                    out logarithmLow);

                Vector256<double> exponentDouble = Vector256.ConvertToDouble(exponent);
                MultiplyDoubleDouble(
                    exponentDouble,
                    Vector256<double>.Zero,
                    Vector256.Create(0.6931471805599453),
                    Vector256.Create(2.3190468138462996E-17),
                    out Vector256<double> exponentHigh,
                    out Vector256<double> exponentLow);
                AddDoubleDouble(
                    logarithmHigh,
                    logarithmLow,
                    exponentHigh,
                    exponentLow,
                    out logarithmHigh,
                    out logarithmLow);

                Vector256<long> absoluteExponent = Vector256.Abs(exponent);
                Vector256<double> error = Vector256.ConditionalSelect(
                    Vector256.GreaterThanOrEqual(absoluteExponent, Vector256.Create(32L)).AsDouble(),
                    Vector256.Create(6.938893903907228E-18),
                    Vector256.Create(3.469446951953614E-18));
                Vector256<double> lower = logarithmHigh + (logarithmLow - error);
                Vector256<double> upper = logarithmHigh + (logarithmLow + error);
                Vector256<double> finiteDomainMask = Vector256.BitwiseAnd(
                    Vector256.GreaterThan(x, Vector256.Create(-1.0)),
                    Vector256.LessThan(Vector256.Abs(x), Vector256.Create(double.PositiveInfinity)));
                Vector256<double> supportedExponentMask = Vector256.LessThan(exponent, Vector256.Create(1023L)).AsDouble();
                Vector256<double> generalMask = Vector256.GreaterThanOrEqual(Vector256.Abs(x), Vector256.Create(0.0625));

                acceptedMask = Vector256.BitwiseAnd(
                    Vector256.BitwiseAnd(finiteDomainMask, supportedExponentMask),
                    Vector256.BitwiseAnd(generalMask, Vector256.Equals(lower, upper)));
                return lower;
            }

            private static void AddDoubleDouble(
                Vector128<double> leftHigh,
                Vector128<double> leftLow,
                Vector128<double> rightHigh,
                Vector128<double> rightLow,
                out Vector128<double> high,
                out Vector128<double> low)
            {
                Vector128<double> sum = leftHigh + rightHigh;
                Vector128<double> rightVirtual = sum - leftHigh;
                Vector128<double> sumError = (leftHigh - (sum - rightVirtual)) + (rightHigh - rightVirtual);
                Vector128<double> tail = sumError + leftLow + rightLow;
                high = sum + tail;
                low = (sum - high) + tail;
            }

            private static void MultiplyDoubleDouble(
                Vector128<double> leftHigh,
                Vector128<double> leftLow,
                Vector128<double> rightHigh,
                Vector128<double> rightLow,
                out Vector128<double> high,
                out Vector128<double> low)
            {
                Vector128<double> product = leftHigh * rightHigh;
                Vector128<double> productError = Vector128.FusedMultiplyAdd(leftHigh, rightHigh, -product);
                Vector128<double> tail = productError + (leftHigh * rightLow) + (leftLow * rightHigh);
                high = product + tail;
                low = (product - high) + tail;
            }

            private static void AddDoubleDouble(
                Vector256<double> leftHigh,
                Vector256<double> leftLow,
                Vector256<double> rightHigh,
                Vector256<double> rightLow,
                out Vector256<double> high,
                out Vector256<double> low)
            {
                Vector256<double> sum = leftHigh + rightHigh;
                Vector256<double> rightVirtual = sum - leftHigh;
                Vector256<double> sumError = (leftHigh - (sum - rightVirtual)) + (rightHigh - rightVirtual);
                Vector256<double> tail = sumError + leftLow + rightLow;
                high = sum + tail;
                low = (sum - high) + tail;
            }

            private static void MultiplyDoubleDouble(
                Vector256<double> leftHigh,
                Vector256<double> leftLow,
                Vector256<double> rightHigh,
                Vector256<double> rightLow,
                out Vector256<double> high,
                out Vector256<double> low)
            {
                Vector256<double> product = leftHigh * rightHigh;
                Vector256<double> productError = Vector256.FusedMultiplyAdd(leftHigh, rightHigh, -product);
                Vector256<double> tail = productError + (leftHigh * rightLow) + (leftLow * rightHigh);
                high = product + tail;
                low = (product - high) + tail;
            }

        }
    }
}
