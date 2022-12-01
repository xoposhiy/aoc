using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day01
{
    public void Solve(string[] lines)
    {
        var sums = lines.ParseBlocks<int>().Select(b => b.Sum()).ToList();
        Console.WriteLine($"Part 1: {sums.Max()}");
        Console.WriteLine($"Part 2: {sums.OrderDescending().Take(3).Sum()}");
    }
}