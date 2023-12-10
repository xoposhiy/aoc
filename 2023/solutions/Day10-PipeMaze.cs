// https://adventofcode.com/2023/day/10

public class Day10
{
    public void Solve(char[][] map)
    {
        var startPos = map.GetPosition('S');
        var startSym = GetSymbol(GetNext(startPos).Select(n => n - startPos).ToArray()).Out("start symbol: ");
        map.Set(startPos, startSym);
        var loop = GraphSearch.Dfs(p => GetNext(p.State), startPos)
            .Select(p => p.State).ToHashSet();
        (loop.Count/2).Part1();
        
        var innerCellsCount = 0;
        for (int y = 0; y < map.Height(); y++)
        {
            // Consider the ray from (x, y) to (0, y).
            // Count the doubled number of crosses of this ray with loop border.
            var doubledCrosses = map.Width()*10;
            for (int x = 0; x < map.Width(); x++)
            {
                if (!loop.Contains(new V(x, y)))
                {
                    if (doubledCrosses / 2 % 2 == 1) innerCellsCount++;
                }
                else
                {
                    var c = map[y][x];
                    if (c == '|') doubledCrosses += 2;
                    if (c == 'L') doubledCrosses += 1;
                    if (c == 'J') doubledCrosses -= 1;
                    if (c == '7') doubledCrosses += 1;
                    if (c == 'F') doubledCrosses -= 1;
                }
            }
        }
        innerCellsCount.Part2();
        return;

        char GetSymbol(V[] exitDirs)
        {
            if (exitDirs.SetEquals(V.UpDown)) return '|';
            if (exitDirs.SetEquals(V.LeftRight)) return '-';
            if (exitDirs.SetEquals(V.RightUp)) return 'L';
            if (exitDirs.SetEquals(V.LeftUp)) return 'J';
            if (exitDirs.SetEquals(V.LeftDown)) return '7';
            if (exitDirs.SetEquals(V.RightDown)) return 'F';
            throw new Exception(exitDirs.Format());
        }

        IEnumerable<V> PossibleDirs(V pos) =>
            map.Get(pos) switch
            {
                '|' => V.UpDown,
                '-' => V.LeftRight,
                'L' => V.RightUp,
                'J' => V.LeftUp,
                '7' => V.LeftDown,
                'F' => V.RightDown,
                'S' => V.Directions4,
                _ => Enumerable.Empty<V>(),
            };

        IEnumerable<V> GetNext(V pos) => 
            PossibleDirs(pos).Where(d => PossibleDirs(pos + d).Contains(-d)).Select(d => pos + d);
    }
}