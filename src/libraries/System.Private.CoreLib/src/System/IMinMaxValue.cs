// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IMinMaxValue<TSelf>
        where TSelf : IMinMaxValue<TSelf>
    {
        static abstract TSelf MinValue { get; }

        static abstract TSelf MaxValue { get; }
    }
}
