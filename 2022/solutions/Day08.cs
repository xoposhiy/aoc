using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day08
{
    public void Solve(string[] lines)
    {
        var map = lines.Select(line => line.ToCharArray().Select(c => (int)c).ToArray()).ToArray();
        var visible = new HashSet<V>();
        for (int y = 0; y < map.Length; y++)
        {
            var h = 0;
            for (int x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] > h)
                {
                    visible.Add(new V(x, y));
                    h = map[y][x];
                }
            }
            h = 0;

            for (int x = map[0].Length - 1; x >= 0; x--)
            {
                if (map[y][x] > h)
                {
                    visible.Add(new V(x, y));
                    h = map[y][x];
                }
            }
        }
        for (int x = 0; x < map[0].Length; x++)
        {
            var h = 0;
            for (int y = 0; y < map.Length; y++)
            {
                if (map[y][x] > h)
                {
                    visible.Add(new V(x, y));
                    h = map[y][x];
                }
            }

            h = 0;
            for (int y = map.Length - 1; y >= 0; y--)
            {
                if (map[y][x] > h)
                {
                    visible.Add(new V(x, y));
                    h = map[y][x];
                }
            }
        }

        var p1 = visible.Count();

        long maxScore = 0;
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                var score = CalcScore(map, x, y);
                if (score > maxScore)
                    maxScore = score;
            }
        }



        var p2 = maxScore;
        Console.WriteLine($"Part1: {p1}");
        Console.WriteLine($"Part2: {p2}");
    }

    private long CalcScore(int[][] map, int x, int y)
    {
        long score = 1;
        var h = map[y][x];
        foreach (var d in new[] { new V(0, -1), new V(0, 1), new V(-1, 0), new V(1, 0) })
        {
            var p = new V(x, y) + d;
            var dist = 0;
            while (p.X < map[0].Length && p.Y < map.Length && p.X >= 0 && p.Y >= 0)
            {
                dist++;
                if (map[p.Y][p.X] >= h) break;
                p += d;
            }

            score *= dist;

            //Console.WriteLine(d + " " + dist);
        }
        Console.WriteLine(x + " " + y + " " + score);

        return score;


    }
}