// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using Xunit;

public static class Runtime_113070
{
    private static readonly ulong[] s_values64 = [0, 1, ulong.MaxValue - 1, ulong.MaxValue];
    private static readonly uint[] s_values32 = [0, 1, uint.MaxValue - 1, uint.MaxValue];
    private static int s_taken;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong Add64(ulong leftLow, ulong leftHigh, ulong rightLow, ulong rightHigh, out ulong high)
    {
        // X64: add
        // X64: adc
        // ARM64: adds
        // ARM64: adc
        ulong low = leftLow + rightLow;
        ulong carry = low < leftLow ? 1UL : 0UL;
        high = leftHigh + rightHigh + carry;
        return low;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong Subtract64(ulong leftLow, ulong leftHigh, ulong rightLow, ulong rightHigh, out ulong high)
    {
        // X64: sub
        // X64: sbb
        // ARM64: subs
        // ARM64: sbc
        ulong low = leftLow - rightLow;
        ulong borrow = low > leftLow ? 1UL : 0UL;
        high = leftHigh - rightHigh - borrow;
        return low;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static uint Add32(uint leftLow, uint leftHigh, uint rightLow, uint rightHigh, out uint high)
    {
        // X64: add
        // X64: adc
        // ARM64: adds
        // ARM64: adc
        uint low = leftLow + rightLow;
        uint carry = low < rightLow ? 1U : 0U;
        high = leftHigh + rightHigh + carry;
        return low;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int SignedAddOverflow(int left, int right)
    {
        // X64: add
        // X64: j{{o|no}}
        // ARM64: adds
        // ARM64: b.v{{s|c}}
        int result = left + right;
        if (((result ^ right) & ~(left ^ right)) < 0)
        {
            Taken();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int SignedSubtractOverflow(int left, int right)
    {
        // X64: sub
        // X64: j{{o|no}}
        // ARM64: subs
        // ARM64: b.v{{s|c}}
        int result = left - right;
        if (((result ^ left) & (left ^ right)) < 0)
        {
            Taken();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong AddCarryBranch64(ulong left, ulong right)
    {
        // X64: add
        // X64: j{{b|ae}}
        // ARM64: adds
        // ARM64: b.{{hs|lo}}
        ulong result = left + right;
        if (result < left)
        {
            Taken();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static uint SubtractBorrowBranch32(uint left, uint right)
    {
        // X64: sub
        // X64: j{{b|ae}}
        // ARM64: subs
        // ARM64: b.{{hs|lo}}
        uint result = left - right;
        if (result > left)
        {
            Taken();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong AddCarryWithLiveOut(
        ulong leftLow, ulong leftHigh, ulong rightLow, ulong rightHigh, bool returnHigh)
    {
        ulong low = leftLow + rightLow;
        ulong carry = low < leftLow ? 1UL : 0UL;
        ulong high = leftHigh + rightHigh + carry;

        if (returnHigh)
        {
            return high;
        }

        return Consume(high, carry, low);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong Consume(ulong high, ulong carry, ulong low) => high + carry + low;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Taken() => s_taken++;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ulong ShiftOutHexDigits(ulong value, int length)
    {
        for (int i = length - 1; (uint)i < (uint)length; i--)
        {
            value >>= 4;
        }

        return value;
    }

    [Fact]
    public static int TestEntryPoint()
    {
        foreach (ulong leftLow in s_values64)
        {
            foreach (ulong rightLow in s_values64)
            {
                foreach (ulong leftHigh in s_values64)
                {
                    foreach (ulong rightHigh in s_values64)
                    {
                        ulong expectedLow = leftLow + rightLow;
                        ulong expectedCarry = expectedLow < leftLow ? 1UL : 0UL;
                        ulong expectedHigh = leftHigh + rightHigh + expectedCarry;

                        ulong actualLow = Add64(leftLow, leftHigh, rightLow, rightHigh, out ulong actualHigh);
                        if ((actualLow != expectedLow) || (actualHigh != expectedHigh))
                        {
                            return -1;
                        }

                        actualLow = Subtract64(leftLow, leftHigh, rightLow, rightHigh, out actualHigh);
                        ulong expectedBorrow = actualLow > leftLow ? 1UL : 0UL;
                        expectedHigh = leftHigh - rightHigh - expectedBorrow;
                        if ((actualLow != leftLow - rightLow) || (actualHigh != expectedHigh))
                        {
                            return -2;
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            bool returnHigh = i != 0;
                            ulong expected = returnHigh ? leftHigh + rightHigh + expectedCarry
                                                        : leftHigh + rightHigh + expectedCarry + expectedCarry +
                                                              expectedLow;
                            ulong actual =
                                AddCarryWithLiveOut(leftLow, leftHigh, rightLow, rightHigh, returnHigh);
                            if (actual != expected)
                            {
                                return -3;
                            }
                        }
                    }
                }
            }
        }

        foreach (uint leftLow in s_values32)
        {
            foreach (uint rightLow in s_values32)
            {
                foreach (uint leftHigh in s_values32)
                {
                    foreach (uint rightHigh in s_values32)
                    {
                        uint expectedLow = leftLow + rightLow;
                        uint expectedCarry = expectedLow < rightLow ? 1U : 0U;
                        uint actualLow = Add32(leftLow, leftHigh, rightLow, rightHigh, out uint actualHigh);
                        if ((actualLow != expectedLow) || (actualHigh != leftHigh + rightHigh + expectedCarry))
                        {
                            return -4;
                        }
                    }
                }
            }
        }

        s_taken = 0;
        if ((SignedAddOverflow(int.MaxValue, 1) != int.MinValue) || (s_taken != 1))
        {
            return -5;
        }

        s_taken = 0;
        if ((SignedAddOverflow(1, 1) != 2) || (s_taken != 0))
        {
            return -6;
        }

        s_taken = 0;
        if ((SignedSubtractOverflow(int.MinValue, 1) != int.MaxValue) || (s_taken != 1))
        {
            return -7;
        }

        s_taken = 0;
        if ((SignedSubtractOverflow(1, 1) != 0) || (s_taken != 0))
        {
            return -8;
        }

        s_taken = 0;
        ulong addResult = AddCarryBranch64(ulong.MaxValue, 1);
        AddCarryBranch64(1, 1);
        if ((addResult != 0) || (s_taken != 1))
        {
            return -9;
        }

        s_taken = 0;
        uint subtractResult = SubtractBorrowBranch32(0, 1);
        SubtractBorrowBranch32(1, 1);
        if ((subtractResult != uint.MaxValue) || (s_taken != 1))
        {
            return -10;
        }

        if (ShiftOutHexDigits(0xFFFF, 4) != 0)
        {
            return -11;
        }

        return 100;
    }
}
