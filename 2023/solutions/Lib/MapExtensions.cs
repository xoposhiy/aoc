public static class MapExtensions
{
    public static int Width<T>(this T[][] map)
    {
        return map[0].Length;
    }
    //Height
    public static int Height<T>(this T[][] map)
    {
        return map.Length;
    }
    public static string[] CreateMap<T>(this IEnumerable<T> points, Func<T, V> getPoint, Func<V, string> empty,
        Func<T, string> showPoint, V? min = null)
    {
        min ??= new V(int.MaxValue, int.MaxValue);
        var pointsImage = points.ToDictionary(getPoint, showPoint);
        var minX = Math.Min(min.X, pointsImage.Keys.Min(p => p.X));
        var minY = Math.Min(min.Y, pointsImage.Keys.Min(p => p.Y));
        var maxX = pointsImage.Keys.Max(p => p.X);
        var maxY = pointsImage.Keys.Max(p => p.Y);
        var map = new string[maxY - minY + 1];
        for (var y = 0; y < map.Length; y++)
        {
            var line = new List<string>();
            for (var x = 0; x < maxX - minX + 1; x++)
            {
                var p = new V(x + minX, y + minY);
                line.Add(pointsImage.TryGetValue(p, out var image) ? image : empty(p));
            }

            map[y] = string.Join("", line);
        }

        return map;
    }

    public static string[] CreateMap(this IEnumerable<V> points, string point = "#", string empty = ".")
    {
        return points.CreateMap(v => v, _ => empty, _ => point);
    }

    public static IEnumerable<V> ToPoints(this string[] lines, Func<V, char, bool> isPoint)
    {
        return lines.SelectMany((line, y) =>
            line.Select((_, x) => new V(x, y)).Where(v => isPoint(v, lines[v.Y][v.X])));
    }

    public static IEnumerable<V> ToPoints(this string[] lines, char pointSymbol)
    {
        return lines.ToPoints((_, c) => c == pointSymbol);
    }

    public static T Get<T>(this T[][] map, V pos, T defaultValue = default)
    {
        if (pos.InRange(map))
            return map[pos.Y][pos.X];
        return defaultValue;
    }

    public static T Set<T>(this T[][] map, V pos, T value)
    {
        return map[pos.Y][pos.X] = value;
    }

    public static V[] GetPositions<T>(this T[][] map, T value)
    {
        return map.GetPositions(v => Equals(v, value));
    }

    public static V[] GetPositions<T>(this T[][] map, Func<T, bool> predicate)
    {
        var result = new List<V>();
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[y].Length; x++)
            if (predicate(map[y][x]))
                result.Add(new V(x, y));

        return result.ToArray();
    }

    public static V GetPosition<T>(this T[][] map, T value)
    {
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[y].Length; x++)
            if (Equals(map[y][x], value))
                return new V(x, y);

        throw new KeyNotFoundException();
    }

    public static string[] FlipX(this string[] lines)
    {
        return lines.Select(line => line.Reverse().StrJoin()).ToArray();
    }

    public static IEnumerable<T> Column<T>(this T[][] map, int x)
    {
        return Enumerable.Range(0, map.Length).Select(y => map[y][x]);
    }
    public static T[][] Columns<T>(this T[][] map)
    {
        return Enumerable.Range(0, map.Width()).Select(x => map.Column(x).ToArray()).ToArray();
    }

    public static T[][] RotateCW<T>(this T[][] map)
    {
        return map.Reversed().ToArray().Columns();
    }

    public static T[][] RotateCCW<T>(this T[][] map)
    {
        return map.Columns().Reverse().ToArray();
    }

    public static IEnumerable<T> Row<T>(this T[][] map, int y)
    {
        return map[y];
    }

    public static string[] Columns(this IEnumerable<string> rows)
    {
        var columns = new List<string>();
        var iRow = 0;
        foreach (var row in rows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                if (columns.Count <= i)
                    columns.Add(new string(' ', iRow));
                columns[i] += row[i];
            }

            iRow++;
        }

        return columns.ToArray();
    }
}