using Shouldly;

public class Day12
{
    public void Solve(char[][] map)
    {
        var startPos = map.GetPosition('S').Out("Start: ");
        var endPos = map.GetPosition('E').Out("End:   ");
        map.Set(startPos, 'a');
        map.Set(endPos, 'z');

        map.BfsLazy(V.Directions4, (f, t) => t - f <= 1, startPos)
            .First(p => p.Pos == endPos)
            .Len
            .Out("Part 1: ").ShouldBe(330);

        map.BfsLazy(V.Directions4, (f, t) => t - f <= 1, starts:map.GetPositions('a'))
            .First(p => p.Pos == endPos)
            .Len
            .Out("Part 2 (forward from multiple starts):\n").ShouldBe(321);
        
        map.BfsLazy(V.Directions4, (f, t) => f - t <= 1, starts:endPos)
            .First(p => map.Get(p.Pos) == 'a')
            .Len
            .Out("Part 2 (go backwards):\n").ShouldBe(321);
    }
}