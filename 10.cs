using System;
using System.Collections.Generic;
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
        Console.WriteLine($"Part One (DP): {differences[1].Count() * differences[3].Count()}");
        Console.WriteLine(deviceJoltage);

        var devices = joltageRatings.ToHashSet();
        var dp = new long[deviceJoltage + 1];
        dp[0] = 1;
        for (int finalJoltage = 1; finalJoltage <= deviceJoltage; finalJoltage++)
            dp[finalJoltage] = Enumerable.Range(finalJoltage - 3, 3).Sum(joltage => devices.Contains(joltage) ? dp[joltage] : 0);

        Console.WriteLine($"Part Two: {dp[deviceJoltage]}");

        // Alternative solution with queue
        var counts = new Queue<(long joltage, long count)>();
        counts.Enqueue((0, 1));
        foreach (var joltage in joltageRatings.Skip(1))
        {
            while (counts.Peek().joltage < joltage - 3)
                counts.Dequeue();
            counts.Enqueue((joltage, counts.Sum(c => c.count)));
        }
        Console.WriteLine($"Part Two (with queue): {counts.Last()}");

        // Alternative solution in one expression
        var ans = joltageRatings.Skip(1)
            .Aggregate(
                new[] { (joltage: 0L, count: 1L) }, 
                (current, joltage) => 
                    current
                        .Where(c => c.joltage >= joltage - 3)
                        .Append((joltage, current.Where(c => c.joltage >= joltage - 3).Sum(c => c.count)))
                        .ToArray())
            .Last();
        Console.WriteLine($"Part Two (one expression with aggregate): {ans}");
    }

}