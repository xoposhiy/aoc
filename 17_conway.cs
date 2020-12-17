using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;

public class Day17
{
    public record V(int X, int Y, int Z = 0, int W = 0);
        
    public void Solve()
    {
        var map = File.ReadAllLines("17.txt");
        var state = new HashSet<V>();
        for (int y = 0; y < map.Length; y++)
        for (int x = 0; x < map[0].Length; x++)
            if (map[y][x] == '#')
                state.Add(new V(x, y));
        Console.WriteLine($"Part One: {Simulate(state, GetNear3).Count}");
        Console.WriteLine($"Part Two: {Simulate(state, GetNear4).Count}");
    }

    private HashSet<V> Simulate(HashSet<V> state, Func<V, IEnumerable<V>> getNear)
    {
        return Range(0, 6)
            .Aggregate(state, (s, _) => Step(s, getNear));
    }

    private static HashSet<V> Step(HashSet<V> activeCells, Func<V, IEnumerable<V>> getNear)
    {
        return activeCells
            .SelectMany(getNear)
            .CountFrequency()
            .Where(g => g.Value == 3 || g.Value == 2 && activeCells.Contains(g.Key))
            .Select(g => g.Key)
            .ToHashSet();
    }

    private static IEnumerable<V> GetNear3(V v)
    {
        return
            from x in Range(v.X - 1, 3)
            from y in Range(v.Y - 1, 3)
            from z in Range(v.Z - 1, 3)
            where x != v.X || y != v.Y || z != v.Z
            select new V(x, y, z, 0);
    }

    private static IEnumerable<V> GetNear4(V v)
    {
        return
            from x in Range(v.X - 1, 3)
            from y in Range(v.Y - 1, 3)
            from z in Range(v.Z - 1, 3)
            from w in Range(v.W - 1, 3)
            where x != v.X || y != v.Y || z != v.Z || w != v.W
            select new V(x, y, z, w);
    }
}