// https://adventofcode.com/2023/day/10

public class Day10
{
    private record Pipe(char Sym, V[] ExitDirs);
    private static readonly Pipe[] PipeSymbols = {
        new ('|', V.UpDown),
        new ('-', V.LeftRight),
        new ('L', V.RightUp),
        new ('J', V.LeftUp),
        new ('7', V.LeftDown),
        new ('F', V.RightDown),
        new ('S', V.Directions4),
    };
    
    public void Solve(char[][] map)
    {
        var startPos = map.GetPosition('S');
        var path = GraphSearch.Dfs(p => GetNext(p.State), startPos).MaxBy(p => p.Len)!;
        //path.VisualizeOnMap(map);
        var steps = path.StepsForward().ToList();
        var startSym = GetSymbol(new[] { steps.Last().to - startPos, steps[0].to - startPos }).Out("start symbol: ");
        map.Set(startPos, startSym);
        
        var loop = path.Select(p => p.State).ToHashSet();
        (loop.Count/2).Part1();
        
        var cellsCount = new[] {0, 0};
        for (int y = 0; y < map.Height(); y++)
        {
            var doubledCrosses = map.Width()*10;
            for (int x = 0; x < map.Width(); x++)
            {
                if (!loop.Contains(new V(x, y)))
                    cellsCount[doubledCrosses / 2 % 2]++;
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
        cellsCount[1].Part2();
        return;

        char GetSymbol(V[] exitDirs) => 
            PipeSymbols.FirstOrDefault(p => p.ExitDirs.SetEquals(exitDirs))?.Sym ?? throw new Exception(exitDirs.Format());

        IEnumerable<V> PossibleDirs(V pos) => 
            PipeSymbols.FirstOrDefault(p => p.Sym == map.Get(pos))?.ExitDirs ?? Enumerable.Empty<V>();

        IEnumerable<V> GetNext(V pos) => 
            PossibleDirs(pos).Where(d => PossibleDirs(pos + d).Contains(-d)).Select(d => pos + d);
    }
}