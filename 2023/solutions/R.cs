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

    public static R operator +(R range, long value) => new(range.Start + value, range.End + value);
    public static R operator -(R range, long value) => new(range.Start - value, range.End - value);
}

public static class RangeExtensions
{
    public static bool Contains(this R range, R other)
    {
        return range.Start <= other.Start && range.End >= other.End;
    }

    public static bool Contains(this R range, long p)
    {
        return range.Start <= p && range.End >= p;
    }

    public static bool IsIntersected(this R range, R other)
    {
        return Math.Max(range.Start, other.Start) <= Math.Min(range.End, other.End);
    }

    public static R Add(this R range, long value)
    {
        return new R(range.Start + value, range.End + value);
    }

    public static R Subtract(this R range, long value)
    {
        return new R(range.Start - value, range.End - value);
    }

    public static R? IntersectWith(this R range, R b)
    {
        var startIntersect = Math.Max(range.Start, b.Start);
        var endIntersect = Math.Min(range.End, b.End);
        return startIntersect > endIntersect ? null : new R(startIntersect, endIntersect);
    }
    
    public static (R? left, R? right) Minus(this R range, R subtrahend)  
    {
        R? left = null; 
        R? right = null; 
        if (range.Start < subtrahend.Start)
            left = new R(range.Start, Math.Min(subtrahend.Start - 1, range.End));
        if (range.End > subtrahend.End)
            right = new R(Math.Max(subtrahend.End + 1, range.Start), range.End);
        return (left, right);
    }

    public static IEnumerable<R> MinusAll(this R range, IEnumerable<R> subtrahends)  
    {
        foreach (var s in subtrahends.OrderBy(s => s.Start))
        {
            var (left, right) = range.Minus(s);
            if (left is not null) yield return left;
            if (right is null) yield break;
            range = right;
        }
        yield return range;
    }
}


public record R2(V LeftTop, V RightBottom)
{
    public long Area => (long)Math.Max(0, RightBottom.X - LeftTop.X + 1) * Math.Max(0, RightBottom.Y - LeftTop.Y + 1);

    public static R2 FromPoints(IReadOnlyCollection<V> points)
    {
        var leftTop = new V(points.Min(p => p.X), points.Min(p => p.Y));
        var rightBottom = new V(points.Max(p => p.X), points.Max(p => p.Y));
        return new R2(leftTop, rightBottom);
    }

    public R2 Intersect(R2 other)
    {
        var leftTop = new V(Math.Max(LeftTop.X, other.LeftTop.X), Math.Max(LeftTop.Y, other.LeftTop.Y));
        var rightBottom = new V(Math.Min(RightBottom.X, other.RightBottom.X),
            Math.Min(RightBottom.Y, other.RightBottom.Y));
        return new R2(leftTop, rightBottom);
    }
}

public record V3(int X, int Y, int Z)
{
    public IEnumerable<V3> Area6()
    {
        yield return this with { X = X - 1 };
        yield return this with { X = X + 1 };
        yield return this with { Y = Y - 1 };
        yield return this with { Y = Y + 1 };
        yield return this with { Z = Z - 1 };
        yield return this with { Z = Z + 1 };
    }
}

public record BBox3(V3 Min, V3 Max)
{
    public int XSize => Max.X - Min.X + 1;
    public int YSize => Max.Y - Min.Y + 1;
    public int ZSize => Max.Z - Min.Z + 1;

    public static BBox3 FromPoints(IReadOnlyCollection<V3> points)
    {
        var minX = points.Min(p => p.X);
        var minY = points.Min(p => p.Y);
        var minZ = points.Min(p => p.Z);
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);
        var maxZ = points.Max(p => p.Z);
        return new BBox3(new V3(minX, minY, minZ), new V3(maxX, maxY, maxZ));
    }

    public bool Contains(V3 p)
    {
        return Min.X <= p.X && p.X <= Max.X &&
               Min.Y <= p.Y && p.Y <= Max.Y &&
               Min.Z <= p.Z && p.Z <= Max.Z;
    }

    public BBox3 Grow(int delta)
    {
        return new BBox3(Min with { X = Min.X - delta, Y = Min.Y - delta, Z = Min.Z - delta },
            Max with { X = Max.X + delta, Y = Max.Y + delta, Z = Max.Z + delta });
    }
}