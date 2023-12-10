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