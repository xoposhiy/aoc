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
        Console.WriteLine($"Part One: {differences[1].Count() * differences[3].Count()}");

        // Part2 classic dynamic programming solution
        var devices = joltageRatings.ToHashSet();
        var counts = new long[deviceJoltage + 1];
        counts[0] = 1;
        for (int finalJoltage = 1; finalJoltage <= deviceJoltage; finalJoltage++)
            counts[finalJoltage] = Enumerable.Range(finalJoltage - 3, 3).Sum(joltage => devices.Contains(joltage) ? counts[joltage] : 0);

        Console.WriteLine($"Part Two (DP): {counts[deviceJoltage]}");

        // Part2 alternative solution with queue
        var candidates = new Queue<(long joltage, long count)>();
        candidates.Enqueue((0, 1));
        foreach (var joltage in joltageRatings.Skip(1))
        {
            while (candidates.Peek().joltage < joltage - 3)
                candidates.Dequeue();
            candidates.Enqueue((joltage, candidates.Sum(c => c.count)));
        }
        Console.WriteLine($"Part Two (with queue): {candidates.Last().count}");

        // Part2 alternative solution in one expression
        var ans = joltageRatings.Skip(1)
            .Aggregate(
                new[] { (joltage: 0L, count: 1L) }, 
                (current, joltage) => 
                    current
                        .Append((joltage, current.Where(c => c.joltage >= joltage - 3).Sum(c => c.count)))
                        .ToArray())
            .Last().count;
        Console.WriteLine($"Part Two (one expression with aggregate): {ans}");

        // Part2 alternative solution with diff analysis.

        var freq = joltageRatings
            .Zip(joltageRatings.Skip(1), (a, b) => b - a)
            .ConstantSegments()
            .Where(segment => segment.value == 1)
            .GroupBy(segment => segment.length)
            .ToDictionary(group => group.Key, group => group.Count());
        var mathAns = Math.Pow(7, freq[4]) * Math.Pow(4, freq[3]) * Math.Pow(2, freq[2]);
        

        Console.WriteLine($"Part Two (Everybody step back! I am going to use... MATH!!!111): {mathAns}");
    }
}