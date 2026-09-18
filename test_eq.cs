using System;
using System.Runtime.CompilerServices;

class Program {
    static void Main() {
        float expected = -0.0f;
        float actual = 0.0f;
        Console.WriteLine($"expected == actual: {expected == actual}");
        Console.WriteLine($"BitConverter.SingleToInt32Bits(expected) == BitConverter.SingleToInt32Bits(actual): {BitConverter.SingleToInt32Bits(expected) == BitConverter.SingleToInt32Bits(actual)}");
    }
}
