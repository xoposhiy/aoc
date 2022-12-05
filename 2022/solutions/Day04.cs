using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day04
{
    // input:
    // 35-73,35-82
    // ...
    public void Solve([Separator("-,")](Seg a, Seg b)[] pairs)
    {
        //In how many assignment pairs does one range fully contain the other?
        var containCount = pairs.Count(r => r.a.Contains(r.b) || r.b.Contains(r.a));
        Console.WriteLine($"Part1: {containCount}");

        //In how many assignment pairs do the ranges overlap?
        var overlapCount = pairs.Count(r => r.a.Overlaps(r.b));
        Console.WriteLine($"Part2: {overlapCount}");
    }
}
