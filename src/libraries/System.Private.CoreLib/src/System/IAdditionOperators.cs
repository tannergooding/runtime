// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IAdditionOperators<TSelf, TOther, TResult>
        where TSelf : IAdditionOperators<TSelf, TOther, TResult>
    {
        static abstract TResult operator +(TSelf left, TOther right);

        [SpecialName]
        static abstract TResult op_AdditionChecked(TSelf left, TOther right);
    }
}
