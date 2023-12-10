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