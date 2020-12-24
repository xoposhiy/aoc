using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Linq.Enumerable;
using static System.Math;

public class Day24
{
    public void Solve()
    {
        var alive = File.ReadAllLines("24.txt")
            .Select(ParseLine)
            .Select(GetVec)
            .GroupBy(v => v)
            .Where(g => g.Count() % 2 == 1)
            .Select(g => g.Key)
            .ToHashSet();
        Console.WriteLine($"Part One: {alive.Count}");

        for (int i = 0; i < 100; i++)
            alive = Step(alive, GetNear);
        Console.WriteLine($"Part Two: {alive.Count}");
    }

    private IEnumerable<Vec> GetNear(Vec v)
    {
        yield return v + new Vec(1, 0);
        yield return v + new Vec(-1, 0);
        yield return v + new Vec(0, -1);
        yield return v + new Vec(-1, -1);
        yield return v + new Vec(1, 1);
        yield return v + new Vec(0, 1);
    }

    private static HashSet<Vec> Step(HashSet<Vec> activeCells, Func<Vec, IEnumerable<Vec>> getNear)
    {
        return activeCells
            .SelectMany(getNear)
            .CountFrequency()
            .Where(g => g.Value == 2 || g.Value == 1 && activeCells.Contains(g.Key))
            .Select(g => g.Key)
            .ToHashSet();
    }

    private Vec GetVec(IEnumerable<string> tile)
    {
        return tile.Aggregate(new Vec(0, 0), (v, inst) => v + GetVec(inst));
    }

    private Vec GetVec(string inst)
    {
        return inst switch
        {
            "e" => new Vec(1, 0),
            "w" => new Vec(-1, 0),
            "se" => new Vec(0, -1),
            "sw" => new Vec(-1, -1),
            "ne" => new Vec(1, 1),
            "nw" => new Vec(0, 1),
            _ => throw new Exception(inst)
        };
    }

    private IEnumerable<string> ParseLine(string arg)
    {
        return Regex.Matches(arg, "(se|sw|ne|nw|e|w)").Select(m => m.Value);
    }
}