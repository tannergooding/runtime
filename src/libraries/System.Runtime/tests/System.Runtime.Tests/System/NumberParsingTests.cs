// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Numerics;
using System.Text;
using Xunit;

namespace System.Tests;

public class NumberParsingTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(33)]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(1024)]
    public static void TrailingNulls(int count)
    {
        TestInteger<byte>(count);
        TestInteger<sbyte>(count);
        TestInteger<short>(count);
        TestInteger<ushort>(count);
        TestInteger<int>(count);
        TestInteger<uint>(count);
        TestInteger<long>(count);
        TestInteger<ulong>(count);
        TestInteger<nint>(count);
        TestInteger<nuint>(count);
        TestInteger<Int128>(count);
        TestInteger<UInt128>(count);
        TestFloat<BFloat16>(count);
        TestFloat<Half>(count);
        TestFloat<float>(count);
        TestFloat<double>(count);
        TestNumber<decimal>(count);
        TestNumber<Decimal32>(count);
        TestNumber<Decimal64>(count);
        TestNumber<Decimal128>(count);
        TestNumber<BigInteger>(count);
    }

    private static void TestInteger<T>(int count) where T : struct, IBinaryInteger<T>
    {
        TestNumber<T>(count);
        TestRemainder<T>("12", NumberStyles.Integer, CultureInfo.InvariantCulture, count);
        TestRemainder<T>("C", NumberStyles.HexNumber, CultureInfo.InvariantCulture, count);
        TestRemainder<T>("1100", NumberStyles.BinaryNumber, CultureInfo.InvariantCulture, count);
    }

    private static void TestFloat<T>(int count) where T : struct, IBinaryFloatingPointIeee754<T>
    {
        TestNumber<T>(count);
        TestRemainder<T>("0x1.8p3", NumberStyles.HexFloat, CultureInfo.InvariantCulture, count);
        TestRemainder<T>("-0", NumberStyles.Float, CultureInfo.InvariantCulture, count);
        TestRemainder<T>("1e-9999", NumberStyles.Float, CultureInfo.InvariantCulture, count);
        TestRemainder<T>("1e9999", NumberStyles.Float, CultureInfo.InvariantCulture, count);
    }

    private static void TestNumber<T>(int count) where T : struct, INumberBase<T>
    {
        TestRemainder<T>("12", NumberStyles.Float, CultureInfo.InvariantCulture, count);
        TestRemainder<T>(" 12.0 ", NumberStyles.Float, CultureInfo.InvariantCulture, count);

        NumberFormatInfo info = new NumberFormatInfo
        {
            CurrencySymbol = "\u20ac",
            CurrencyDecimalSeparator = ",",
            CurrencyGroupSeparator = "."
        };
        TestRemainder<T>("\u20ac12,0", NumberStyles.Currency, info, count);

        string nulls = new string('\0', count);
        foreach (string text in new[] { nulls, nulls + "x" })
        {
            Assert.False(T.TryParsePartial(text, NumberStyles.Float, CultureInfo.InvariantCulture, out T result, out int consumed));
            Assert.Equal(T.Zero, result);
            Assert.Equal(0, consumed);
            Assert.False(T.TryParsePartial(Encoding.UTF8.GetBytes(text), NumberStyles.Float, CultureInfo.InvariantCulture, out result, out consumed));
            Assert.Equal(T.Zero, result);
            Assert.Equal(0, consumed);
        }
    }

    private static void TestRemainder<T>(string prefix, NumberStyles style, IFormatProvider provider, int count)
        where T : struct, INumberBase<T>
    {
        T expected = T.Parse(prefix, style, provider);
        string number = prefix + new string('\0', count);
        int expectedBytesConsumed = Encoding.UTF8.GetByteCount(number);

        foreach (string suffix in new[] { "", ",", "," + new string('1', 128), "\0", " \0", "x\0", "\ud800" })
        {
            string text = number + suffix;
            byte[] utf8 = Encoding.UTF8.GetBytes(text);
            int extraConsumed = suffix switch
            {
                "\0" => 1,
                " \0" when count == 0 => 2,
                _ => 0
            };
            bool fullSuccess = suffix.Length == 0 || extraConsumed != 0;

            Assert.True(T.TryParsePartial(text, style, provider, out T result, out int consumed));
            Assert.Equal(expected, result);
            Assert.Equal(T.IsNegative(expected), T.IsNegative(result));
            Assert.Equal(number.Length + extraConsumed, consumed);
            Assert.True(T.TryParsePartial(text.AsSpan(), style, provider, out result, out consumed));
            Assert.Equal(expected, result);
            Assert.Equal(T.IsNegative(expected), T.IsNegative(result));
            Assert.Equal(number.Length + extraConsumed, consumed);
            Assert.True(T.TryParsePartial(utf8, style, provider, out result, out consumed));
            Assert.Equal(expected, result);
            Assert.Equal(T.IsNegative(expected), T.IsNegative(result));
            Assert.Equal(expectedBytesConsumed + extraConsumed, consumed);

            Assert.Equal(fullSuccess, T.TryParse(text, style, provider, out result));
            Assert.Equal(fullSuccess ? expected : T.Zero, result);
            Assert.Equal(fullSuccess, T.TryParse(text.AsSpan(), style, provider, out result));
            Assert.Equal(fullSuccess ? expected : T.Zero, result);
            Assert.Equal(fullSuccess, T.TryParse(utf8, style, provider, out result));
            Assert.Equal(fullSuccess ? expected : T.Zero, result);
        }
    }
}
