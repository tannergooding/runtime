using System;
class Program {
    static void Main() {
        double x = 0.3;
        double u = 1.0 + x;
        double Kahan1 = Math.Log(u) * (x / (u - 1.0));
        double Kahan2 = Math.Log(u) - ((u - 1.0) - x) / u;
        double TensorLog = Math.Log(u) - ((u - 1.0) - x);
        Console.WriteLine($"x: {x}");
        Console.WriteLine($"Kahan1: {Kahan1:G17}");
        Console.WriteLine($"Kahan2: {Kahan2:G17}");
        Console.WriteLine($"TensorLog: {TensorLog:G17}");
        Console.WriteLine($"Expected Math.Log(1.3): {Math.Log(1.3):G17}");
    }
}
