// https://adventofcode.com/2023/day/11

public class Day11
{
    public void Solve(char[][] map)
    {
        char[][] ExpandRow(char[] row)
            => row.All(c => c == '.') ? new[] { row, row } : new[] { row };

        map
            .SelectMany(ExpandRow).ToArray()
            .Columns()
            .SelectMany(ExpandRow).ToArray()
            .GetPositions('#')
            .Combinations(2)
            .Sum(c => c[0].MDistTo(c[1]))
            .Part1();
        
        
        var galaxies = map.GetPositions('#');
        SumPairDistances(2).Out("Part 1 v2: ");
        SumPairDistances(1000000).Part2();

        long SumPairDistances(int exp)
        {
            var xs = Expand(galaxies.Select(g => g.X), exp).ToList();
            var ys = Expand(galaxies.Select(g => g.Y), exp).ToList();
            return galaxies.Length
                .PairsIndices()
                .Sum(pair => Abs(xs[pair.left] - xs[pair.right]) + Abs(ys[pair.left] - ys[pair.right]));
        }
    }

    private static IEnumerable<long> Expand(IEnumerable<int> xs, int exp)
    {
        var values = xs.Order().ToList();

        return values.Bigrams()
            .SelectWithPrev(0L, (item, prev) => prev + Max(0, item.right - item.left - 1) * (exp - 1))
            .Zip(values.Bigrams(), (shift, pair) => pair.right + shift)
            .Prepend(values[0]);
    }
}