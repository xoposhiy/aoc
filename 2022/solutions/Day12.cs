public class Day12
{
    public void Solve(char[][] map)
    {
        var startPos = map.GetPosition('S').Out("Start: ");
        var endPos = map.GetPosition('E').Out("End:   ");
        map.Set(startPos, 'a');
        map.Set(endPos, 'z');

        map.Bfs((f, t) => t - f <= 1, startPos)
            .First(p => p.Pos == endPos)
            .Distance
            .Out("Part 1: ");

        map.Bfs((f, t) => t - f <= 1, starts:map.GetPositions('a'))
            .First(p => p.Pos == endPos)
            .Distance
            .Out("Part 2 (forward from multiple starts):\n");
        
        map.Bfs((f, t) => f - t <= 1, starts:endPos)
            .First(p => p.Value == 'a')
            .Distance
            .Out("Part 2 (go backwards):\n");
    }
}