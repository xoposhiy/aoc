

// https://adventofcode.com/2022/day/24
public class Day24
{
    record Blizzard(V Pos, V Dir)
    {
        public V PosAt(int time, int w, int h) => 
            (Pos + Dir * time).Mod(w, h);
    }

    record State(int Width, int Height, Blizzard[] Blizzards, V Me)
    {
        public bool IsWall(V pos)
        {
            if (pos == new V(0, -1) || pos == new V(Width-1, Height)) return false;
            return pos.X < 0 || pos.X >= Width || pos.Y < 0 || pos.Y >= Height;
        }

        public void PrintAtTime(int time)
        {
            Console.WriteLine(time);
            var where = Blizzards.Select(b => (d:b.Dir, p:b.PosAt(time, Width, Height)))
                .ToLookup(bb => bb.p, bb => bb.d);
            where.CreateMap(g => g.Key, g => ".", g => g.Count() == 1 ? g.First().ToArrow() : g.Count().ToString(), V.Zero)
                .Out();
        }
    }

    public void Solve(string[] lines)
    {
        State state = ParseState(lines);
        var exit = new V(state.Width - 1, state.Height);
        var start = new V(0, -1);
        var therePath = GraphSearch.BfsLazy<(V me, int time)>(me => GetNextState(state, me), s => s.me == exit, (start, time:0)).Last();
        var there = therePath.State.time;
        there.Part1();
        var backPath = GraphSearch.BfsLazy<(V me, int time)>(me => GetNextState(state, me), s => s.me == start, (exit, there)).Last();
        var back = backPath.State.time.Out("back: ");
        var therePath2 = GraphSearch.BfsLazy<(V me, int time)>(me => GetNextState(state, me), s => s.me == exit, backPath.State);
        var thereWithSnacks = therePath2.Last().State.time;
        thereWithSnacks.Part2();
    }

    public static int lastDist = -1;
    public static HashSet<V> Blizzards = new HashSet<V>();
    
    private IEnumerable<(V me, int time)> GetNextState(State state, PathItem<(V me, int time)> path)
    {
        if (path.Distance != lastDist)
        {
            lastDist = path.Distance;
            Blizzards = state.Blizzards.Select(b => b.PosAt(path.State.time+1, state.Width, state.Height)).ToHashSet();
        }
        var me = path.State.me;
        var time = path.State.time+1;
        foreach (var d in V.Directions5)
        {
            var me2 = me + d;
            if (state.IsWall(me2)) continue;
            if (Blizzards.Contains(me2)) continue;
            //Console.WriteLine("-> " + me2 + " " + (path.Distance + 1));
            yield return (me2, time);
        }
    }

    private State ParseState(string[] lines)
    {
        var blizzards = new List<Blizzard>();
        var me = V.Zero;
        for (int y = 0; y < lines.Length-2; y++)
        {
            for (int x = 0; x < lines[y].Length-2; x++)
            {
                var c = lines[y+1][x+1];
                if (c == '>')
                    blizzards.Add(new Blizzard(new V(x, y), new V(1, 0)));
                else if (c == '<')
                    blizzards.Add(new Blizzard(new V(x, y), new V(-1, 0)));
                else if (c == 'v')
                    blizzards.Add(new Blizzard(new V(x, y), new V(0, 1)));
                else if (c == '^')
                    blizzards.Add(new Blizzard(new V(x, y), new V(0, -1)));
            }
        }
        return new State(lines[0].Length-2, lines.Length-2, blizzards.ToArray(), new V(0, -1));

    }
}