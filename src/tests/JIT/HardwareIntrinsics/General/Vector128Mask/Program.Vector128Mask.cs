// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Xunit;

namespace JIT.HardwareIntrinsics.General
{
    public static partial class Program
    {
        [Fact]
        public static void Vector128Mask_AndNot_Byte()
        {
            var result = Vector128Mask.AndNot(
                Vector128Mask.Create<byte>(TestLibrary.Generator.GetUInt16()),
                Vector128Mask.Create<byte>(TestLibrary.Generator.GetUInt16())
            );
        }

        [Fact]
        public static void Vector256Mask_AndNot_Byte()
        {
            var result = Vector256Mask.AndNot(
                Vector256Mask.Create<byte>(TestLibrary.Generator.GetUInt32()),
                Vector256Mask.Create<byte>(TestLibrary.Generator.GetUInt32())
            );
        }

        [Fact]
        public static void Vector512Mask_AndNot_Byte()
        {
            var result = Vector512Mask.AndNot(
                Vector512Mask.Create<byte>(TestLibrary.Generator.GetUInt64()),
                Vector512Mask.Create<byte>(TestLibrary.Generator.GetUInt64())
            );
        }
    }
}
