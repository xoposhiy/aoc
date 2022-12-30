public record R(int Start, int End)
{
    public static R FromPoints(IReadOnlyCollection<int> points)
    {
        var start = points.Min();
        var end = points.Max();
        return new R(start, end);
    }
    public bool Contains(R other) => Start <= other.Start && End >= other.End;
    public bool Contains(int p) => Start <= p && End >= p;
    public bool Overlaps(R other) => Math.Max(Start, other.Start) <= Math.Min(End, other.End);
    public int Len => End - Start + 1;
}

public record R2(V LeftTop, V RightBottom)
{
    public static R2 FromPoints(IReadOnlyCollection<V> points)
    {
        var leftTop = new V(points.Min(p => p.X), points.Min(p => p.Y));
        var rightBottom = new V(points.Max(p => p.X), points.Max(p => p.Y));
        return new R2(leftTop, rightBottom);
    }

    public R2 Intersect(R2 other)
    {
        var leftTop = new V(Math.Max(LeftTop.X, other.LeftTop.X), Math.Max(LeftTop.Y, other.LeftTop.Y));
        var rightBottom = new V(Math.Min(RightBottom.X, other.RightBottom.X), Math.Min(RightBottom.Y, other.RightBottom.Y));
        return new R2(leftTop, rightBottom);
    }

    public long Area => (long)Math.Max(0, RightBottom.X - LeftTop.X + 1) * Math.Max(0, RightBottom.Y - LeftTop.Y + 1);
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