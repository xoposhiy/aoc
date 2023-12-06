//namespace Aoc2023Day6CompositionDemo;

public record Range(long Start, long Length)
{
    public long End => Start + Length - 1;
    public Range? IntersectWith(Range range)
    {
        var startIntersect = Math.Max(Start, range.Start);
        var endIntersect = Math.Min(End, range.End);
        return startIntersect <= endIntersect 
            ? new Range(startIntersect, endIntersect - startIntersect + 1) 
            : null;        
    }

    public Range AddOffset(long offset) => this with { Start = Start + offset };

    private (Range? left, Range? right) Exclude(Range rangeToExclude)
    {
        Range? left = null;
        Range? right = null;
        if (Start < rangeToExclude.Start)
            left = new Range(Start, Math.Min(rangeToExclude.Start - Start, Length));
        if (End > rangeToExclude.End)
            right = new Range(Math.Max(Start, rangeToExclude.End + 1), Math.Min(Length, End - rangeToExclude.End));
        return (left, right);
    }

    public IEnumerable<Range> ExcludeAll(IEnumerable<Range> rangesToExclude)
    {
        var current = this;
        foreach (var rangeToExclude in rangesToExclude.OrderBy(r => r.Start))
        {
            var (prefix, suffix) = current.Exclude(rangeToExclude);
            if (prefix is not null) yield return prefix;
            if (suffix is null) yield break;
            current = suffix;
        }
        yield return current;
    }
}

public record Mapping(Range Source, long Offset)
{
    public Range? GetMappedRange(Range range) => 
        Source.IntersectWith(range)?.AddOffset(Offset);
}

public record Transform(Mapping[] Mappings)
{
    public Range[] ApplyTo(Range range)
    {
        // Отобразить все пересечения Mappings с range
        var mapped = Mappings
            .Select(m => m.GetMappedRange(range))
            .OfType<Range>(); // Убрать null-ы, полученные из mapping-ов, которые не пересекаются с range 

        // Все части range, не пересекающиеся с Mappings оставить как есть.
        var unchanged = range.ExcludeAll(Mappings.Select(m => m.Source));
        return mapped.Concat(unchanged).ToArray();
    }
    
    public Range[] ApplyTo(Range[] ranges) => 
        ranges.SelectMany(ApplyTo).ToArray();
}

public record Almanac(Range[] InputRanges, Transform[] Transforms)
{
    public long GetMinOutputNumber()
    {
        return Transforms.Aggregate(
                InputRanges, 
                (ranges, transform) => transform.ApplyTo(ranges))
            .Min(range => range.Start);
    }
}

public class Day5
{
    #region Parsing
    
    public void Solve(params string[][][] input)
    {
        var almanac = ParseAlmanac(input);
        var minOutputNumber = almanac.GetMinOutputNumber();
        Console.WriteLine($"Min output number: {minOutputNumber}");
    }

    private Almanac ParseAlmanac(string[][][] input)
    {
        var ranges = input[0][0].Skip(1).Select(long.Parse)
            .Chunk(2).Select(pair => new Range(pair[0], pair[1])).ToArray();
        var transforms = input.Skip(1).Select(ParseTransform).ToArray();
        return new Almanac(ranges, transforms);
    }

    private Transform ParseTransform(string[][] block)
    {
        var mappings = block.Skip(1)
            .Select(line => line.Select(long.Parse).ToArray())
            .Select(line => new Mapping(new Range(line[1], line[2]), line[0] - line[1]));
        return new Transform(mappings.ToArray());
    }
    
    #endregion
}