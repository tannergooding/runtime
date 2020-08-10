// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Numerics;

class Runtime_8162
{
    public static int Main()
    {
        bool isHardwareAccelerated = Vector.IsHardwareAccelerated;
        bool reflectionIsHardwareAccelerated = Convert.ToBoolean(typeof(Vector).GetMethod("get_IsHardwareAccelerated").Invoke(null, null));
        return (isHardwareAccelerated == reflectionIsHardwareAccelerated) ? 100 : 0;
    }
}
