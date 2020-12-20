using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using static System.Linq.Enumerable;
using static System.Math;

public class Day20
{
    public record Tile(char[][] Image, int Id)
    {
        public static Tile Parse(string s)
        {
            var lines = s.Trim().Split("\r\n");
            var id = lines[0].Split(' ', ':')[1].ToInt();
            var rows = lines.Skip(1).Select(row => row.ToCharArray()).ToArray();
            return new Tile(rows, id);
        }

        public override string ToString()
        {
            return Image.Select(row => new string(row)).StrJoin("\n");
        }

        public int Size => Image.Length;

        public Tile ApplyTransform(int[] transform)
        {
            var image = Range(0, Size).Select(
                    y => Range(0, Size).Select(
                            x =>
                            {
                                var (xx, yy) = Transform(transform, x, y);
                                return Image[yy][xx];
                            })
                        .ToArray())
                .ToArray();
            return this with { Image = image };
        }

        private (int x, int y) Transform(int[] transform, int x, int y)
        {
            return (
                x * transform[0] + y * transform[1] - transform[4] * (Size - 1),
                x * transform[2] + y * transform[3] - transform[5] * (Size - 1)
                );
        }

        public string GetTopLine(int[] transform)
        {
            var chars = Range(0, Size).Select(x => Transform(transform, x, 0))
                .Select(p => Image[p.y][p.x]).ToArray();
            return new(chars);
        }

        public string GetTopLine() => new(Range(0, Size).Select(x => Image[0][x]).ToArray());
        public string GetBottomLine() => new(Range(0, Size).Select(x => Image[Size - 1][x]).ToArray());
        public string GetLeftLine() => new(Range(0, Size).Select(y => Image[y][0]).ToArray());
        public string GetRightLine() => new(Range(0, Size).Select(y => Image[y][Size - 1]).ToArray());
    }

    public void Solve()
    {
        var transforms = new int[][]
        {
            new[]{1,0,0,1,0,0}, // id
            new[]{0,1,-1,0,0,-1}, //rot90
            new[]{-1,0,0,-1,-1,-1}, //rot180
            new[]{0,-1,1,0,-1,0}, //rot270
            new[]{1,0,0,-1,0,-1}, //flipY
            new[]{-1,0,0,1,-1,0},//flipX
            new[]{0,1,1,0,0,0}, //flip/
            new[]{0,-1,-1,0,-1,-1}, //flip\
        };
        var tiles = File.ReadAllText("20.txt").Split("\r\n\r\n").Select(Tile.Parse).ToList();

        var freq = tiles.SelectMany(
            tile => transforms.Select((tran, i) => (tile, i, top: tile.GetTopLine(tran)))
        ).CountFrequency(t => t.top);

        var corners = tiles
            .Select(tile => (tile, count: transforms.Take(4).Count(t => freq[tile.GetTopLine(t)] > 1)))
            .Where(t => t.count == 2)
            .ToList();

        Console.WriteLine(corners.Count + " = 4 corners?");
        Console.WriteLine(corners.StrJoin(", ", c => c.tile.Id.ToString()));
        Console.WriteLine($"Part One: {corners.Aggregate(1L, (a, b) => a * b.tile.Id)}");

        var map = new Tile[12, 12];
        var corner = corners[0].tile;
        var topLeftCorner = transforms.Select(corner.ApplyTransform)
            .First(tile => freq[tile.GetTopLine(transforms[0])] == 1 && freq[tile.GetTopLine(transforms[1])] == 1);
        tiles.RemoveAll(t => t.Id == topLeftCorner.Id);
        for (int y = 0; y < 12; y++)
            for (int x = 0; x < 12; x++)
            {
                if (x == 0 && y == 0) map[y, x] = topLeftCorner;
                else if (y > 0)
                {
                    var upperTile = map[y - 1, x];
                    var border = upperTile.GetBottomLine();
                    var bestTile = tiles.SelectMany(tile => transforms.Select(tile.ApplyTransform))
                        .Single(tile => tile.GetTopLine() == border);
                    map[y, x] = bestTile;
                    tiles.RemoveAll(t => t.Id == bestTile.Id);
                }
                else if (x > 0)
                {
                    var leftTile = map[y, x - 1];
                    var border = leftTile.GetRightLine();
                    var bestTile = tiles.SelectMany(tile => transforms.Select(tile.ApplyTransform))
                        .Single(tile => tile.GetLeftLine() == border);
                    map[y, x] = bestTile;
                    tiles.RemoveAll(t => t.Id == bestTile.Id);
                }
            }

        Console.WriteLine(tiles.Count + " = 0 tiles left ?");

        var monster = new[]
        {
            "                  # ".ToCharArray(),
            "#    ##    ##    ###".ToCharArray(),
            " #  #  #  #  #  #   ".ToCharArray()
        };

        var charmap = CreateMap(map);
        var monstersCount = 0;
        foreach (var transform in transforms)
        {
            var transformedMonster = TransformMonster(monster, transform);
            var monsterWidth = transformedMonster[0].Length;
            var monsterHeight = transformedMonster.Length;
            for (int y = 0; y < 8 * 12 - monsterHeight; y++)
            {
                for (int x = 0; x < 8 * 12 - monsterWidth; x++)
                {
                    var found = true;

                    for (int mx = 0; mx < monsterWidth; mx++)
                        for (int my = 0; my < monsterHeight; my++)
                        {
                            if (transformedMonster[my][mx] != '#') continue;
                            if (charmap[y + my][x + mx] != '#') found = false;
                        }

                    if (found) monstersCount++;
                }
            }

            if (monstersCount > 0)
            {
                Console.WriteLine("Transform to find monsters: " + transform.StrJoin(" "));
                break;
            }
        }

        Console.WriteLine("Monsters count: " + monstersCount);
        Console.WriteLine($"Part Two: {charmap.Sum(row => row.Count(c => c == '#')) - 15 * monstersCount}");
    }

    private char[][] CreateMap(Tile[,] map)
    {
        var res = new char[8 * 12][];
        for (int y = 0; y < 8 * 12; y++)
        {
            var row = new char[8 * 12];
            for (int x = 0; x < 8 * 12; x++)
            {
                var tile = map[y / 8, x / 8];
                row[x] = tile.Image[1 + y % 8][1 + x % 8];
            }
            res[y] = row;
        }
        return res;
    }

    private char[][] TransformMonster(char[][] monster, int[] transform)
    {
        return monster; //TODO
    }
}