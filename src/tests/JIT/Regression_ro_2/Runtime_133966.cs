// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using Xunit;

// Unrolled UTF16 comparisons must not move ahead of earlier side effects on the evaluation stack.
public class Runtime_133966
{
    private static int s_sink;
    private static int s_value;
    private static volatile int s_volatileValue;

    // The assignments append local stores while the left operand is still on the importer stack.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadAfterDivision(int x, int y)
    {
        int copy;
        int sum = (x / y) + (copy = s_value);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int DivisionAfterVolatileRead(int x, int y)
    {
        int copy;
        int sum = s_volatileValue + (copy = x / y);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int VolatileReadAfterDivision(int x, int y)
    {
        int copy;
        int sum = (x / y) + (copy = s_volatileValue);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int VolatileReadAfterVolatileRead()
    {
        int copy;
        int sum = s_volatileValue + (copy = s_volatileValue);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int VolatileReadAfterRead()
    {
        int copy;
        int sum = s_value + (copy = s_volatileValue);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadAfterVolatileRead()
    {
        int copy;
        int sum = s_volatileValue + (copy = s_value);
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int OverflowAfterDivision(int x, int y, int z)
    {
        int copy;
        int sum = (x / y) + (copy = checked(z + 1));
        return sum + copy;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int StoreAfterDivision(int x, int y)
    {
        return (x / y) + (s_value = 42);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CallAfterDivision(int x, int y)
    {
        return (x / y) + StoreValue();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int StoreValue()
    {
        s_value = 42;
        return s_value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadBeforeStore()
    {
        return s_value + (s_value = 42);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int AliasedLocalBeforeStore(int value)
    {
        ref int alias = ref value;
        return value + (alias = 42);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int LocalStoreAfterDivision(int x, int y)
    {
        int value = 7;
        try
        {
            return (x / y) + (value = 42);
        }
        catch (DivideByZeroException)
        {
            return value;
        }
    }

    [Theory]
    [InlineData(14, 2, 7)]
    [InlineData(-14, 2, -7)]
    [InlineData(0, 2, 0)]
    public static void SpillInterference(int x, int y, int quotient)
    {
        s_value = 7;
        s_volatileValue = 11;

        Assert.Equal(quotient + 14, ReadAfterDivision(x, y));
        Assert.Equal(11 + (2 * quotient), DivisionAfterVolatileRead(x, y));
        Assert.Equal(quotient + 22, VolatileReadAfterDivision(x, y));
        Assert.Equal(33, VolatileReadAfterVolatileRead());
        Assert.Equal(29, VolatileReadAfterRead());
        Assert.Equal(25, ReadAfterVolatileRead());
        Assert.Equal(quotient + 8, OverflowAfterDivision(x, y, 3));
        Assert.Equal(49, ReadBeforeStore());
        Assert.Equal(42, s_value);
        Assert.Equal(49, AliasedLocalBeforeStore(7));
        Assert.Equal(quotient + 42, LocalStoreAfterDivision(x, y));
        Assert.Equal(quotient + 42, StoreAfterDivision(x, y));
        s_value = 7;
        Assert.Equal(quotient + 42, CallAfterDivision(x, y));
        Assert.Equal(42, s_value);
    }

    [Theory]
    [InlineData(1, 0, typeof(DivideByZeroException))]
    [InlineData(int.MinValue, -1, typeof(OverflowException))]
    public static void SpillExceptionOrdering(int x, int y, Type exceptionType)
    {
        s_value = 7;
        Assert.Throws(exceptionType, () => ReadAfterDivision(x, y));
        Assert.Throws(exceptionType, () => DivisionAfterVolatileRead(x, y));
        Assert.Throws(exceptionType, () => VolatileReadAfterDivision(x, y));
        Assert.Throws(exceptionType, () => OverflowAfterDivision(x, y, int.MaxValue));
        Assert.Throws(exceptionType, () => StoreAfterDivision(x, y));
        Assert.Equal(7, s_value);
        Assert.Throws(exceptionType, () => CallAfterDivision(x, y));
        Assert.Equal(7, s_value);
        Assert.Throws<OverflowException>(() => OverflowAfterDivision(1, 1, int.MaxValue));
        Assert.Equal(7, LocalStoreAfterDivision(1, 0));
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static int EqualsAfterBoundsCheck(string s, int[] a, int i)
    {
        return a[i] + (s.Equals("ab") ? 1 : 0);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static int StartsWithAfterDivision(string s, int x, int y)
    {
        return (x / y) + (s.StartsWith("ab", StringComparison.Ordinal) ? 1 : 0);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static int EndsWithAfterDivision(string s, int x, int y)
    {
        return (x / y) + (s.EndsWith("ab", StringComparison.Ordinal) ? 1 : 0);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
    private static int SpanEqualsAfterDivision(ref ReadOnlySpan<char> s, int x, int y)
    {
        return (x / y) + (s.SequenceEqual("ab") ? 1 : 0);
    }

    [Fact]
    public static void TestEntryPoint()
    {
        int[] a = new int[1];

        // The earlier side effect has to win over the unrolled comparison's null check.
        Assert.Throws<IndexOutOfRangeException>(() => { s_sink = EqualsAfterBoundsCheck(null, a, 5); });
        Assert.Throws<DivideByZeroException>(() => { s_sink = StartsWithAfterDivision(null, 1, 0); });
        Assert.Throws<DivideByZeroException>(() => { s_sink = EndsWithAfterDivision(null, 1, 0); });

        // Also exercise span unrolling when the literal-to-span conversion is not inlined.
        Assert.Throws<DivideByZeroException>(() =>
        {
            s_sink = SpanEqualsAfterDivision(ref Unsafe.NullRef<ReadOnlySpan<char>>(), 1, 0);
        });
        Assert.Throws<NullReferenceException>(() =>
        {
            s_sink = SpanEqualsAfterDivision(ref Unsafe.NullRef<ReadOnlySpan<char>>(), 1, 1);
        });

        // The unrolled comparison itself must still work, and still throw NRE on its own.
        Assert.Throws<NullReferenceException>(() => { s_sink = EqualsAfterBoundsCheck(null, a, 0); });
        Assert.Equal(1, EqualsAfterBoundsCheck("ab", a, 0));
        Assert.Equal(0, EqualsAfterBoundsCheck("ba", a, 0));
        Assert.Equal(3, StartsWithAfterDivision("abc", 2, 1));
        Assert.Equal(2, StartsWithAfterDivision("cba", 2, 1));
        Assert.Equal(3, EndsWithAfterDivision("xab", 2, 1));
        Assert.Equal(2, EndsWithAfterDivision("xba", 2, 1));

        ReadOnlySpan<char> span = "ab";
        Assert.Equal(3, SpanEqualsAfterDivision(ref span, 2, 1));
        span = "ba";
        Assert.Equal(2, SpanEqualsAfterDivision(ref span, 2, 1));
    }
}
