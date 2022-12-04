using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DayN
{
    public void Solve(string[] lines)
    {
        var data = lines.ParseLines<int>();
        var p1 = 1;
        var p2 = 2;
        Console.WriteLine($"Part1: {p1}");
        Console.WriteLine($"Part2: {p2}");
    }
}