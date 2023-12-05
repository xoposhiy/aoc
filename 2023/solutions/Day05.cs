using static System.Math;

// https://adventofcode.com/2023/day/5
public class Day05
{
    public void Solve(params string[][] blocks)
    {
        var seeds = blocks[0][0].Split(" ").Skip(1).Select(long.Parse).ToArray();

        var stages = blocks.Skip(1).Select(ParseStage).ToList();

        long Process(long seed)
        {
            foreach (var stage in stages)
            foreach (var range in stage.Ranges)
                if (range.Src.Contains(seed))
                {
                    seed = range.Dest.Start + seed - range.Src.Start;
                    break;
                }

            return seed;
        }

        seeds.Select(Process).Min()
            .Part1();
        
        var cur = new List<R>();
        for (var index = 0; index < seeds.Length; index+=2)
            cur.Add(new R(seeds[index], seeds[index]+seeds[index+1]-1));
        var next = new List<R>();
        foreach (var stage in stages)
        {
            foreach (var currentSeedRange in cur)
            {
                foreach (var r2 in stage.Ranges)
                {
                    var intersection = currentSeedRange.IntersectWith(r2.Src);
                    if (intersection != null) next.Add(intersection + (r2.Dest.Start - r2.Src.Start));
                }
                var sourceRanges = stage.Ranges.Select(r => r.Src);
                next.AddRange(currentSeedRange.MinusAll(sourceRanges)); // unmapped
            }
            cur = next;
            next = new List<R>();
        }
        cur.Min(r => r.Start)
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