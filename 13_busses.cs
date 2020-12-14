using System;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

public class Day13
{
    public void Solve()
    {
        var inp = File.ReadAllLines("13.txt");
        var time = inp[0].ToInt();
        (int id, int index)[] indexedIds = inp[1].Split(",").Select((s, i) => s == "x" ? (0, i) : (int.Parse(s), i)).Where(t => t.Item1 != 0).ToArray();
        var ids = indexedIds.Select(t => t.id).ToArray();
        Console.WriteLine(indexedIds.StrJoin("\n"));
        var bestId = ids.MinBy(id => Complement(time, id));
        Console.WriteLine($"Part One: {bestId} * {Complement(time, bestId)}");
        Console.WriteLine($"Part One: {bestId * Complement(time, bestId)}");
        Console.WriteLine(ids.Aggregate(1L, (a, b) => checked(a*b)));
        Console.WriteLine(ids.Max());
        Console.WriteLine($"Part Two: {2}");
    }

    private int Complement(int x, in int modulo)
    {
        return (x * modulo - x) % modulo;
    }
}