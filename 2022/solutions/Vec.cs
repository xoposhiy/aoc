using System;
using System.Collections.Generic;
using System.Linq;
using static System.Linq.Enumerable;

// ReSharper disable once CheckNamespace
public record Vec(int X, int Y)
{
    public static Vec operator +(Vec a, Vec b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec operator -(Vec a, Vec b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec operator *(Vec a, int b) => new(a.X * b, a.Y * b);

    public int MLen => Math.Abs(X) + Math.Abs(Y);

    public static IEnumerable<Vec> Rect(int width, int height) =>
        from x in Range(0, width)
        from y in Range(0, height)
        select new Vec(x, y);
    
    public IEnumerable<Vec> GetNeighbors8() =>
        Directions8.Select(d => d + this);
    
    public static readonly Vec[] Directions8 = (
        from dx in new[] { -1, 0, 1 }
        from dy in new[] { -1, 0, 1 }
        where dx != 0 || dy != 0
        select new Vec(dx, dy)).ToArray();

    public static Vec FromCompass(char compass) => FromCompass(compass + "");

    public static Vec FromCompass(string compass) =>
        compass switch
        {
            "W" => new Vec(-1, 0),
            "E" => new Vec(1, 0),
            "N" => new Vec(0, -1),
            "S" => new Vec(0, 1),
            "NW" => new Vec(-1, -1),
            "SE" => new Vec(-1, 1),
            "NE" => new Vec(1, -1),
            "SW" => new Vec(1, 1),
            _ => throw new Exception(compass)
        };

    public string Compass() =>
        this switch
        {
            (1, 0) => "E",
            (-1, 0) => "W",
            (0, -1) => "N",
            (0, 1) => "S",
            (1, -1) => "NE",
            (-1, -1) => "NW",
            (1, 1) => "SE",
            (-1, 1) => "SW",
            _ => throw new Exception(ToString())
        };

    public Vec RotateCCW() => new(-Y, X);
    public Vec RotateCW() => new(Y, -X);

    public Vec RotateCCW(int turnsCount) =>
        turnsCount switch
        {
            0 => this,
            1 => RotateCCW(),
            2 => RotateCW().RotateCW(),
            3 => RotateCW(),
            < 0 => RotateCCW(4 - Math.Abs(turnsCount) % 4),
            _ => RotateCCW(turnsCount % 4)
        };
}