// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IComparisonOperators<TSelf, TOther>
        : IComparable,
          IComparable<TOther>,
          IEqualityOperators<TSelf, TOther>
        where TSelf : IComparisonOperators<TSelf, TOther>
    {
        static abstract bool operator <(TSelf left, TOther right);

        static abstract bool operator <=(TSelf left, TOther right);

        static abstract bool operator >(TSelf left, TOther right);

        static abstract bool operator >=(TSelf left, TOther right);
    }
}
