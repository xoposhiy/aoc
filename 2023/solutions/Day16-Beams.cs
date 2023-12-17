// https://adventofcode.com/2023/day/16

public class Day16
{
    /*
     If the beam encounters empty space (.), it continues in the same direction.
If the beam encounters a mirror (/ or \), the beam is reflected 90 degrees depending on the angle of the mirror. For instance, a rightward-moving beam that encounters a / mirror would continue upward in the mirror's column, while a rightward-moving beam that encounters a \ mirror would continue downward from the mirror's column.
If the beam encounters the pointy end of a splitter (| or -), the beam passes through the splitter as if the splitter were empty space. For instance, a rightward-moving beam that encounters a - splitter would continue in the same direction.
If the beam encounters the flat side of a splitter (| or -), the beam is split into two beams going in each of the two directions the splitter's pointy ends are pointing. For instance, a rightward-moving beam that encounters a | splitter would split into two beams: one that continues upward from the splitter's column and one that continues downward from the splitter's column.
     */
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

        int Cnt(V pos, V dir)
        {
            var paths = GraphSearch.Bfs(GetNext, int.MaxValue, (pos, dir))
                ;
            return paths.Select(p => p.State.pos).Where(p => p.InRange(map)).Distinct().Count();
        }
        Cnt(new V(0, 0), V.Right).Part1();

        var max = 0;
        for (int i = 0; i < map.Width(); i++)
        {
            max = Max(max, Cnt(new V(0, i), V.Right));
            max = Max(max, Cnt(new V(map.Width()-1, i), V.Left));
        }
        for (int i = 0; i < map.Height(); i++)
        {
            max = Max(max, Cnt(new V(i, 0), V.Down));
            max = Max(max, Cnt(new V(i, map.Height()-1), V.Up));
        }
        
        max.Part2();
    }
}