using static System.Math;

// https://adventofcode.com/2023/day/5
public class Day05
{
    public void Solve(params string[][] blocks)
    {
        var seeds = blocks[0][0].Split(" ").Skip(1).Select(long.Parse).ToArray();

        var stages = blocks.Skip(1).Select(ParseStage).ToList();

        long Process(long seed) =>
            stages.Aggregate(
                seed, 
                (cur, stage) => 
                    cur + stage.Ranges
                        .Where(r => r.Src.Contains(cur))
                        .Select(r => r.Dest.Start - r.Src.Start)
                        .FirstOrDefault(0));

        seeds.Select(Process).Min()
            .Part1();
        
        var seedRanges = 
            seeds.Chunk(2)
                .Select(chunk => R.FromLen(chunk[0], chunk[1]))
                .ToList();
        stages.Aggregate(seedRanges, 
            (current, stage) => 
                current.SelectMany(seedRange => 
                        stage.Ranges.Select(mapRange => seedRange.IntersectWith(mapRange.Src)
                                ?.ShiftBy(mapRange.Dest.Start - mapRange.Src.Start))
                    .ExceptNulls()
                    .Concat(seedRange.MinusAll(stage.Ranges.Select(r => r.Src))))
                .ToList())
            .Min(r => r.Start)
            .Part2();
    }

    private Stage ParseStage(string[] lines)
    {
        var ranges = lines.Skip(1).Select(line => line.Split(" ").Select(long.Parse).ToArray())
            .Select(line => (R.FromLen(line[0],line[2]), R.FromLen(line[1], line[2])))
            .ToArray();
        return new Stage(ranges);
    }

    public record Stage((R Dest, R Src)[] Ranges);
}