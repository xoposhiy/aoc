using System.Collections;
using System.Numerics;

public static class BitsExtensions
{
    public static bool HasBit(this long v, int bitIndex) => (v & (1L << bitIndex)) != 0;
    public static bool HasBit(this int v, int bitIndex) => (v & (1 << bitIndex)) != 0;

    public static long SetBit(this long v, int bitIndex) => v | (1L << bitIndex);
    public static int SetBit(this int v, int bitIndex) => v | (1 << bitIndex);

    public static long UnsetBit(this long v, int bitIndex) => v & ~(1L << bitIndex);
    public static int UnsetBit(this int v, int bitIndex) => v & ~(1 << bitIndex);

    public static bool HasBit(this int v, char letter) => v.HasBit(char.ToLower(letter) - 'a');
    public static int SetBit(this int v, char letter) => v.SetBit(char.ToLower(letter) - 'a');
    public static int UnsetBit(this int v, char letter) => v.UnsetBit(char.ToLower(letter) - 'a');
}

public static class MapExtensions
{
    public static T Get<T>(this T[][] map, V pos) => map[pos.Y][pos.X];
    public static T Set<T>(this T[][] map, V pos, T value) => map[pos.Y][pos.X] = value;

    public static V[] GetPositions<T>(this T[][] map, T value)
    {
        return map.GetPositions(v => Equals(v, value));
    }

    public static V[] GetPositions<T>(this T[][] map, Func<T, bool> predicate)
    {
        var result = new List<V>();
        for (int y = 0; y < map.Length; y++)
        for (int x = 0; x < map[y].Length; x++)
        {
            if (predicate(map[y][x]))
                result.Add(new(x, y));
        }

        return result.ToArray();
    }

    public static V GetPosition<T>(this T[][] map, T value)
    {
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[y].Length; x++)
        {
            if (Equals(map[y][x], value))
                return new(x, y);
        }

        throw new KeyNotFoundException();
    }

    public static string[] FlipX(this string[] lines) => lines.Select(line => line.Reverse().StrJoin()).ToArray();

    public static IEnumerable<T> Column<T>(this T[][] map, int x) =>
        Enumerable.Range(0, map.Length).Select(y => map[y][x]);

    public static IEnumerable<T> Row<T>(this T[][] map, int y) => map[y];

    public static string[] Columns(this IEnumerable<string> rows)
    {
        var columns = new List<string>();
        var iRow = 0;
        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length; i++)
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

public static class Extensions
{
    public static T ModPositive<T>(this T dividend, T divisor) where T : IAdditionOperators<T, T, T>, IModulusOperators<T, T, T> =>
        (dividend % divisor + divisor) % divisor;
    
    public static bool IsOneOf<T>(this T item, params T[] set)
    {
        return set.IndexOf(item) >= 0;
    }

    public static IEnumerable<T> CycleShiftLeft<T>(this IList<T> items, int count)
    {
        count = count.ModPositive(items.Count);
        return items.Skip(count).Concat(items.Take(count));
    }

    public static string[] CreateMap<T>(this IEnumerable<T> points, Func<T, V> getPoint, Func<V, string> empty, Func<T, string> showPoint)
    {
        var pointsImage = points.ToDictionary(getPoint, showPoint);
        var minX = pointsImage.Keys.Min(p => p.X);
        var minY = pointsImage.Keys.Min(p => p.Y);
        var maxX = pointsImage.Keys.Max(p => p.X);
        var maxY = pointsImage.Keys.Max(p => p.Y);
        var map = new string[maxY - minY + 1];
        for (int y = 0; y < map.Length; y++)
        {
            var line = new List<string>();
            for (int x = 0; x < maxX - minX + 1; x++)
            {
                var p = new V(x + minX, y + minY);
                line.Add(pointsImage.TryGetValue(p, out var image) ? image: empty(p));
            }
            map[y] = string.Join("", line);
        }
        return map;
    }

    public static string[] CreateMap(this IEnumerable<V> points, string point = "#", string empty = ".") => 
        points.CreateMap(v => v, _ => empty, _ => point);

    public static IEnumerable<V> ToPoints(this string[] lines, Func<V, char, bool> isPoint) => 
        lines.SelectMany((line, y) => line.Select((_, x) => new V(x, y)).Where(v => isPoint(v, lines[v.Y][v.X])));
    
