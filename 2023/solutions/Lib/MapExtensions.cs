public static class MapExtensions
{
    public static V BottomRight<T>(this T[][] map) => new(map.Width() - 1, map.Height() - 1);
    public static V BottomLeft<T>(this T[][] map) => new(0, map.Height() - 1);
    public static V TopRight<T>(this T[][] map) => new(map.Width() - 1, 0);
    public static V TopLeft<T>(this T[][] map) => V.Zero;
    
    
    public static IEnumerable<(V Pos, V Dir)> NextSteps<T>(this T[][] map, V pos, V dir) =>
        V.Directions4
            .Where(d => d != -dir)
            .Select(d => (pos: pos + d, dir: d))
            .Where(p => p.pos.InRange(map));

    public static T[][] CloneMap<T>(this T[][] map)
    {
        var result = new T[map.Length][];
        for (var y = 0; y < map.Length; y++)
        {
            result[y] = new T[map[y].Length];
            for (var x = 0; x < map[y].Length; x++)
                result[y][x] = map[y][x];
        }
        return result;
    }
    
    public static int Width<T>(this T[][] map) => map[0].Length;

    public static int Height<T>(this T[][] map) => map.Length;

    public static string[] CreateMap<T>(this IEnumerable<T> points, Func<T, V> getPoint, Func<V, string> empty,
        Func<T, string> showPoint, V? min = null)
    {
        min ??= new V(int.MaxValue, int.MaxValue);
        var pointsImage = points.ToDictionary(getPoint, showPoint);
        var minX = Min(min.X, pointsImage.Keys.Min(p => p.X));
        var minY = Min(min.Y, pointsImage.Keys.Min(p => p.Y));
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
        if (!pos.InRange(map))
            throw new Exception(pos.ToString());
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
        return Range(0, map.Length).Select(y => map[y][x]);
    }
    public static T[][] Columns<T>(this T[][] map)
    {
        return Range(0, map.Width()).Select(x => map.Column(x).ToArray()).ToArray();
    }

    public static T[][] RotateCW<T>(this T[][] map) => 
        map.Reverse().ToArray().Columns();
    
    public static T[][] RotateCWInplace<T>(this T[][] map)
    {
        var size = map.Height();
        if (map.Width() != size)
            throw new ArgumentException("width != height", nameof(map));
        var partSize = size / 2 + size % 2;
        var sizeMinus1 = size - 1;
        for (var y = 0; y < size / 2; y++)
        for (var x = 0; x < partSize; x++)
        {
            var xx = sizeMinus1 - x;
            var yy = sizeMinus1 - y;
            // rotate 4 values clockwise
            var temp = map[y][x];
            map[y][x] = map[xx][y];
            map[xx][y] = map[yy][xx];
            map[yy][xx] = map[x][yy];
            map[x][yy] = temp;
        }
        return map;
    }

    public static T[][] RotateCCW<T>(this T[][] map) => 
        map.Columns().Reverse().ToArray();

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