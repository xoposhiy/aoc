// https://adventofcode.com/2023/day/10

public class Day10
{
    public void Solve(char[][] map)
    {
        /*
| is a vertical pipe connecting north and south.
- is a horizontal pipe connecting east and west.
L is a 90-degree bend connecting north and east.
J is a 90-degree bend connecting north and west.
7 is a 90-degree bend connecting south and west.
F is a 90-degree bend connecting south and east.
. is ground; there is no pipe in this tile.
S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.
         */
        IEnumerable<V> GetNext(PathItem<V> pos)
        {
            if (pos.Prev == null)
            {
                if (map.Get(pos.State + V.Up).IsOneOf('|', 'F', '7'))
                    yield return pos.State + V.Up;
                if (map.Get(pos.State + V.Down).IsOneOf('|', 'L', 'J'))
                    yield return pos.State + V.Down;
                if (map.Get(pos.State + V.Left).IsOneOf('-', 'L', 'F'))
                    yield return pos.State + V.Left;
                if (map.Get(pos.State + V.Right).IsOneOf('-', 'J', '7'))
                    yield return pos.State + V.Right;
            }
            else
            {
                var sym = map.Get(pos.State);
                var d = pos.State - pos.Prev.State;
                if (sym is '|' or '-') yield return pos.State + d;
                if (sym is '7' && d == V.Up) yield return pos.State + V.Left;
                if (sym is '7' && d == V.Right) yield return pos.State + V.Down;
                
                if (sym is 'L' && d == V.Down) yield return pos.State + V.Right;
                if (sym is 'L' && d == V.Left) yield return pos.State + V.Up;

                if (sym is 'J' && d == V.Down) yield return pos.State + V.Left;
                if (sym is 'J' && d == V.Right) yield return pos.State + V.Up;

                if (sym is 'F' && d == V.Up) yield return pos.State + V.Right;
                if (sym is 'F' && d == V.Left) yield return pos.State + V.Down;

            }
        }

        var start = map.GetPosition('S');
        var path = GraphSearch.Bfs(GetNext, int.MaxValue, start);
        path.Max(p => p.Len).Part1();

        var left = new HashSet<V>();
        var right = new HashSet<V>();

        var me = new PathItem<V>(start, null, 0);
        while (true)
        {
            //L-
            var nextPos = GetNext(me).First();
            var d = nextPos - me.State;
            right.Add(me.State + d.RotateCW());
            left.Add(me.State + d.RotateCW());
            if (me.Prev != null)
            {
                var prevDir = me.State - me.Prev!.State;
                var other = me.State - d;
                if (prevDir.RotateCCW() == d)
                    right.Add(other);
                if (prevDir.RotateCW() == d)
                    left.Add(other);
            }

            if (nextPos == start)
            {
                break;
            }
            me = new PathItem<V>(nextPos, me, me.Len + 1);
        }
        // var used = path.Select(p => p.State).ToHashSet();
        left.ExceptWith(path.Select(p => p.State));
        right.ExceptWith(path.Select(p => p.State));
        left.Count().Out("left: ");
        right.Count.Out("right: ");
        
        var used = path.Select(p => p.State).ToHashSet();
        var startPositions = right;
        startPositions.Count.Out("count: ");
        
        var sum = 0;
        var res = new List<V>();
        foreach (V pos in startPositions)
        {
            if (used.Contains(pos)) continue;
            var paths2 = GraphSearch.Bfs(p => 
                p.State.Area4().Where(n => !used.Contains(n) && n.InRange(map)), int.MaxValue, pos);
            foreach (var item in paths2)
            {
                used.Add(item.State);
            }
            if (!paths2.Any(p => p.State.X.IsOneOf(0, map[0].Length - 1) || p.State.Y.IsOneOf(0, map.Length - 1)))
            {
                sum += paths2.Count;
                foreach (var item in paths2)
                {
                    res.Add(item.State);
                }
            }
            
        }
        
        sum.Part2();
        return;
    }

}