// https://adventofcode.com/2023/day/17

public class Day17
{
    public record SearchNode(V Pos, V Dir, int LastDirectionStepsCount);

    public void Solve(int[][] map)
    {
        IEnumerable<SearchNode> GetNext(SearchNode state) =>
            from next in map.NextSteps(state.Pos, state.Dir)
            let sameDir = next.Dir == state.Dir
            let newSteps = sameDir ? state.LastDirectionStepsCount + 1 : 1
            where newSteps <= 3
            select new SearchNode(next.Pos, next.Dir, newSteps);
        
        IEnumerable<SearchNode> GetNextUltraCrucible(SearchNode state) =>
            from next in map.NextSteps(state.Pos, state.Dir)
            let sameDir = next.Dir == state.Dir
            let newSteps = sameDir ? state.LastDirectionStepsCount + 1 : 1
            where state.Dir == V.Zero || sameDir && newSteps <= 10 || !sameDir && state.LastDirectionStepsCount >= 4 
            select new SearchNode(next.Pos, next.Dir, newSteps);

        GraphSearch.Dijkstra(GetNext, (a, b) => map.Get(b.Pos),
                new SearchNode(Pos: V.Zero, Dir: V.Zero, LastDirectionStepsCount: 0))
            .First(p => p.Path.State.Pos == map.BottomRight())
            .Cost.Part1();
        
        GraphSearch.Dijkstra(GetNextUltraCrucible, (a, b) => map.Get(b.Pos),
                new SearchNode(Pos: V.Zero, Dir: V.Zero, LastDirectionStepsCount: 0))
            .First(p => p.Path.State.Pos == map.BottomRight())
            .Cost.Part2();
    }
}