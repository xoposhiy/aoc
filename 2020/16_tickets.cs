using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;

public class Day16
{
    public record ValueRange(int Min, int Max)
    {
        public bool Contains(int value) => value.InRange(Min, Max + 1);
    }

    public record FieldMeaning(string Name, ValueRange Range1, ValueRange Range2)
    {
        public bool Match(int value) => Range1.Contains(value) || Range2.Contains(value);
    }
    
    public void Solve()
    {
        var input = File.ReadAllText("16.txt").Split("\r\n\r\n");
        var fields
            = input[0].Split("\r\n")
            .Select(line => line.Split(new[] {": ", " or ", "-"}, StringSplitOptions.RemoveEmptyEntries))
            .Select(line => (n: line[0], rs: line.Skip(1).Select(int.Parse).ToList()))
            .Select(line => new FieldMeaning(line.n, new ValueRange(line.rs[0], line.rs[1]), new ValueRange(line.rs[2], line.rs[3])))
            .ToList();
        
        var allTickets = input[2].Split("\r\n").Skip(1).Select(line => line.Split(",").Select(int.Parse).ToList()).ToList();
        var errorRate = allTickets.SelectMany(t => t).Where(v => !fields.Any(f => f.Match(v))).Sum();
        Console.WriteLine($"Part One: {errorRate}");
        
        var tickets = allTickets.Where(t => t.All(v => fields.Any(r => r.Match(v)))).ToList();
        
        var meaningByField = Range(0, fields.Count)
            .Select(fi => 
                (fi, Range(0, fields.Count)
                    .Where(ri => tickets.All(t => fields[ri].Match(t[fi])))
                    .ToHashSet()))
            .FindBijection();

        var myTicket = input[1].Split("\r\n")[1].Split(",").Select(int.Parse).ToList();
        var ans = myTicket.Where((_, i) => fields[meaningByField[i]].Name.StartsWith("departure")).Aggregate(1L, (a, b) => a * b);

        Console.WriteLine(meaningByField.OrderBy(p => p.Key).Select(p => p.Value).StrJoin(","));
        Console.WriteLine($"Part Two: {ans}");
    }
}