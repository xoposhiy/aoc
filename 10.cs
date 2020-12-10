using System;
using System.IO;
using System.Linq;

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

        var devices = joltageRatings.ToHashSet();
        var counts = new long[deviceJoltage+1];
        counts[0] = 1;
        for (int finalJoltage = 1; finalJoltage <= deviceJoltage; finalJoltage++)
            counts[finalJoltage] = Enumerable.Range(finalJoltage-3, 3).Sum(joltage => devices.Contains(joltage) ? counts[joltage] : 0);
        
        Console.WriteLine($"Part Two: {counts[deviceJoltage]}");
    }

}