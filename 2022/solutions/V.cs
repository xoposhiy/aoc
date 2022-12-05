using static System.Linq.Enumerable;

// ReSharper disable once CheckNamespace
public record V(int X, int Y)
{
    public static V operator +(V a, V b) => new(a.X + b.X, a.Y + b.Y);
    public static V operator -(V a, V b) => new(a.X - b.X, a.Y - b.Y);
    public static V operator *(V a, int b) => new(a.X * b, a.Y * b);

    public int MLen => Math.Abs(X) + Math.Abs(Y);

    public static IEnumerable<V> Rect(int width, int height) =>
        from x in Range(0, width)
        from y in Range(0, height)
        select new V(x, y);
    
    public IEnumerable<V> GetNeighbors8() =>
        Directions8.Select(d => d + this);
    
    public static readonly V[] Directions8 = (
        from dx in new[] { -1, 0, 1 }
        from dy in new[] { -1, 0, 1 }
        where dx != 0 || dy != 0
        select new V(dx, dy)).ToArray();

    public static V FromCompass(char compass) => FromCompass(compass + "");

    public static V FromCompass(string compass) =>
        compass switch
        {
            "W" => new V(-1, 0),
            "E" => new V(1, 0),
            "N" => new V(0, -1),
            "S" => new V(0, 1),
            "NW" => new V(-1, -1),
            "SE" => new V(-1, 1),
            "NE" => new V(1, -1),
            "SW" => new V(1, 1),
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

    public V RotateCCW() => new(-Y, X);
    public V RotateCW() => new(Y, -X);

    public V RotateCCW(int turnsCount) =>
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