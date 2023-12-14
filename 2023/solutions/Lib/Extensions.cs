using System.Collections;
using System.Numerics;

public record SequenceCycle<TNode>(TNode StartNode, long StartIndex, long PeriodLen);

public static class Extensions
{
    /// <summary>
    /// Select, witch can use previous outputed value to map current input value
    /// </summary>
    public static IEnumerable<TOut> Scan<TIn, TOut>(
        this IEnumerable<TIn> items, 
        TOut outSeed,
        Func<TIn, TOut, TOut> nextByCurrentInAndPrevOut)
    {
        yield return outSeed;
        var curr = outSeed;
        foreach (var item in items)
            yield return curr = nextByCurrentInAndPrevOut(item, curr);
    }
    
    public static char Translate(this char ch, string sourceChars, string targetChars)
    {
        var index = sourceChars.IndexOf(ch);
        return index < 0 ? ch : targetChars[index];
    }
    
    public static T[,] FillWith<T>(this T[,] array, T value)
    {
        for (var y = 0; y < array.GetLength(0); y++)
        for (var x = 0; x < array.GetLength(1); x++)
            array[y, x] = value;
        return array;
    }

    public static T Sum<TIn, T>(this IEnumerable<TIn> vs, Func<TIn, T> map) where T : INumber<T>
    {
        return vs.Aggregate(T.Zero, (a, x) => a + map(x));
    }
    
    public static string Translate(this string s, string sourceChars, string targetChars) => 
        s.Select(ch => ch.Translate(sourceChars, targetChars)).StrJoin();

    public static Array ToArray<T>(this IEnumerable<T> items, Type elementType)
    {
        var itemsList = items.ToList();
        var array = Array.CreateInstance(elementType, itemsList.Count);
        var i = 0;
        foreach (var item in itemsList)
            array.SetValue(item, i++);
        return array;
    }

    public static bool SetEquals<T>(this IEnumerable<T> a, IEnumerable<T> b) => a.ToHashSet().SetEquals(b);

    public static IEnumerable<T> ExceptNulls<T>(this IEnumerable<T?> items)
    {
        foreach (var item in items)
        {
            if (item is not null) yield return (T)item;
        }
    }

    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
    {
        foreach (var item in items)
            set.Add(item);
    }

    public static string SubstringSafe(this string s, int start, int len = int.MaxValue)
    {
        start = int.Max(0, start);
        len = int.Min(len, s.Length - start);
        return s.Substring(start, len);
    }

    public static IEnumerable<T> MakeSequence<T>(this T start, Func<T, T> next)
    {
        var current = start;
        yield return current;
        for (int i = 0; i < int.MaxValue; i++)
            yield return current = next(current);
    }
    
    public static IEnumerable<T> MakeSequence<T>(this T start, Func<T, int, T> next)
    {
        var current = start;
        yield return current;
        for (int i = 0; i < int.MaxValue; i++)
            yield return current = next(current, i);
    }

    public static bool IsOneOf<T>(this T item, params T[] set)
    {
        return set.IndexOf(item) >= 0;
    }

    public static IEnumerable<T> CycleShiftLeft<T>(this IList<T> items, int count)
    {
        count = count.ModPositive(items.Count);
        return items.Skip(count).Concat(items.Take(count));
    }
    
    /// <summary>
    /// Find cycle in sequence where next value is fully determined by the previous value
    /// </summary>
    /// <returns>Periodic part of sequence. Index - in the initial sequence</returns>
    public static List<(TNode node, int index)> GetCycle<TNode>(
        this IEnumerable<TNode> sequence, Func<TNode, long>? getHash = null)
    {
        if (sequence == null) throw new ArgumentNullException(nameof(sequence));
        getHash ??= v => v?.GetHashCode() ?? 0; 
        var period = new List<(TNode node, int index)>();
        var seenAt = new Dictionary<long, int>();
        var index = 0;
        var inCycle = false;
        var cycleStartHash = 0L; 
        foreach (var node in sequence)
        {
            var hash = getHash(node);
            if (!inCycle)
            {
                if (seenAt.TryGetValue(hash, out var prevIndex))
                {
                    cycleStartHash = hash;
                    period.Add((node, prevIndex));
                    inCycle = true;
                }
                else
                {
                    seenAt.Add(hash, index);
                }
            }
            else
            {
                if (getHash(node) == cycleStartHash)
                    return period;
                period.Add((node, period[^1].index+1));
            }
            index++;
        }
        throw new Exception("Unexpected end of sequence");
    }
    
    public static T ElementAtWithTrackingLoop<T>(this IEnumerable<T> items, long indexToFind, Func<T, long>? getHash = null, bool log = false)
    {
        var cycle = items.GetCycle(getHash);
        var skipCount = (int)((indexToFind - cycle[0].index) % cycle.Count);
        return cycle[skipCount].node;
    }
    
