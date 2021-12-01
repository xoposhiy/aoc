using System;
using System.IO;
using System.Linq;

public class Day12
{
    public record Command(char OpCode, int Count);
    public record Ferry(Vec Dir, Vec Pos);

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
        var ferry = new Ferry(Vec.FromCompass("E"), new(0, 0));
        ferry = commands.Aggregate(ferry, Move);
        Console.WriteLine($"Part One: {ferry.Pos.MLen}");
    }

    private Ferry Move(Ferry ferry, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => ferry with { Dir = ferry.Dir.RotateCCW(len / 90) },
            ('L', var len) => ferry with { Dir = ferry.Dir.RotateCCW(-len / 90) },
            ('F', var len) => ferry with { Pos = ferry.Pos + ferry.Dir * len },
            var (dir, len) => ferry with { Pos = ferry.Pos + Vec.FromCompass(dir) * len }
        };
    }

    private void Part2(Command[] commands)
    {
        var wp = new Vec(10, -1);
        var pos = new Vec(0, 0);
        foreach (var command in commands)
        {
            if (command is ('F', var count))
                pos += wp * count;
            else
                wp = MoveWayPoint(wp, command);
        }
        Console.WriteLine($"Part Two: {pos.MLen}");
    }

    private Vec MoveWayPoint(Vec wp, Command cmd)
    {
        return cmd switch
        {
            ('R', var len) => wp.RotateCCW(len / 90),
            ('L', var len) => wp.RotateCCW(-len / 90),
            var (dir, len) => wp + Vec.FromCompass(dir) * len
        };
    }
}