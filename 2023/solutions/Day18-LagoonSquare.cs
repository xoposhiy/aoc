// https://adventofcode.com/2023/day/18

public class Day18
{
    public void Solve([Separators(" ()#")] (string dir, int steps, string color)[] lines)
    {
        var steps = lines.Select(line => V.FromCompass(line.dir) * line.steps).ToList();
        GetSquare(steps).Part1();
        
        var dirs = new[] { V.Right, V.Down, V.Left, V.Up };
        var stepsPart2 = lines.Select(line =>
            dirs[Convert.ToInt32(line.color[^1..])] * Convert.ToInt32(line.color[..^1], 16)).ToList();
        GetSquare(stepsPart2).Part2();
    }

    private static long GetSquare(IReadOnlyList<V> steps)
    {
        var square = steps.Aggregate(
            (s: 0L, y: 0L), 
            (acc, d) => (acc.s + d.X * acc.y, acc.y + d.Y)).s;
        var perimiter = steps.Sum(s => s.MLen);
        return Abs(square) + perimiter / 2 + 1;
    }
}