    public static string Format(this object? value)
    {
        if (value is null) return "";
        if (value is string s) return s;
        if (value is char[][] map)
        {
            return map.Select(row => new string(row)).StrJoin("\n") + "\n";
        }
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

    public static Stack<T> ToStack<T>(this IEnumerable<T> source)
    {
        return new Stack<T>(source);
    }

    public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
    {
        return new Queue<T>(source);
    }

    public static IEnumerable<T> Reversed<T>(this IEnumerable<T> source)
    {
        return source.Reverse();
    }

    public static IEnumerable<V> Indices<T>(this T[][] map)
    {
        return V.AllInRange(map[0].Length, map.Length);
    }

    public static IEnumerable<int> Indices<T>(this T[] map)
    {
        return Enumerable.Range(0, map.Length);
    }

    public static IEnumerable<T> EveryNth<T>(this IEnumerable<T> items, int n, int startFromIndex = 0)
    {
        return items.Skip(startFromIndex).Where((_, index) => index % n == 0);
    }

    public static Dictionary<TKey, TValue> FindBijection<TKey, TValue>(this IEnumerable<(TKey, HashSet<TValue>)> groups)
        where TKey : notnull
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

    public static IEnumerable<T[]> SplitBy<T>(this IEnumerable<T> items, Predicate<T> isSeparator, bool returnEmptyGroups = false)
        where T : IEquatable<T>?
    {
        var group = new List<T>();
        foreach (var item in items)
            if (isSeparator(item))
            {
                if (group.Count > 0 || returnEmptyGroups)
                {
                    yield return group.ToArray();
                    group.Clear();
                }
            }
            else
            {
                group.Add(item);
            }

        if (group.Count > 0 || returnEmptyGroups)
            yield return group.ToArray();
    }

    public static IEnumerable<T> JoinWith<T>(this IEnumerable<IEnumerable<T>> parts, T delimiter)
    {
        var index = 0;
        foreach (var part in parts)
        {
            if (index != 0) yield return delimiter;
            foreach (var item in part)
                yield return item;
            index++;
        }
    }

    public static IEnumerable<(T value, int length)> ConstantSegments<T>(this IEnumerable<T> items)
        where T : IEquatable<T>
    {
        var isFirst = true;
        T? value = default;
        var length = 1;
        foreach (var item in items)
        {
            if (!isFirst && item.Equals(value))
            {
                length++;
            }
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

    public static HashSet<TValue> IntersectAll<TKey, TValue>(this IEnumerable<TKey> items,
        Func<TKey, IEnumerable<TValue>> get)
    {
        return items.Select(get).IntersectAll();
    }

    public static HashSet<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> items)
    {
        HashSet<T>? set = null;
        foreach (var item in items)
            if (set == null) set = item as HashSet<T> ?? item.ToHashSet();
            else set.IntersectWith(item);
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

    public static long Product(this IEnumerable<int> items)
    {
        return items.Aggregate(1L, (a, b) => a * b);
    }

    public static long Product(this IEnumerable<long> items)
    {
        return items.Aggregate(1L, (a, b) => a * b);
    }

    public static long Product<T>(this IEnumerable<T> items, Func<T, long> map)
    {
        return items.Aggregate(1L, (a, b) => a * map(b));
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
    {
        return matrix.ContainsIndices(pos) && matrix[pos.Y][pos.X].Equals(expectedValue);
    }

    public static bool ContainsIndices<T>(this T[][] matrix, V v)
    {
        return matrix.ContainsIndices(v.Y, v.X);
    }

    public static bool ContainsIndices<T>(this T[][] matrix, int rowIndex, int colIndex)
    {
        return rowIndex.InRange(0, matrix.Length) && colIndex.InRange(0, matrix[rowIndex].Length);
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

    public static Dictionary<TK, int> CountFrequency<T, TK>(this IEnumerable<T> items, Func<T, TK> getKey)
        where TK : notnull
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
        return new string(s.ToCharArray().Reverse().ToArray());
    }

    public static string StrJoin<T>(this IEnumerable<T>? items, string delimiter = "")
    {
        return items == null ? "" : string.Join(delimiter, items);
    }

    public static string StrJoin<T>(this IEnumerable<T> items, string delimiter, Func<T, string> toString)
    {
        return items.Select(toString).StrJoin(delimiter);
    }
    
    public static IEnumerable<T[]> Window<T>(this IEnumerable<T> items, int windowSize)
    {
        var queue = new Queue<T>(windowSize);
        foreach (var item in items)
        {
            if (queue.Count == windowSize)
                queue.Dequeue();
            
            queue.Enqueue(item);
        
            if (queue.Count == windowSize)
                yield return queue.ToArray();
        }
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
    {
        return items.Select((v, i) => (v, i));
    }
    
    public static IEnumerable<T> Without<T>(this IEnumerable<T> items, params T[] values)
    {
        if (values.Length < 30)
            return items.Where(x => !values.Contains(x));
        var vs = values.ToHashSet();
        return items.Where(x => !vs.Contains(x));
    }

}