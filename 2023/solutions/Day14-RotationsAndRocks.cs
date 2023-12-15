// https://adventofcode.com/2023/day/14

using System.Buffers;

public class Day14
{
    public void Solve(char[][] map)
    {
        map = map.RotateCCW(); // top become left
        
        GetLoadOnLeftBorder(MoveRoundRocksLeft(map))
            .Part1();

        var cycle = map
            .MakeSequence(ApplyOneCycle)
            .GetCycle(m => m.Format().GetHashCode());
        var finalMap = cycle[(1_000_000_000 - cycle[0].index) % cycle.Count].node;
        GetLoadOnLeftBorder(finalMap)
            .Part2();
    }

    private char[][] MoveRoundRocksLeft(char[][] map) => 
        map.Select(MoveRoundRocksLeft).ToArray();

    private char[] MoveRoundRocksLeft_Slow(char[] row) =>
        row.SplitBy(c => c is '#', true)
            .Select(group => group.OrderBy(x => "O.".IndexOf(x))).JoinWith('#')
            .ToArray();

    private char[] MoveRoundRocksLeft(char[] row)
    {
        var result = new char[row.Length];
        var emptyCount = 0;
        var j = 0;
        foreach (var t in row)
        {
            switch (t)
            {
                case '#':
                {
                    for(var i = 0; i < emptyCount; i++)
                        result[j++] = '.';
                    emptyCount = 0;
                    result[j++] = '#';
                    break;
                }
                case '.':
                    emptyCount++;
                    break;
                case 'O':
                    result[j++] = 'O';
                    break;
                default:
                    throw new Exception(t.ToString());
            }
        }
        for(var i = 0; i < emptyCount; i++)
            result[j++] = '.';
        return result;
    }

    private long GetLoadOnLeftBorder(char[][] map) =>
        map.Sum(GetLoadOnLeftBorder);

    private long GetLoadOnLeftBorder(char[] row) => 
        row.Select((ch, index) => (ch, index))
            .Where(t => t.ch == 'O')
            .Sum(t => row.Length - t.index);

    private char[][] ApplyOneCycle(char[][] map) 
        => map.MakeSequence(m => MoveRoundRocksLeft(m).RotateCWInplace()).ElementAt(4);
}