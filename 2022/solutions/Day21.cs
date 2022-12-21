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

        // humanFactor * humnValue + free
        var (humanFactorA, freeA) = GetHumanExpression(root.A, monkeys).Out("human expression for root.A: ");
        var (humanFactorB, freeB) = GetHumanExpression(root.B!, monkeys).Out("human expression for root.B: ");

        var humanValue1 = (freeB - freeA) / (humanFactorA - humanFactorB);
        humanValue1.Denominator.ShouldBe(1);
        humanValue1.Numerator.Part2("linear equation solution").ShouldBe(3219579395609);

        // Alternative solution. Binary search correct answer
        monkeys["root"] = root with { Op = "-" };
        var hmnValue = 0L;
        var step = 1000000000000000;
        while (step > 0)
        {
            var rootValue = ComputeValue(root.Name, monkeys, hmnValue);
            if (rootValue.Numerator > 0)
                hmnValue += step;
            else
                hmnValue -= step;
            step /= 2;
        }
        hmnValue++;
        hmnValue
            .Part2("answer bin search").ShouldBe(3219579395609);
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

    // h * humn + f
    private (Rat HumanFactor, Rat Free) GetHumanExpression(string name, IReadOnlyDictionary<string, Monkey> monkeys)
    {
        if (name == "humn") 
            return (1, 0);
        var m = monkeys[name];
        if (m.Op == null)
            return (0,long.Parse(m.A));
        var (leftH, leftF) = GetHumanExpression(m.A, monkeys);
        var (rightH, rightF) = GetHumanExpression(m.B!, monkeys);
        return m.Op switch
        {
            "+" => (leftH + rightH, leftF + rightF),
            "-" => (leftH - rightH, leftF - rightF),
            "*" when leftH*rightH == 0 => (leftH * rightF + leftF * rightH, leftF * rightF),
            "/" when rightH == 0 => (leftH / rightF, leftF / rightF),
            _ => throw new Exception(m.ToString())
        };
    }

    private Rat ComputeValue(string name, IReadOnlyDictionary<string, Monkey> monkeys, long? humanValue = null)
    {
        var (h, f) = GetHumanExpression(name, monkeys);
        return (humanValue ?? long.Parse(monkeys["humn"].A)) * h + f;
    }
}