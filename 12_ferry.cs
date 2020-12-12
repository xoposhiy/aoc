using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;

public class Day12
{
    public record Command(char Dir, int Count);
    public record Ferry(char Dir, Point Pos);

    public record Point(int X, int Y)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
        public static Point operator *(Point a, int b) => new(a.X * b, a.Y * b);
        public int MLen => Abs(X) + Abs(Y);
    }

    public void Solve()
    {
        var commands = File.ReadLines("12.txt")
            .Select(s => new Command(s[0], int.Parse(s[1..])))
            .ToArray();
        Part1(commands);
        Part2(commands);
    }

    private void Part1(Command[] commands)
    {
        var ferry = new Ferry('E', new(0, 0));
        ferry = commands.Aggregate(ferry, Move);
        Console.WriteLine($"Part One: {ferry.Pos.MLen}");
    }

    private void Part2(Command[] commands)
    {
        var wp = new Point(10, -1);
        var pos = new Point(0, 0);
        foreach (var command in commands)
        {
            if (command is ('F', var count))
                pos += wp * count;
            else
                wp = MoveWayPoint(wp, command);
        }

        Console.WriteLine($"Part Two: {pos.MLen}");
    }

    private Point MoveWayPoint(Point wp, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => RotateWayPointRight(wp, (len / 90) % 4),
            ('L', var len) => RotateWayPointRight(wp, 4 - (len / 90) % 4),
            var (dir, len) => wp + deltas[dir] * len
        };
    }

    private Point RotateWayPointRight(Point wp, int len)
    {
        for (int i = 0; i < len; i++)
            wp = new Point(-wp.Y, wp.X);
        return wp;
    }

    private static readonly Dictionary<char, char> nextDir = new()
    {
        { 'E', 'S' },
        { 'S', 'W' },
        { 'W', 'N' },
        { 'N', 'E' },
    };

    private static readonly Dictionary<char, Point> deltas = new()
    {
        { 'E', new(1, 0) },
        { 'S', new(0, 1) },
        { 'W', new(-1, 0) },
        { 'N', new(0, -1) },
    };

    private Ferry Move(Ferry ferry, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => ferry with { Dir = RotateRight(ferry.Dir, (len / 90) % 4) },
            ('L', var len) => ferry with { Dir = RotateRight(ferry.Dir, 4 - (len / 90) % 4) },
            ('F', var len) => ferry with { Pos = ferry.Pos + deltas[ferry.Dir] * len },
            var (dir, len) => ferry with { Pos = ferry.Pos + deltas[dir] * len }
        };
    }

    private char RotateRight(char dir, int count)
    {
        for (int i = 0; i < count; i++)
            dir = nextDir[dir];
        return dir;
    }
}