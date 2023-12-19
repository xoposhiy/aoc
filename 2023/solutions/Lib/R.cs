#nullable enable
public record RStartLen(long Start, long Len) : R(Start, Start + Len - 1);

public record R(long Start, long End)
{
    public static R FromLen(long start, long len) => new(start, start + len - 1);
    public long Len => End - Start + 1;
    public override string ToString() => $"[{Start}..{End}]";

    public static R FromPoints(IReadOnlyCollection<int> points)
    {
        var start = points.Min();
        var end = points.Max();
        return new R(start, end);
    }

    public R ShiftBy(long value) => new(Start + value, End + value);
}

public static class RangeExtensions
{
    public static bool Contains(this R range, R other) => range.Start <= other.Start && range.End >= other.End;
    public static bool Contains(this R range, long p) => range.Start <= p && range.End >= p;
    public static bool IsIntersected(this R range, R other) => Math.Max(range.Start, other.Start) <= Math.Min(range.End, other.End);
    public static R? IntersectWith(this R range, R b)
    {
        var startIntersect = Math.Max(range.Start, b.Start);
        var endIntersect = Math.Min(range.End, b.End);
        return startIntersect > endIntersect ? null : new R(startIntersect, endIntersect);
    }

    public static R SafeIntersectWith(this R range, R b)
    {
        var startIntersect = Math.Max(range.Start, b.Start);
        var endIntersect = Math.Min(range.End, b.End);
        return startIntersect > endIntersect ? new R(startIntersect, startIntersect-1) : new R(startIntersect, endIntersect);
    }
    
    public static (R? left, R? right) Exclude(this R range, R subtrahend)  
    {
        R? left = null; 
        R? right = null; 
        if (range.Start < subtrahend.Start)
            left = new R(range.Start, Math.Min(subtrahend.Start - 1, range.End));
        if (range.End > subtrahend.End)
            right = new R(Math.Max(subtrahend.End + 1, range.Start), range.End);
        return (left, right);
    }

    public static IEnumerable<R> ExcludeAll(this R range, IEnumerable<R> rangesToExclude)  
    {
        foreach (var s in rangesToExclude.OrderBy(s => s.Start))
        {
            var (prefix, suffix) = range.Exclude(s);
            if (prefix is not null) yield return prefix;
            if (suffix is null) yield break;
            range = suffix;
        }
        yield return range;
    }
}