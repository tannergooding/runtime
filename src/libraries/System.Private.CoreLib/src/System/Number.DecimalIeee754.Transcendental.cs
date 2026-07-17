// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace System
{
    internal static partial class Number
    {
        // Transcendental IEEE 754 decimal operations, evaluated by routing through binary64 (`double`):
        // convert the decimal operand(s) to `double`, apply the corresponding `double` math surface, then
        // convert the result back with the shared correctly-rounded decimal<->double conversions. This is
        // the faithful low-risk base; the extended-precision path is a later refinement.
        //
        // LIMITATION: `double` carries only ~15.9 significant decimal digits and magnitude up to ~1e308.
        // That is exact enough for Decimal32 (7 digits, exponent +/-96), marginal for Decimal64 (16
        // digits, last digit may differ), and insufficient for Decimal128 (34 digits, exponent +/-6144 -
        // only ~16 digits are meaningful and any operand beyond double's range saturates to Inf/0 before
        // the operation is applied). This mirrors what binary libm and Intel's own decimal reference do
        // and is an accepted starting point, refinable by a future extended-precision implementation.
        //
        // Special cases: a NaN operand is canonicalized up front via PropagateNaN (preserving its sign and
        // payload, as the exact ops do). A NaN produced by the operation itself (an invalid operation on a
        // non-NaN operand) is returned as the positive canonical quiet NaN, matching the invalid-operation
        // convention of the existing decimal ops. Infinities and signed zeros are carried through by the
        // conversions, which already follow IEEE for the binary domain.

        internal static TValue UnaryFromDoubleDecimalIeee754<TDecimal, TValue>(TValue decimalBits, Func<double, double> operation)
            where TDecimal : unmanaged, IDecimalIeee754ParseAndFormatInfo<TDecimal, TValue>
            where TValue : unmanaged, IBinaryInteger<TValue>
        {
            if (TDecimal.IsNaN(decimalBits))
            {
                return PropagateNaN<TDecimal, TValue>(decimalBits, decimalBits);
            }

            double x = ConvertDecimalIeee754ToFloat<TDecimal, TValue, double>(decimalBits);
            double result = operation(x);

            if (double.IsNaN(result))
            {
                return TDecimal.NaN;
            }

            return ConvertFloatToDecimalIeee754<double, TDecimal, TValue>(result);
        }
    }
}
