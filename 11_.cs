using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Day11
{
    public void Solve()
    {
        var map = File.ReadLines("11.txt")
            .Select(line => line.ToCharArray())
            .ToArray();
        var initialMap = map;
        Console.WriteLine(string.Join("\n", map.Select(line => new string(line))));
        for (int i = 0; i < 200; i++)
        {
            map = Step(map);
            var ans = map.SelectMany(c => c).Count(c => c == '#');
            Console.WriteLine(ans);

        }

        var ans1 = map.SelectMany(c => c).Count(c => c == '#');
        Console.WriteLine($"Part One: {ans1}");

        map = initialMap;
        for (int i = 0; i < 200; i++)
        {
            map = Step2(map);
            var ans = map.SelectMany(c => c).Count(c => c == '#');
            //Console.WriteLine(string.Join("\n", map.Select(line => new string(line))));
            Console.WriteLine(ans);
        }


        Console.WriteLine($"Part Two: {2}");
    }

    private char[][] Step(char[][] map)
    {
        var h = map.Length;
        var w = map[0].Length;
        var next = Enumerable.Range(0, h).Select(y => new char[w]).ToArray();
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            if (map[y][x] == '.') next[y][x] ='.';
            else
            {
                var ns =
                    from nx in Enumerable.Range(x - 1, 3)
                    from ny in Enumerable.Range(y - 1, 3)
                    where nx != x || ny != y
                    where nx >= 0 && nx < w && ny >= 0 && ny < h
                    where map[ny][nx] == '#'
                    select (nx, ny);
                var occupiedCount = ns.Count();
                next[y][x] = occupiedCount == 0 ? '#' : occupiedCount >= 4 ? 'L' : map[y][x];
            }
        }
        return next;
    }

    private char[][] Step2(char[][] map)
    {
        var h = map.Length;
        var w = map[0].Length;
        var next = Enumerable.Range(0, h).Select(y => new char[w]).ToArray();
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            if (map[y][x] == '.') next[y][x] = '.';
            else
            {
                var ns =
                    from dx in new[]{-1,0, 1}
                    from dy in new[]{-1,0, 1}
                    where dx != 0 || dy != 0
                    select (dx, dy);
                var occupiedCount = ns.Select(d => GetSeatInDirection(map, x, y, d.dx, d.dy)).Count(c => c == '#');
                next[y][x] = occupiedCount == 0 ? '#' : occupiedCount >= 5 ? 'L' : map[y][x];
            }
        }
        return next;
    }

    private char GetSeatInDirection(char[][]map, int x, int y, int dx, int dy)
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