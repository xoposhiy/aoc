// https://adventofcode.com/2023/day/9

public class Day09
{
    public void Solve(long[][] hs)
    {
        var extrapolated = hs.Select(Extrapolate).ToList();
        extrapolated.Sum(e => e.next).Part1();
        extrapolated.Sum(e => e.prev).Part2();
        return;

        (long prev, long next) Extrapolate(IList<long> history)
        {
            if (history.All(v => v == 0)) return (0, 0);
            var diffs = history.Zip(history.Skip(1), (a, b) => b - a).ToList();
            var (prev, next) = Extrapolate(diffs);
            return (history[0] - prev, history[^1] + next);
        }
    }
}