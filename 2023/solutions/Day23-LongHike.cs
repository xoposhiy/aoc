// https://adventofcode.com/2023/day/23

using Graph = System.Linq.ILookup<V, (V from, V to, int len)>;
public class Day23
{
    public void Solve(char[][] map)
    {
        IEnumerable<V> GetNext(V p)
        {
            if (map.Get(p) == '<') return new[] { p + V.Left };
            if (map.Get(p) == '>') return new[] { p + V.Right };
            if (map.Get(p) == '^') return new[] { p + V.Up };
            if (map.Get(p) == 'v') return new[] { p + V.Down };
            return GetNextIgnoringMarks(p);
        }

        IEnumerable<V> GetNextIgnoringMarks(V p) => p.Area4().Where(n => n.InRange(map) && map.Get(n) != '#');

        GetMaxPathLen(new V(1, 0), BuildGraph(GetNext))
            .Part1();

        GetMaxPathLen(new V(1, 0), BuildGraph(GetNextIgnoringMarks))
            .Part2();

        return;

        int GetMaxPathLen(V cur, Graph graph, HashSet<V>? visited = null)
        {
            if (visited == null)
                visited = new HashSet<V> { cur };
            if (cur.Y == map.Height() - 1)
                return 0;
            int bestLen = int.MinValue;
            foreach (var edge in graph[cur])
            {
                var n = edge.to;
                if (!visited.Add(n)) continue;
                var len = GetMaxPathLen(n, graph, visited) + edge.len;
                if (len > bestLen)
                    bestLen = len;
                visited.Remove(n);
            }

            return bestLen;
        }

        Graph BuildGraph(Func<V, IEnumerable<V>> getNext)
        {
            var edges = new HashSet<(V from, V to, int len)>();
            var start = new V(1, 0);
            var t = new Thread(() => CollectEdges(start, start, 0), 100_000_000);
            t.Start();
            t.Join();

            return edges.ToLookup(e => e.from);

            void AddEdge(V from, V to, int len)
            {
                edges.Add((from, to, len));
                edges.Add((to, from, len));
            }

            void CollectEdges(V cur, V prevVertice, int edgeLen, Dictionary<V, V?>? visitedFrom = null)
            {
                visitedFrom ??= new Dictionary<V, V?> { { cur, null } };
                if (cur.Y == map.Height() - 1)
                {
                    AddEdge(prevVertice, cur, edgeLen);
                    return;
                }

                var nextPoints = getNext(cur).ToList();
                var isCross = nextPoints.Count > 2;
                if (isCross) AddEdge(prevVertice, cur, edgeLen);
                foreach (var n in nextPoints)
                {
                    if (!visitedFrom.TryAdd(n, cur))
                    {
                        if (n != visitedFrom[cur]) AddEdge(prevVertice, n, edgeLen + 1);
                        continue;
                    }

                    if (isCross)
                        CollectEdges(n, cur, 1, visitedFrom);
                    else
                        CollectEdges(n, prevVertice, edgeLen + 1, visitedFrom);
                }
            }
        }
    }
}