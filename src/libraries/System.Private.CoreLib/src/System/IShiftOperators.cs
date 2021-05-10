// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IShiftOperators<TSelf, TOther, TResult>
        where TSelf : IShiftOperators<TSelf, TOther, TResult>
    {
        [SpecialName]
        static abstract TResult op_LeftShift(TSelf value, TOther shiftAmount);

        [SpecialName]
        static abstract TResult op_RightShift(TSelf value, TOther shiftAmount);

        [SpecialName]
        static abstract TResult op_UnsignedRightShift(TSelf value, TOther shiftAmount);
    }
}
