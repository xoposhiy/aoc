// https://adventofcode.com/2023/day/14

using System.Buffers;
using System.Diagnostics;

public class Day14
{
    public void Solve(char[][] map)
    {
        var backup = map.CloneMap();
        map = map.RotateCCW(); // top become left
        
        GetLoadOnLeftBorder(MoveRoundRocksLeft(map))
            .Part1();

        // Functional way
        var sw = Stopwatch.StartNew();
        var cycle = map
            .MakeSequence(ApplyOneCycle)
            .GetCycle(m => m.Format().GetHashCode());
        (cycle[0].index, cycle.Count).Out("1: ");
        var finalMap = cycle[(1_000_000_000 - cycle[0].index) % cycle.Count].node;
        GetLoadOnLeftBorder(finalMap)
            .Part2();
        sw.Elapsed.Out("Functional: ");
        
        map.RotateCWInplace();
        sw.Restart();


        for (int repeat = 0; repeat < 100; repeat++)
        {
            map = backup.CloneMap();
            // Imperative way
            
            cycle = map
                .MakeSequence(ApplyOneCycleInPlace)
                .GetCycle(m => m.Format().GetHashCode());
            (cycle[0].index, cycle.Count).Out("2: ");
            for (int i = 0; i < (1_000_000_000 - cycle[0].index) % cycle.Count; i++) 
                ApplyOneCycleInPlace(map);
            GetLoadOnUpperBorder(map)
                .Part2();
            
        }
        
        sw.Elapsed.Out("Imperative: ");
        
    }

    private char[][] MoveRoundRocksLeft(char[][] map) => 
        map.Select(MoveRoundRocksLeft).ToArray();

    private char[] MoveRoundRocksLeft_Slow(char[] row) =>
        row.SplitBy(c => c is '#', true)
            .Select(group => group.OrderBy(x => "O.".IndexOf(x))).JoinWith('#')
            .ToArray();

    private void MoveRoundRocksInPlace(char[][] map, int dir)
    {
        var size = map.Height();
        int?[] xs = { null, 0, null, size-1 };
        int[] dxs = { 0, 1, 0, -1 };
        int?[] ys = { 0, null, size-1, null };
        int[] dys = { 1, 0, -1, 0 };
        for (int i = 0; i < map.Height(); i++)
        {
            var x = xs[dir] ?? i;
            var y = ys[dir] ?? i;
            MoveRoundRocksInPlace(map, x, y, dxs[dir], dys[dir]);
        }    
    }
    
    private char[][] ApplyOneCycleInPlace(char[][] map)
    {
        for (int i = 0; i < 4; i++) 
            MoveRoundRocksInPlace(map, i);
        return map;
    }

    private void MoveRoundRocksInPlace(char[][] map, int x, int y, int dx, int dy)
    {
        var emptyCount = 0;
        var fillX = x;
        var fillY = y;
        var size = map.Width();
        for(int k=0; k<size; k++)
        {
            var t = map[y][x];
            switch (t)
            {
                case '#':
                {
                    for (var i = 0; i < emptyCount; i++)
                    {
                        map[fillY][fillX] = '.';
                        fillX += dx;
                        fillY += dy;
                    }
                    map[fillY][fillX] = '#';
                    fillX += dx;
                    fillY += dy;
                    emptyCount = 0;
                    break;
                }
                case '.':
                    emptyCount++;
                    break;
                case 'O':
                    map[fillY][fillX] = 'O';
                    fillX += dx;
                    fillY += dy;
                    break;
                default:
                    throw new Exception(t.ToString());
            }

            x += dx;
            y += dy;
        }
        for (var i = 0; i < emptyCount; i++)
        {
            map[fillY][fillX] = '.';
            fillX += dx;
            fillY += dy;
        }
    }

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

    private long GetLoadOnUpperBorder(char[][] map)
    {
        var load = 0L;
        for(int x = 0; x < map.Width(); x++)
        for (int y = 0; y < map.Width(); y++)
        {
            if (map[y][x] == 'O')
                load += map.Height() - y;
        }
        return load;
    }

    private long GetLoadOnLeftBorder(char[] row) => 
        row.Select((ch, index) => (ch, index))
            .Where(t => t.ch == 'O')
            .Sum(t => row.Length - t.index);

    private char[][] ApplyOneCycle(char[][] map) 
        => map.MakeSequence(m => MoveRoundRocksLeft(m).RotateCWInplace()).ElementAt(4);
}