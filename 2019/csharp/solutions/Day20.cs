using Shouldly;

public class Day20
{
    public void Solve(char[][] map)
    {
        var labels = GetPortalLabels(map).ToList();
        var ps = GetPortals(labels);
        var aa = labels.First(t => t.name == "AA").pos;
        var zz = labels.First(t => t.name == "ZZ").pos;

        IEnumerable<V> GetNext(V pos)
        {
            if (ps.TryGetValue(pos, out var p))
                yield return p;
            foreach (var n in pos.Area4())
                if (map.Get(n) == '.') 
                    yield return n;
        }

        GraphSearch.BfsLazy(pathItem => GetNext(pathItem.State), aa)
            //.Visualize(map[0].Length, map.Length, v=>v)
            .First(p => p.State == zz)
            //.Visualize(v=>v)
            .Len.Part1().ShouldBe(484);

        IEnumerable<(V pos, int level)> GetNext2((V pos, int level) state)
        {
            bool isOuter = state.pos.X == 2 || 
                           state.pos.Y == 2 || 
                           state.pos.X == map[0].Length - 3 ||
                           state.pos.Y == map.Length - 3;
            if (ps.TryGetValue(state.pos, out var p))
            {
                if (!isOuter)
                    yield return (p, state.level + 1);
                else if (state.level > 0)
                    yield return (p, state.level - 1);
            }
            foreach (var n in state.pos.Area4())
            {
                if (map.Get(n) == '.')
                    yield return (n, state.level);
            }
        }

        GraphSearch.BfsLazy<(V pos, int level)>(pathItem => GetNext2(pathItem.State), (pos:aa, level:0))
            .First(p => p.State == (zz, 0))
            //.Visualize(v => v.pos, s => $"Level: {s.level}")
            .Len.Part2().ShouldBe(5754);
    }

    private Dictionary<V, V> GetPortals(IEnumerable<(V pos, string name)> labels)
    {
        var pairs = labels.ToLookup(kv => kv.name, kv => kv.pos);
        var ps = new Dictionary<V, V>();
        foreach (var pair in pairs)
        {
            if (pair.Count() == 2)
            {
                ps.Add(pair.First(), pair.Last());
                ps.Add(pair.Last(), pair.First());
            }
        }
        return ps;
    }

    private IEnumerable<(V pos, string name)> GetPortalLabels(char[][] map)
    {
        var used = new HashSet<V>();
        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[y].Length; x++)
            {
                var v = new V(x, y);
                if (used.Contains(v)) continue;
                var c = map[y][x];
                if (char.IsLetter(c))
                {
                    var right = map[y][x + 1];
                    var down = map[y+1][x];
                    if (char.IsLetter(right))
                    {
                        var name = "" + c + right;
                        used.Add(new V(x + 1, y));
                        if (x == 0 || x > map[y].Length/2 && x < map[y].Length-5)
                            yield return (new V(x + 2, y), name);
                        else
                            yield return (new V(x - 1, y), name);
                    }
                    else if (char.IsLetter(down))
                    {
                        var name = "" + c + down;
                        used.Add(new V(x, y + 1));
                        if (y == 0 || y > map.Length / 2 && y < map.Length - 5)
                            yield return (new V(x, y + 2), name);
                        else
                            yield return (new V(x, y - 1), name);
                    }
                    else
                    {
                        throw new Exception("oops");
                    }
                }
            }
        }
    }
}