    public static IEnumerable<V> ToPoints(this string[] lines, char pointSymbol) =>
        lines.ToPoints((_, c) => c == pointSymbol);

    public static string Format(this object? value)
    {
        if (value is null) return "";
        if (value is string s) return s;
        if (value is IEnumerable e)
        {
            
            var parts = e.Cast<object>().Select(Format).ToList();
            if (parts.All(p => p.Length < 5))
                return "[" + string.Join(",", parts) + "]";
            return "[\n" + string.Join(",\n", parts) + "\n]";

        }
        return value.ToString() ?? "";
    }

    public static T Out<T>(this T value, string prefix = "")
    {
        Console.Write(prefix);
        Console.WriteLine(value.Format());
        return value;
    }
    public static Stack<T> ToStack<T>(this IEnumerable<T> source) => new(source);
    public static Queue<T> ToQueue<T>(this IEnumerable<T> source) => new(source);

    public static IEnumerable<T> Reversed<T>(this IEnumerable<T> source) => source.Reverse();

    public static IEnumerable<V> Indices<T>(this T[][] map) =>
        V.AllInRange(map[0].Length, map.Length);

    public static IEnumerable<int> Indices<T>(this T[] map) =>
        Enumerable.Range(0, map.Length);

    public static IEnumerable<T> EveryNth<T>(this IEnumerable<T> items, int n, int startFromIndex = 0) =>
        items.Skip(startFromIndex).Where((_, index) => index % n == 0);

    public static Dictionary<TKey, TValue> FindBijection<TKey, TValue>(this IEnumerable<(TKey, HashSet<TValue>)> groups) where TKey : notnull
    {
        var d = groups.ToList();
        var used = new HashSet<TKey>();
        foreach (var _ in d)
        {
            var option = d
                .First(o => o.Item2.Count == 1 && used.Add(o.Item1));
            foreach (var (_, values) in d.Where(other => !other.Item1.Equals(option.Item1)))
                values.Remove(option.Item2.Single());
        }
        return d.ToDictionary(p => p.Item1, p => p.Item2.Single());
    }

    public static IEnumerable<T[]> SplitBy<T>(this IEnumerable<T> items, Predicate<T> isSeparator)
        where T : IEquatable<T>?
    {
        var group = new List<T>();
        foreach (var item in items)
        {
            if (isSeparator(item))
            {
                if (group.Count > 0)
                {
                    yield return group.ToArray();
                    group.Clear();
                }
            }
            else
            {
                group.Add(item);
            }
        }
        if (group.Count > 0)
            yield return group.ToArray();

    }

    public static IEnumerable<(T value, int length)> ConstantSegments<T>(this IEnumerable<T> items) where T : IEquatable<T>
    {
        var isFirst = true;
        T? value = default;
        var length = 1;
        foreach (var item in items)
        {
            if (!isFirst && item.Equals(value))
                length++;
            else
            {
                if (!isFirst)
                    yield return (value!, length);
                length = 1;
            }
            value = item;
            isFirst = false;
        }
        if (!isFirst)
            yield return (value!, length);
    }

    public static HashSet<TValue> IntersectAll<TKey, TValue>(this IEnumerable<TKey> items, Func<TKey, IEnumerable<TValue>> get)
    {
        return items.Select(get).IntersectAll();
    }

