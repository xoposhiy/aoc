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
        var pairs = lines.ParseLines<Seg[]>();

        //In how many assignment pairs does one range fully contain the other?
        var containCount = pairs.Count(r => r[0].Contains(r[1]) || r[1].Contains(r[0]));
        Console.WriteLine($"Part1: {containCount}");

        //In how many assignment pairs do the ranges overlap?
        var overlapCount = pairs.Count(r => r[0].Overlaps(r[1]));
        Console.WriteLine($"Part2: {overlapCount}");
    }
}