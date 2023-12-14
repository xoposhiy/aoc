// https://adventofcode.com/2023/day/14

public class Day14
{
    public void Solve(char[][] map)
    {
        map = map.RotateCCW(); // top become left
        
        GetLoadOnLeftBorder(MoveRoundRocksLeft(map)).Part1();

        GetLoadOnLeftBorder(
                map.MakeSequence(ApplyOneCycle)
                    .ElementAtWithTrackingLoop(1_000_000_000, m => m.Format().GetHashCode()))
            .Part2();
    }

    private char[][] MoveRoundRocksLeft(char[][] map) => 
        map.Select(MoveRoundRocksLeft).ToArray();

    private char[] MoveRoundRocksLeft(char[] row) =>
        row.SplitBy(c => c is '#', true)
            .Select(group => group.OrderBy(x => "O.".IndexOf(x))).JoinWith('#')
            .ToArray();

    private long GetLoadOnLeftBorder(char[][] map) =>
        map.Sum(GetLoadOnLeftBorder);

    private long GetLoadOnLeftBorder(char[] row) => 
        row.Select((ch, index) => (ch, index))
            .Where(t => t.ch == 'O')
            .Sum(t => row.Length - t.index);

    private char[][] ApplyOneCycle(char[][] map) => 
        map.MakeSequence(m => MoveRoundRocksLeft(m).RotateCW()).ElementAt(4);
}