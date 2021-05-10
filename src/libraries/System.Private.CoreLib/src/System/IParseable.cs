// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System
{
    [RequiresPreviewFeatures]
    public interface IParseable<TSelf>
        where TSelf : IParseable<TSelf>
    {
        static abstract TSelf Parse(string s, IFormatProvider? provider);

        static abstract bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TSelf result);
    }
}
