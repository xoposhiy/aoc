using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
// ReSharper disable once CheckNamespace

public static class Extensions
{
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

    public static void Out(this object? value, string prefix = "")
    {
        Console.Write(prefix);
        Console.WriteLine(value.Format());
    }
    public static Stack<T> ToStack<T>(this IEnumerable<T> source)
    {
        return new Stack<T>(source);
    }
    public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
    {
        return new Queue<T>(source);
    }

    public static bool IsCompactType(this Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(decimal);
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

    public static IEnumerable<T> EveryNth<T>(this IEnumerable<T> items, int n, int startFrom = 0)
    {
        var i = 0;
        foreach (var item in items)
        {
            if (i % n == startFrom)
                yield return item;
            i++;
        }
    }

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
        TV v;
        if (d.TryGetValue(key, out v)) return v;
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
