public class Day12
{
    public void Solve(char[][] map)
    {
        var s = map.GetPos('S').Out("Start: ");
        var e = map.GetPos('E').Out("End:   ");
        map[s.Y][s.X] = 'a';
        map[e.Y][e.X] = 'z';

        map.Bfs((f, t) => t - f <= 1, s)
            .First(p => p.Pos == e)
            .Distance
            .Out("Part 1: ");

        var starts = map.GetAll(c => c == 'a').ToArray();
        map.Bfs((f, t) => t - f <= 1, starts)
            .First(p => p.Pos == e)
            .Distance
            .Out("Part 2: ");
    }
}