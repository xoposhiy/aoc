// https://adventofcode.com/2023/day/17

public class Day17
{
    public record SearchNode(V Pos, V Dir, int Steps);
    
    public void Solve(int[][] map)
    {
        IEnumerable<SearchNode> GetNext(SearchNode state)
        {
            var (pos, dir, steps) = state;
            
            foreach (var nextDir in V.Directions4)
            {
                if (nextDir.X == -dir.X && nextDir.Y == -dir.Y) continue;
                var next = pos + nextDir;
                var newSteps = nextDir == dir ? steps + 1 : 0;
                if (nextDir == dir && newSteps == 3) continue;
                if (!next.InRange(map)) continue;
                var newCost = map.Get(next);
                yield return new SearchNode(next, nextDir, newSteps);
            }
        }
        
        IEnumerable<SearchNode> GetNext2(SearchNode state)
        {
            var (pos, dir, steps) = state;
            
            foreach (var nextDir in V.Directions4)
            {
                if (nextDir.X == -dir.X && nextDir.Y == -dir.Y) continue;
                var next = pos + nextDir;
                if (nextDir == dir && steps == 9) continue;
                if (nextDir != dir && steps < 3) continue;
                var newSteps = nextDir == dir ? steps + 1 : 0;
                if (!next.InRange(map)) continue;
                yield return new SearchNode(next, nextDir, newSteps);
            }
        }

        var path1 = GraphSearch.Dijkstra(GetNext, (a, b) => map.Get(b.Pos),
                new SearchNode(Pos: V.Zero, Dir: V.Left, Steps: -1))
            .First(p => p.path.State.Pos == new V(map.Width() - 1, map.Height() - 1));
        path1.cost.Part1();
        var path2 = GraphSearch.Dijkstra(GetNext2, (a, b) => map.Get(b.Pos),
                new SearchNode(Pos: V.Zero, Dir: V.Right, Steps: 0))
            .First(p => p.path.State.Pos == new V(map.Width() - 1, map.Height() - 1));
        //path2.path.VisualizeOne(p => p.Pos);
        path2.cost.Part2();
    }
}