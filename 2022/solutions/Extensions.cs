using System.Collections;
using System.Drawing;
using System.Linq;

public record PathItem(V Pos, PathItem? Prev, int Distance);
public static class Extensions
{
    public static bool HasBit(this int v, int bitIndex) => (v & (1 << bitIndex)) != 0;
    public static bool HasBit(this int v, char letter) => v.HasBit(char.ToLower(letter) - 'a');
    public static int SetBit(this int v, int bitIndex) => v | (1 << bitIndex);
    public static int SetBit(this int v, char letter) => v.SetBit(char.ToLower(letter) - 'a');
    public static int UnsetBit(this int v, int bitIndex) => v & ~(1 << bitIndex);
    public static int UnsetBit(this int v, char letter) => v.UnsetBit(char.ToLower(letter) - 'a');

    public static bool IsOneOf<T>(this T item, params T[] set)
    {
        return set.IndexOf(item) >= 0;
    }

    public static PathItem OutPath(this PathItem pathEnd, char[][]? map = null)
    {
        var pathItems = new List<PathItem>();
        for (var p = pathEnd; p != null; p = p.Prev)
            pathItems.Add(p);

        string DirChar(V next, V? prev) =>
            (next - (prev??next)) switch
            {
                (0, -1) => "↑",
                (0, 1) => "↓",
                (-1, 0) => "<",
                (1, 0) => "→",
                _ => "*"
            };

        pathItems
            .CreateMap(pi => pi.Pos, v=> map == null ? "." : (map.Get(v)+""), pi => DirChar(pi.Pos, pi.Prev?.Pos))
            .Out();
        return pathEnd;
    }

    public static IEnumerable<PathItem> Bfs<T>(this T[][] map, V[] neighbors, Func<T, T, bool> canPassFromTo, params V[] starts)
    {
        var visited = starts.ToHashSet();
        var queue = new Queue<PathItem>();
        foreach (var start in starts)
            queue.Enqueue(new(start, null, 0));
        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            yield return item;
            var from = map.Get(item.Pos);
            foreach (var next in neighbors.Select(d => item.Pos + d))
            {
                if (!next.InRange(map) || !canPassFromTo(from, map.Get(next)) || visited.Contains(next))
                    continue;
                visited.Add(next);
                queue.Enqueue(new(next, item, item.Distance + 1));
            }
        }
    }

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

    public static string Format(this object? value)
    {
        if (value is null) return "";
        if (value is string s) return s;
        if (value is IEnumerable e)
        {
            
            var parts = e.Cast<object>().Select(Format).ToList();
            if (parts.All(p => p.Length < 10))
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
    public static Stack<T> ToStack<T>(this IEnumerable<T> source)
    {
        return new Stack<T>(source);
    }
    public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
    {
        return new Queue<T>(source);
    }

    public static string[] FlipX(this string[] lines) => lines.Select(line => line.Reverse().StrJoin()).ToArray();
    public static IEnumerable<T> Reversed<T>(this IEnumerable<T> source) => source.Reverse();
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

    public static IEnumerable<IList<T>> GroupBy<T>(this IEnumerable<T> items, int groupSize) where T : notnull
    {
        var index = 0;
        var group = new List<T>();
        foreach (var item in items)
        {
            group.Add(item);
            index++;
            if (index == groupSize)
            {
                yield return group;
                group = new List<T>();
                index = 0;
            }
        }
        if (group.Count > 0)
            yield return group;
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

    public static Dictionary<T, int> CountFrequency<T>(this IEnumerable<T> items) where T : notnull
    {
        var freq = new Dictionary<T, int>();
        foreach (var item in items)
            freq[item] = freq.GetValueOrDefault(item) + 1;
        return freq;
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
    
    public static Dictionary<K, int> CountFrequency<T, K>(this IEnumerable<T> items, Func<T, K> getKey) where K : notnull
    {
        return items.Select(getKey).CountFrequency();
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

    public static long Product(this IEnumerable<int> items) => items.Aggregate(1L, (a, b) => a * b);
    public static long Product(this IEnumerable<long> items) => items.Aggregate(1L, (a, b) => a * b);
    public static long Product<T>(this IEnumerable<T> items, Func<T, long> map) => items.Aggregate(1L, (a, b) => a * map(b));
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

    public static bool InRange(this int v, int minInclusive, int maxExclusive)
    {
        return v >= minInclusive && v < maxExclusive;
    }
    public static bool EqAt<T>(this T[][] matrix, V pos, T expectedValue) where T : IEquatable<T> 
        => matrix.ContainsIndices(pos) && matrix[pos.Y][pos.X].Equals(expectedValue);
    
    public static bool ContainsIndices<T>(this T[][] matrix, V v) => matrix.ContainsIndices(v.Y, v.X);

    public static bool ContainsIndices<T>(this T[][] matrix, int i, int j)
    {
        return i.InRange(0, matrix.Length) && j.InRange(0, matrix[i].Length);
    }

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

    public static TV GetValueOrCreate<TK, TV>(this IDictionary<TK, TV> d, TK key, Func<TK, TV> create)
    {
        if (d.TryGetValue(key, out var v)) return v;
        return d[key] = create(key);
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

    public static string StrJoin<T>(this IEnumerable<T>? items, string delimiter = "")
    {
        return items == null ? "" : string.Join(delimiter, items);
    }

    public static string Reverse(this string s)
    {
        return new(s.ToCharArray().Reverse().ToArray());
    }

    public static string StrJoin<T>(this IEnumerable<T> items, string delimiter, Func<T, string> toString)
    {
        return items.Select(toString).StrJoin(delimiter);
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
}
