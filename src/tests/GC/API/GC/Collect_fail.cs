// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/* 
 * Tests GC.Collect(n), where n = -1...MaxGeneration+10
 * An exception should be thrown for -1, but no other value
 *
 * Notes:
 *    -passes with DOTNET_jitminops
 *    -passes with debug
 *    -passes with DOTNET_gcstress
 */

using System;
using Xunit;

public class Test_Collect_fail
{
    [Fact]
    public static int TestEntryPoint()
    {
        int[] array = new int[25];
        bool passed = false;

        try
        {
            GC.Collect(-1);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Should throw exception
            passed = true;
        }

        if (!passed)
        {
            // Exception not thrown
            Console.WriteLine("Test for GC.Collect(-1) failed: ArgumentOutOfRangeException not thrown!");
            return 1;
        }

        for (int i = 0; i <= GC.MaxGeneration + 10; i++)
        {
            try
            {
                GC.Collect(i); // Should not throw exception!
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Exception thrown
                Console.WriteLine("Test for GC.Collect({0}) failed: {1}", i, e.Message);
                return 1;
            }
        }

        Console.WriteLine("Test for GC.Collect() passed!");
        return 100;
    }
}
