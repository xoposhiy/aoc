using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day04
{
    // input:
    // 35-73,35-82
    // ...
    public void Solve((R, R)[] pairs)
    {
        //In how many assignment pairs does one range fully contain the other?
        var containCount = pairs.Count(pair => pair.Item1.Contains(pair.Item2) || pair.Item2.Contains(pair.Item1));
        Console.WriteLine($"Part1: {containCount}");

        //In how many assignment pairs do the ranges overlap?
        var overlapCount = pairs.Count(pair => pair.Item1.Overlaps(pair.Item2));
        Console.WriteLine($"Part2: {overlapCount}");
    }
}
