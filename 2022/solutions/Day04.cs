using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day04
{
    public void Solve(string[] lines)
    {
        // input:
        // 35-73,35-82
        // ...
        var pairs = lines.ParseLines<int[]>();

        //In how many assignment pairs does one range fully contain the other?
        var containCount = pairs.Count(p => p[0] <= p[2] && p[1] >= p[3] || p[2] <= p[0] && p[3] >= p[1]);
        Console.WriteLine($"Part1: {containCount}");

        //In how many assignment pairs do the ranges overlap?
        var overlapCount = pairs.Count(x => Math.Max(x[0], x[2]) <= Math.Min(x[1], x[3]));
        Console.WriteLine($"Part2: {overlapCount}");
    }
}