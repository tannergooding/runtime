using System;

class Program {
    static void Main() {
        float expected = -0.0f;
        float actual = 0.0f;
        float actualTolerance = 0.0f;
        bool fail = MathF.Abs(expected - actual) > actualTolerance;
        Console.WriteLine($"MathF.Abs(expected - actual) > actualTolerance : {fail}");
    }
}
