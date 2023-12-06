using static System.Math;

// https://adventofcode.com/2023/day/5
public class _Day05
{
    [Oneline]
    public record Mapping(long DestStart, long SrcStart, long Len)
    {
        public R Src => R.FromLen(SrcStart, Len);
        public R Dest => R.FromLen(DestStart, Len);
    }
    
    [Multiline]
    public record Stage(string Name, [Multiline]Mapping[] Ranges);

    public void Solve([Oneline] (string _, long[] seeds) header, [Multiline]params Stage[] stages)
    {
        var seeds = header.seeds;

        seeds.Select(seed => stages.Aggregate(seed, ApplyMapping)).Min()
            .Part1();

        var seedRanges =
            seeds.Chunk(2)
                .Select(chunk => R.FromLen(chunk[0], chunk[1]));

        stages.Aggregate(
                seedRanges,
                (currentRanges, stage) =>
                    currentRanges.SelectMany(range => ApplyMapping(range, stage)))
            .Min(r => r.Start)
            .Part2();
    }

    private static long ApplyMapping(long cur, Stage stage) =>
        cur + stage.Ranges
            .Where(r => r.Src.Contains(cur))
            .Select(r => r.Dest.Start - r.Src.Start)
            .FirstOrDefault(0);

    private static IEnumerable<R> ApplyMapping(R curRange, Stage stage) =>
        stage.Ranges.Select(mapRange =>
                curRange.IntersectWith(mapRange.Src)?.ShiftBy(mapRange.Dest.Start - mapRange.Src.Start))
            .ExceptNulls()
            .Concat(curRange.MinusAll(stage.Ranges.Select(r => r.Src)));


}