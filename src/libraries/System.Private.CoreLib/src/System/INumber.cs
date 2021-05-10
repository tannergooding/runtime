// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface INumber<TSelf>
        : IAdditionOperators<TSelf, TSelf, TSelf>,
          IAdditiveIdentity<TSelf, TSelf>,
          IComparisonOperators<TSelf, TSelf>,   // implies IEquatableOperators<TSelf, TSelf>
          IDecrementOperators<TSelf>,
          IDivisionOperators<TSelf, TSelf, TSelf>,
          IIncrementOperators<TSelf>,
          IModulusOperators<TSelf, TSelf, TSelf>,
          IMultiplicativeIdentity<TSelf, TSelf>,
          IMultiplyOperators<TSelf, TSelf, TSelf>,
          ISpanFormattable,                     // implies IFormattable
          ISpanParseable<TSelf>,                // implies IParseable<TSelf>
          ISubtractionOperators<TSelf, TSelf, TSelf>,
          IUnaryNegationOperators<TSelf, TSelf>,
          IUnaryPlusOperators<TSelf, TSelf>
        where TSelf : INumber<TSelf>
    {
        static abstract TSelf One { get; }

        static abstract TSelf Zero { get; }

        static abstract TSelf Abs(TSelf value);

        static abstract TSelf Clamp(TSelf value, TSelf min, TSelf max);

        static abstract (TSelf Quotient, TSelf Remainder) DivRem(TSelf left, TSelf right);

        static abstract TSelf Max(TSelf x, TSelf y);

        static abstract TSelf Min(TSelf x, TSelf y);

        static abstract TSelf Parse(string s, NumberStyles style, IFormatProvider? provider);

        static abstract TSelf Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider);

        static abstract TSelf Sign(TSelf value);

        static abstract bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out TSelf result);

        static abstract bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out TSelf result);
    }

    [RequiresPreviewFeatures]
    public interface IBinaryNumber<TSelf>
        : IBitwiseOperators<TSelf, TSelf, TSelf>,
          INumber<TSelf>
        where TSelf : IBinaryNumber<TSelf>
    {
        static abstract bool IsPow2(TSelf value);

        static abstract TSelf Log2(TSelf value);
    }

    [RequiresPreviewFeatures]
    public interface ISignedNumber<TSelf>
        : INumber<TSelf>,
          IUnaryNegationOperators<TSelf, TSelf>
        where TSelf : ISignedNumber<TSelf>
    {
    }

    [RequiresPreviewFeatures]
    public interface IUnsignedNumber<TSelf>
        : INumber<TSelf>
        where TSelf : IUnsignedNumber<TSelf>
    {
    }
}
