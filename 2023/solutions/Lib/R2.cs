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