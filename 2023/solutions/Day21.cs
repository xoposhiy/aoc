// https://adventofcode.com/2023/day/21

public class Day21
{
    public void Solve(char[][] map)
    {
        var start = map.GetPosition('S');
        GraphSearch.Bfs(GetNext, 64, new SearchNode(start, 0))
            .Count(p => p is { Len: <= 64, State.Parity: 64 % 2 })
            .Part1();

        const int n = 26501365; // 131 * k + 131/2
        const int size = 131;
        var distances = GraphSearch.Bfs(GetNext, 800, new SearchNode(start, 0))
            .Select(p => p.Len).ToList();
        
        // Skip size/2 steps to start sequence from the border of the initial tile.
        Range(size/2, distances.Max())
            .EveryNth(size) // Watch at the moments new tiles are reached.
            .Select(d => distances.Count(s => s <= d && s % 2 == d % 2))
            .PredictArithmeticSequenceItem(n/size, log:false) // Hope it is the second order arithmetic sequence. 
            .Part2();
        return;

        IEnumerable<SearchNode> GetNext(PathItem<SearchNode> item) =>
            item.State.Pos.Area4()
                .Where(next => map.GetByModulo(next) is not '#')
                .Select(next => new SearchNode(next, 1-item.State.Parity));
    }
    
    public record struct SearchNode(V Pos, int Parity);
}
