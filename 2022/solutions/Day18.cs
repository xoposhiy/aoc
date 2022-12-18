public class Day18
{
    /* ƒаны координаты вокселей (3D-пикселей).
     *
     * Part 1:
     * Ќайти площадь поверхности фигуры из вокселей.
     *
     * Part 2:
     * Ќайти площадь внешней поверхности фигуры из вокселей. “о есть поверхности внешних полостей не считаютс€.
     */
    public void Solve(V3[] ps)
    {
        var set = ps.ToHashSet();
        ps.Sum(p => p.Area6().Count(n => !set.Contains(n)))
            .Out("Part 1: ");
        var maxVoxel = new V3(ps.Max(p => p.X) + 1, ps.Max(p => p.Y) + 1, ps.Max(p => p.Z) + 1);
        var diameterUpperBound =
            ps.Max(p => p.X) - ps.Min(p => p.X)
            + ps.Max(p => p.Y) - ps.Min(p => p.Y)
            + ps.Max(p => p.Z) - ps.Min(p => p.Z);
        diameterUpperBound.Out("Diameter: ");
        var outerBorder = GraphSearch.Bfs(
                p => p.State.Area6()
                    .Where(n => 
                        !set.Contains(n) && 
                        n.X <= maxVoxel.X && n.Y <= maxVoxel.Y && n.Z <= maxVoxel.Z), // “олько дл€ ускорени€
                diameterUpperBound+3, 
                starts:maxVoxel)
            .Select(p => p.State).ToHashSet();
        ps.Sum(p => p.Area6().Count(n => outerBorder.Contains(n)))
            .Out("Part 2: ");
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

