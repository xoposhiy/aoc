using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static System.Linq.Enumerable;
using static System.Math;
    
public class Day11
{
    public void Solve()
    {
        var map = File.ReadLines("11.txt")
            .Select(line => line.ToCharArray())
            .ToArray();
        var sw = Stopwatch.StartNew();
        PlacePeople(map);
        Console.WriteLine(sw.Elapsed);
        Console.WriteLine(map.SelectMany(c => c).Count(c => c == '#'));
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

    private IEnumerable<(int x, int y)> GetFreeNeighbors(int x, int y, char[][] map)
    {
        return
            from nx in Range(x-1, 3)
            from ny in Range(x-1, 3)
            where nx != 0 || ny != 0
            where nx >= 0 && nx < map[0].Length && ny >= 0 && ny < map.Length
            where map[ny][nx] == 'L'
            select (nx, ny);
    }

    private bool WillBecomeOccupied(int x, int y, char[][] map)
    {
        if (map[y][x] != 'L') return false;
        return GetFreeNeighbors(x, y, map).Count() < 4;
    }

    private void PlacePeople(char[][] map)
    {
        var front = (
                from y in Range(0, map.Length)
                from x in Range(0, map[0].Length)
                where WillBecomeOccupied(x, y, map)
                select (x, y)
                ).ToHashSet();
        foreach (var (x, y) in front)
            map[y][x] = '#';
        while (front.Count > 0)
        {
            var emptyFront = front
                .SelectMany(p => GetFreeNeighbors(p.x, p.y, map))
                .ToList();
            foreach (var (x, y) in emptyFront)
                map[y][x] = '0';
            front = emptyFront
                .SelectMany(p => GetFreeNeighbors(p.x, p.y, map))
                .Where(p => WillBecomeOccupied(p.x, p.y, map))
                .ToHashSet();
            foreach (var (x, y) in front)
                map[y][x] = '#';
        }
    }

    private char[][] Step2(char[][] map)
    {
        var h = map.Length;
        var w = map[0].Length;
        var next = Enumerable.Range(0, h).Select(y => new char[w]).ToArray();
        for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
                if (map[y][x] == '.')
                {
                    next[y][x] = '.';
                }
                else
                {
                    var ns =
                        from dx in new[] { -1, 0, 1 }
                        from dy in new[] { -1, 0, 1 }
                        where dx != 0 || dy != 0
                        select (dx, dy);
                    var occupiedCount = ns.Select(d => GetSeatInDirection(map, x, y, d.dx, d.dy)).Count(c => c == '#');
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