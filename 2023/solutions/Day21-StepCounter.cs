// https://adventofcode.com/2023/day/21

public class Day21
{
    public void Solve(char[][] map)
    {
        var pos = map.GetPosition('S');
        var visited = new HashSet<(V pos, int odd)> {(pos, 0)};
        IEnumerable<(V pos, int dist)> GetNext(PathItem<(V pos, int dist)> item)
        {
            var (p, dist) = item.State;
            foreach (var dir in V.Directions4)
            {
                var next = p + dir;
                
                if (map.Get(new V(next.X.ModPositive(map.Width()), next.Y.ModPositive(map.Height()))) == '#') continue;
                if (visited.Add((next, (dist + 1) % 2)))
                    yield return (next, dist + 1);
            }
        }
        //var n = 5000
        //var period = 11;

        var n = 26501365;
        var size = 131;
        var paths = GraphSearch.Bfs(GetNext, 1000, (pos, dist:0));
        paths.Count(p => p.State.dist <= 64 && p.State.dist % 2 == 64 % 2).Part1();

        var distances = paths.Select(p => p.State.dist).ToList();
        Range(0, distances.Max()+1)
            .Skip(n % size)
            .EveryNth(size)
            .Select(d => distances.Count(s => s <= d && s % 2 == d % 2))
            .TryPredictSequence(n/size)
            .Part2();
    }
}
