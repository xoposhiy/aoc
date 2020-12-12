using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

public class Day12
{
    public record Command(char dir, int count);
    public record Ferry(char dir, int x, int y);
    public record Point(int x, int y);

    public void Solve()
    {
        var ns = File.ReadLines("12.txt")
            .Select(s => new Command(s[0], int.Parse(s.Substring(1))))
            .ToArray();
        var ferry = new Ferry('E', x: 0, y: 0);
        ferry = ns.Aggregate(ferry, Move);

        Console.WriteLine($"Part One: {Abs(ferry.x) + Abs(ferry.y)}");
        var wp = new Point(x: 10, y: -1);
        var pos = new Point(x: 0, y: 0);
        foreach (var step in ns)
        {
            if (step.dir == 'F')
                pos = new Point(pos.x + wp.x * step.count, pos.y + wp.y * step.count);
            else
                wp = MoveWayPoint(wp, step);
        }
        Console.WriteLine($"Part Two: {Abs(pos.x) + Abs(pos.y)}");
    }

    private Point MoveWayPoint(Point wp, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => RotateWpRight(wp, (len / 90) % 4),
            ('L', var len) => RotateWpRight(wp, 4-(len / 90) % 4),
            var (dir, len) => new Point(wp.x + deltas[dir].x * len, wp.y + deltas[dir].y * len)
        };
    }

    private Point RotateWpRight(Point wp, int len)
    {
        for (int i = 0; i < len; i++)
            wp = new Point(-wp.y, wp.x);
        return wp;
    }

    private static readonly Dictionary<char, char> nextDir = new()
    {
        {'E', 'S'},
        {'S', 'W'},
        {'W', 'N'},
        {'N', 'E'},
    };

    private static readonly Dictionary<char, (int x, int y)> deltas= new()
    {
        {'E', (1, 0)},
        {'S', (0, 1)},
        {'W', (-1, 0)},
        {'N', (0, -1)},
    };

    private Ferry Move(Ferry ferry, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => ferry with { dir = RotateRight(ferry.dir, (len / 90) % 4) },
            ('L', var len) => ferry with { dir = RotateRight(ferry.dir, 4 - (len / 90) % 4) },
            ('F', var len) => ferry with { x = ferry.x + deltas[ferry.dir].x * len, y = ferry.y + deltas[ferry.dir].y * len },
            var (dir, len) => new Ferry(ferry.dir, ferry.x + deltas[dir].x * len, ferry.y + deltas[dir].y * len)
        };
    }

    private char RotateRight(char dir, int count)
    {
        for (int i = 0; i < count; i++)
            dir = nextDir[dir];
        return dir;
    }
}