// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IFloatingPoint<TSelf>
        : ISignedNumber<TSelf>
        where TSelf : IFloatingPoint<TSelf>
    {
        static abstract TSelf E { get; }

        static abstract TSelf Epsilon { get; }

        static abstract TSelf NaN { get; }

        static abstract TSelf NegativeInfinity { get; }

        static abstract TSelf Pi { get; }

        static abstract TSelf PositiveInfinity { get; }

        static abstract TSelf Tau { get; }

        static abstract TSelf Acos(TSelf x);

        static abstract TSelf Acosh(TSelf x);

        static abstract TSelf Asin(TSelf x);

        static abstract TSelf Asinh(TSelf x);

        static abstract TSelf Atan(TSelf x);

        static abstract TSelf Atan2(TSelf y, TSelf x);

        static abstract TSelf Atanh(TSelf x);

        static abstract TSelf BitIncrement(TSelf x);

        static abstract TSelf BitDecrement(TSelf x);

        static abstract TSelf Cbrt(TSelf x);

        static abstract TSelf Ceiling(TSelf x);

        static abstract TSelf CopySign(TSelf x, TSelf y);

        static abstract TSelf Cos(TSelf x);

        static abstract TSelf Cosh(TSelf x);

        static abstract TSelf Exp(TSelf x);

        static abstract TSelf Floor(TSelf x);

        static abstract TSelf FusedMultiplyAdd(TSelf x, TSelf y, TSelf z);

        static abstract TSelf IEEERemainder(TSelf x, TSelf y);

        static abstract int ILogB(TSelf x);

        static abstract bool IsFinite(TSelf value);

        static abstract bool IsInfinity(TSelf value);

        static abstract bool IsNaN(TSelf value);

        static abstract bool IsNegative(TSelf value);

        static abstract bool IsNegativeInfinity(TSelf value);

        static abstract bool IsNormal(TSelf value);

        static abstract bool IsPositiveInfinity(TSelf value);

        static abstract bool IsSubnormal(TSelf value);

        static abstract TSelf Log(TSelf x);

        static abstract TSelf Log(TSelf x, TSelf newBase);

        static abstract TSelf Log2(TSelf x);

        static abstract TSelf Log10(TSelf x);

        static abstract TSelf MaxMagnitude(TSelf x, TSelf y);

        static abstract TSelf MinMagnitude(TSelf x, TSelf y);

        static abstract TSelf Pow(TSelf x, TSelf y);

        static abstract TSelf Round(TSelf x);

        static abstract TSelf Round(TSelf x, int digits);

        static abstract TSelf Round(TSelf x, MidpointRounding mode);

        static abstract TSelf Round(TSelf x, int digits, MidpointRounding mode);

        static abstract TSelf ScaleB(TSelf x, int n);

        static abstract TSelf Sin(TSelf x);

        static abstract TSelf Sinh(TSelf x);

        static abstract TSelf Sqrt(TSelf x);

        static abstract TSelf Tan(TSelf x);

        static abstract TSelf Tanh(TSelf x);

        static abstract TSelf Truncate(TSelf x);

        // static abstract TSelf AcosPi(TSelf x);
        //
        // static abstract TSelf AsinPi(TSelf x);
        //
        // static abstract TSelf AtanPi(TSelf x);
        //
        // static abstract TSelf Atan2Pi(TSelf y, TSelf x);
        //
        // static abstract TSelf Compound(TSelf x, TSelf n);
        //
        // static abstract TSelf CosPi(TSelf x);
        //
        // static abstract TSelf ExpM1(TSelf x);
        //
        // static abstract TSelf Exp2(TSelf x);
        //
        // static abstract TSelf Exp2M1(TSelf x);
        //
        // static abstract TSelf Exp10(TSelf x);
        //
        // static abstract TSelf Exp10M1(TSelf x);
        //
        // static abstract TSelf Hypot(TSelf x, TSelf y);
        //
        // static abstract TSelf LogP1(TSelf x);
        //
        // static abstract TSelf Log2P1(TSelf x);
        //
        // static abstract TSelf Log10P1(TSelf x);
        //
        // static abstract TSelf MaxMagnitudeNumber(TSelf x, TSelf y);
        //
        // static abstract TSelf MaxNumber(TSelf x, TSelf y);
        //
        // static abstract TSelf MinMagnitudeNumber(TSelf x, TSelf y);
        //
        // static abstract TSelf MinNumber(TSelf x, TSelf y);
        //
        // static abstract TSelf Root(TSelf x, TSelf n);
        //
        // static abstract TSelf SinPi(TSelf x);
        //
        // static abstract TSelf TanPi(TSelf x);
    }

    [RequiresPreviewFeatures]
    public interface IBinaryFloatingPoint<TSelf>
        : IBinaryNumber<TSelf>,
          IFloatingPoint<TSelf>
        where TSelf : IBinaryFloatingPoint<TSelf>
    {
    }
}
