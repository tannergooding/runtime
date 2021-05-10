// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IUnaryPlusOperators<TSelf, TResult>
        where TSelf : IUnaryPlusOperators<TSelf, TResult>
    {
        static abstract TResult operator +(TSelf value);

        [SpecialName]
        static abstract TResult op_UnaryPlusChecked(TSelf value);
    }
}
