// https://adventofcode.com/2023/day/16

public class Day16
{
    public void Solve(char[][] map)
    {
        IEnumerable<(V pos, V dir)> GetNext(PathItem<(V pos, V dir)> state)
        {
            var (pos, dir) = state.State;
            var c = map.Get(pos);
            var dirs = new[] { dir };
            if (c == '/' && dir == V.Right) dirs = new[] { V.Up };
            if (c == '/' && dir == V.Left) dirs = new[] { V.Down };
            if (c == '/' && dir == V.Up) dirs = new[] { V.Right };
            if (c == '/' && dir == V.Down) dirs = new[] { V.Left };
            if (c == '\\' && dir == V.Right) dirs = new[] { V.Down };
            if (c == '\\' && dir == V.Left) dirs = new[] { V.Up };
            if (c == '\\' && dir == V.Up) dirs = new[] { V.Left };
            if (c == '\\' && dir == V.Down) dirs = new[] { V.Right };
            if (c == '|' && (dir == V.Right || dir == V.Left)) dirs = new[] { V.Up, V.Down };
            if (c == '-' && (dir == V.Up || dir == V.Down)) dirs = new[] { V.Right, V.Left };
            foreach (var d in dirs)
            {
                if ((pos + d).InRange(map))
                    yield return (pos + d, d);
            }
        }

        int GetPathLen(V pos, V dir)
        {
            var paths = GraphSearch.Bfs(GetNext, int.MaxValue, (pos, dir));
            return paths.Select(p => p.State.pos).Where(p => p.InRange(map)).Distinct().Count();
        }
        GetPathLen(new V(0, 0), V.Right)
            .Part1();

        var max = 0;
        for (int i = 0; i < map.Width(); i++)
        {
            max = Max(max, GetPathLen(new V(0, i), V.Right));
            max = Max(max, GetPathLen(new V(map.Width()-1, i), V.Left));
        }
        for (int i = 0; i < map.Height(); i++)
        {
            max = Max(max, GetPathLen(new V(i, 0), V.Down));
            max = Max(max, GetPathLen(new V(i, map.Height()-1), V.Up));
        }
        
        max.Part2();
    }
}