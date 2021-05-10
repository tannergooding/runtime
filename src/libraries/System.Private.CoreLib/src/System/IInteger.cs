// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IBinaryInteger<TSelf>
        : IBinaryNumber<TSelf>,
          IShiftOperators<TSelf, TSelf, TSelf>
        where TSelf : IBinaryInteger<TSelf>
    {
        static abstract TSelf LeadingZeroCount(TSelf value);

        static abstract TSelf PopCount(TSelf value);

        static abstract TSelf RotateLeft(TSelf value, TSelf rotateAmount);

        static abstract TSelf RotateRight(TSelf value, TSelf rotateAmount);

        static abstract TSelf TrailingZeroCount(TSelf value);
    }
}
