using System.Text;
using Shouldly;

// https://adventofcode.com/2022/day/21
public class Day21
{
    public record Monkey(string Name, string A, string? Op=null, string? B = null);
    
    // Для каждой обезьяны дано либо её значение, либо формула, как вычислить ее значение, по значениям других обезьян.
    public void Solve([Separator(": ")] Monkey[] ms)
    {
        PrintGraphViz(ms);
        // Part1
        // Вычислить значение обезьяны по имени root
        var monkeys = ms.ToDictionary(m => m.Name);
        var root = monkeys["root"];
        ComputeValue(root.Name, monkeys)
            .Out("Part 1: ").ShouldBe(291425799367130);


        // Part1
        // Обезьяна root теперь сравнивает два своих аргумента на равенство.
        // А обезьяна humn − это вы. Какое значение должно быть у вас, чтобы у root получилось равенство?
        monkeys["root"] = root with { Op = "-" };
        // Binary search correct answer

        var hmnValue = 0L;
        var step = 1000000000000000;
        while (step > 0)
        {
            var rootValue = ComputeValue(root.Name, monkeys, new Dictionary<string, long> { { "humn", hmnValue }, });
            if (rootValue > 0)
                hmnValue += step;
            else
                hmnValue -= step;
            step /= 2;
        }
        hmnValue++;
        hmnValue
            .Out("Part 2: ").ShouldBe(3219579395609);
    }

    private void PrintGraphViz(Monkey[] ms)
    {
        var res = new StringBuilder("digraph Day21 {\n");
        res.AppendLine(" root[shape=box, color=red, style=filled];");
        res.AppendLine(" humn[shape=box, color=red, style=filled];");
        foreach (var monkey in ms.Where(m => m.Op != null))
        {
            res.AppendLine($"  {monkey.Name} -> {monkey.A};");
            res.AppendLine($"  {monkey.Name} -> {monkey.B};");
        }
        res.AppendLine("}");
        File.WriteAllText("Day21.gv", res.ToString());
    }

    private long ComputeValue(string name, IReadOnlyDictionary<string, Monkey> monkeys, Dictionary<string, long>? monkeyValues = null)
    {
        monkeyValues ??= new Dictionary<string, long>();
        if (monkeyValues.TryGetValue(name, out var v))
            return v;
        var m = monkeys[name];
        if (m.Op == null)
            return monkeyValues[name] = long.Parse(m.A);
        var a = ComputeValue(m.A, monkeys, monkeyValues);
        var b = ComputeValue(m.B!, monkeys, monkeyValues);
        var c = m.Op switch
        {
            "-" => a - b,
            "+" => a + b,
            "*" => a * b,
            "/" => a / b,
            _ => throw new Exception(m.Op)
        };
        return monkeyValues[name] = c;
    }
}