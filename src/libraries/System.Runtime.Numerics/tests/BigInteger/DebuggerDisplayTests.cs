// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Tests;
using Xunit;

namespace System.Numerics.Tests
{
    public class DebuggerDisplayTests
    {
        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsDebuggerTypeProxyAttributeSupported))]
        [InlineData(new uint[] { 0, 0, 1 }, "18446744073709551616", "18446744073709551616")]
        [InlineData(new uint[] { 0, 0, 0, 0, 1 }, "3.40282367e+38", "340282366920938463463374607431768211456")]
        [InlineData(new uint[] { 0, 0x12345678, 0, 0xCC00CC00, 0x80808080 }, "7.33616508e+47", "733616507979605610118788984834443474975600170616")]
        [SkipOnPlatform(TestPlatforms.Browser, "DebuggerDisplayAttribute is stripped on wasm")]
        public void TestDebuggerDisplay(uint[] input, string displayString32, string displayString64)
        {
            nuint[] bits;

            if (Environment.Is64BitProcess)
            {
                bits = new nuint[(input.Length + 1) / 2];

                for (int i = 0; i < bits.Length; i++)
                {
                    int n = i * 2;
                    nuint value = input[n++];

                    if (n < input.Length)
                    {
                        value = (value << 32) | input[n];
                    }
                    bits[i] = value;
                }
            }
            else
            {
                bits = new nuint[input.Length];
                input.AsSpan().CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<nuint, uint>(ref bits[0]), bits.Length));
            }

            string displayString = Environment.Is64BitProcess ? displayString64 : displayString32;

            using (new ThreadCultureChange(CultureInfo.InvariantCulture))
            {
                BigInteger positiveValue = new BigInteger(1, bits);
                Assert.Equal(displayString, DebuggerAttributes.ValidateDebuggerDisplayReferences(positiveValue));

                BigInteger negativeValue = new BigInteger(-1, bits);
                Assert.Equal("-" + displayString, DebuggerAttributes.ValidateDebuggerDisplayReferences(negativeValue));
            }
        }
    }
}
