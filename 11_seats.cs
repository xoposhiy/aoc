using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;

public class Day11
{
    public void Solve()
    {
        var map = File.ReadLines("11.txt")
            .Select(line => line.ToCharArray())
            .ToArray();
        PlacePeople(map);
        Console.WriteLine($"Part One: {GetOccupiedSeatsCount(map)}");
        map = File.ReadLines("11.txt")
            .Select(line => line.ToCharArray())
            .ToArray();
        for (var i = 0; i < 200; i++)
        {
            map = Step2(map);
            var ans = map.SelectMany(c => c).Count(c => c == '#');
            //Console.WriteLine(string.Join("\n", map.Select(line => new string(line))));
            Console.WriteLine(ans);
        }
        Console.WriteLine($"Part Two: {map.SelectMany(c => c).Count(c => c == '#')}");
    }

    private static int GetOccupiedSeatsCount(char[][] map)
    {
        var occupiedSeatsCount = map.SelectMany(c => c).Count(c => c == '#');
        return occupiedSeatsCount;
    }

    private IEnumerable<Vec> GetFreeNeighbors(Vec p, char[][] map)
    {
        return p.GetNeighbors8().Where(pos => map.EqAt(pos, 'L'));
    }

    private bool WillBecomeOccupiedAfter2Ticks(Vec p, char[][] map)
    {
        if (!map.EqAt(p, 'L')) return false;
        return GetFreeNeighbors(p, map).Count() < 4;
    }

    
    
    
    private void PlacePeople(char[][] map)
    {
        var front = 
            map.Indices()
            .Where(p => WillBecomeOccupiedAfter2Ticks(p, map))
            .ToHashSet();
        foreach (var (x, y) in front)
            map[y][x] = '#';
        while (front.Count > 0)
        {
            var emptyFront = front
                .SelectMany(p => GetFreeNeighbors(p, map))
                .ToList();
            foreach (var (x, y) in emptyFront)
                map[y][x] = '0';
            front = emptyFront
                .SelectMany(p => GetFreeNeighbors(p, map))
                .Where(p => WillBecomeOccupiedAfter2Ticks(p, map))
                .ToHashSet();
            foreach (var (x, y) in front)
                map[y][x] = '#';
        }
    }
    
    
    
    

    private char[][] Step2(char[][] map)
    {
        var h = map.Length;
        var w = map[0].Length;
        var next = Range(0, h).Select(y => new char[w]).ToArray();
        for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
                if (map[y][x] == '.')
                {
                    next[y][x] = '.';
                }
                else
                {
                    var occupiedCount = Vec.Directions8.Select(d => GetSeatInDirection(map, x, y, d.X, d.Y)).Count(c => c == '#');
                    next[y][x] = occupiedCount == 0 ? '#' : occupiedCount >= 5 ? 'L' : map[y][x];
                }

        return next;
    }

    private char GetSeatInDirection(char[][] map, int x, int y, int dx, int dy)
    {
        var h = map.Length;
        var w = map[0].Length;
        x += dx;
        y += dy;
        while (x >= 0 && x < w && y >= 0 && y < h)
        {
            if (map[y][x] != '.') return map[y][x];
            x += dx;
            y += dy;
        }

        return '.';
    }
}