    public static HashSet<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> items)
    {
        HashSet<T>? set = null;
        foreach (var item in items)
        {
            if (set == null) set = item.ToHashSet();
            else set.IntersectWith(item);
        }
        return set ?? new HashSet<T>();
    }

    public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var i = 0;
        foreach (var item in items)
        {
            if (predicate(item)) return i;
            i++;
        }

        return -1;
    }

    public static int IndexOf<T>(this IList<T> items, int startIndex, Func<T, bool> predicate)
    {
        for (var index = startIndex; index < items.Count; index++)
        {
            var item = items[index];
            if (predicate(item)) return index;
        }
        return -1;
    }

    public static T? MinBy<T>(this IEnumerable<T> items, Func<T, IComparable> getKey)
    {
        var best = default(T);
        IComparable? bestKey = null;
        var found = false;
        foreach (var item in items)
            if (!found || getKey(item).CompareTo(bestKey) < 0)
            {
                best = item;
                bestKey = getKey(best);
                found = true;
            }

        return best;
    }

    public static T? MaxBy<T>(this IEnumerable<T> items, Func<T, IComparable> getKey)
    {
        var best = default(T);
        IComparable? bestKey = null;
        var found = false;
        foreach (var item in items)
            if (!found || getKey(item).CompareTo(bestKey) > 0)
            {
                best = item;
                bestKey = getKey(best);
                found = true;
            }

        return best;
    }

    public static int Sum(this IEnumerable<int> items, Func<int, int, int> selectWithIndex)
    {
        var i = 0;
        var sum = 0;
        foreach (var item in items)
        {
            sum += selectWithIndex(item, i);
            i++;
        }
        return sum;
    }

    public static long Product(this IEnumerable<int> items) => items.Aggregate(1L, (a, b) => a * b);
    public static long Product(this IEnumerable<long> items) => items.Aggregate(1L, (a, b) => a * b);
    public static long Product<T>(this IEnumerable<T> items, Func<T, long> map) => items.Aggregate(1L, (a, b) => a * map(b));

    public static int BoundTo(this int v, int left, int right)
    {
        if (v < left) return left;
        if (v > right) return right;
        return v;
    }

    public static int TruncateAbs(this int v, int maxAbs)
    {
        if (v < -maxAbs) return -maxAbs;
        if (v > maxAbs) return maxAbs;
        return v;
    }

    public static IEnumerable<T> Times<T>(this int count, Func<int, T> create)
    {
        return Enumerable.Range(0, count).Select(create);
    }

    public static IEnumerable<T> Times<T>(this int count, T item)
    {
        return Enumerable.Repeat(item, count);
    }

    public static bool InRange(this int v, int minInclusive, int maxExclusive) => 
        v >= minInclusive && v < maxExclusive;

    public static bool EqAt<T>(this T[][] matrix, V pos, T expectedValue) where T : IEquatable<T> 
        => matrix.ContainsIndices(pos) && matrix[pos.Y][pos.X].Equals(expectedValue);
    
    public static bool ContainsIndices<T>(this T[][] matrix, V v) => 
        matrix.ContainsIndices(v.Y, v.X);

    public static bool ContainsIndices<T>(this T[][] matrix, int rowIndex, int colIndex) => 
        rowIndex.InRange(0, matrix.Length) && colIndex.InRange(0, matrix[rowIndex].Length);

    public static int IndexOf<T>(this IReadOnlyList<T> readOnlyList, T value)
    {
        var count = readOnlyList.Count;
        var equalityComparer = EqualityComparer<T>.Default;
        for (var i = 0; i < count; i++)
        {
            var current = readOnlyList[i];
            if (equalityComparer.Equals(current, value)) return i;
        }

        return -1;
    }

    public static Dictionary<TK, int> CountFrequency<T, TK>(this IEnumerable<T> items, Func<T, TK> getKey) where TK : notnull
    {
        return items.Select(getKey).CountFrequency();
    }

    public static Dictionary<T, int> CountFrequency<T>(this IEnumerable<T> items) where T : notnull
    {
        var freq = new Dictionary<T, int>();
        foreach (var item in items)
            freq[item] = freq.GetValueOrDefault(item) + 1;
        return freq;
    }

    public static TV GetValueOrCreate<TK, TV>(this IDictionary<TK, TV> d, TK key, Func<TK, TV> create)
    {
        if (d.TryGetValue(key, out var v)) return v;
        return d[key] = create(key);
    }

    public static int UpdateMax<TK>(this IDictionary<TK, int> d, TK key, int newValue)
    {
        if (d.TryGetValue(key, out var current) && current >= newValue)
            return current;
        return d[key] = newValue;
    }

    public static int ElementwiseHashcode<T>(this IEnumerable<T> items)
    {
        unchecked
        {
            var r = 0;
            foreach (var item in items)
                r = (r * 379) ^ item?.GetHashCode() ?? 0;
            return r;
        }
    }

    public static int ToInt(this string s)
    {
        return int.Parse(s);
    }

    public static string Reverse(this string s)
    {
        return new(s.ToCharArray().Reverse().ToArray());
    }

    public static string StrJoin<T>(this IEnumerable<T>? items, string delimiter = "")
    {
        return items == null ? "" : string.Join(delimiter, items);
    }

    public static string StrJoin<T>(this IEnumerable<T> items, string delimiter, Func<T, string> toString)
    {
        return items.Select(toString).StrJoin(delimiter);
    }
}
