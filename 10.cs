using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Local

public class Day10
{
    public void Solve()
    {
        var joltageRatings = File.ReadLines("10.txt")
            .Select(long.Parse).OrderBy(n => n)
            .Prepend(0)
            .ToList();
        var deviceJoltage = joltageRatings[^1] + 3;
        joltageRatings.Add(deviceJoltage);
        var differences = joltageRatings.Zip(joltageRatings.Skip(1), (a, b) => b - a).ToLookup(x => x);
        Console.WriteLine($"Part One: {differences[1].Count() * differences[3].Count()}");

        var lookup = joltageRatings.ToLookup(n => n);
        var counts = new long[deviceJoltage+1];
        counts[0] = 1;
        for (int i = 1; i < deviceJoltage+1; i++)
            counts[i] = Enumerable.Range(i-3, 3).Sum(joltage => lookup[joltage].Any() ? counts[joltage] : 0);
        
        Console.WriteLine($"Part Two: {counts[deviceJoltage]}");
    }

}