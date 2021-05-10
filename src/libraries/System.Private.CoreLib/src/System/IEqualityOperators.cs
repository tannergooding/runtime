// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IEqualityOperators<TSelf, TOther> : IEquatable<TOther>
        where TSelf : IEqualityOperators<TSelf, TOther>
    {
        static abstract bool operator ==(TSelf left, TOther right);

        static abstract bool operator !=(TSelf left, TOther right);
    }
}
