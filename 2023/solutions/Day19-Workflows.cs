// https://adventofcode.com/2023/day/19

public class Day19
{
    public record Rule(string Expr, string? Exit = null)
    {
        public string RatingName => Expr[..1];
        public char Op => Expr[1];
        public int Value => int.Parse(Expr[2..]);

        public bool Accept((string name, int value)[] part) => 
            Exit is null || AcceptRange.Contains(part.First(p => p.name == RatingName).value);

        public R AcceptRange => Op == '<' ? new R(1, Value - 1) : new R(Value + 1, 4000);
        public R RejectRange => Op == '<' ? new R(Value, 4000) : new R(1, Value);
    }

    public void Solve(
        [Separators("{},:")](string Name, Rule[] Rules)[] workflows, 
        [Separators("{=,}")](string Name, int Value)[][] parts)
    {
        var ws = workflows.ToDictionary(w => w.Name, w => w.Rules);

        bool IsAccepted((string name, int value)[] part)
        {
            var wName = "in";
            while (!wName.IsOneOf("A", "R"))
            {
                var rule = ws[wName].First(r => r.Accept(part));
                wName = rule.Exit ?? rule.Expr;
            }
            return wName == "A";
        }

        parts.Where(IsAccepted).Sum(p => p.Sum(r => r.Value)).Part1();

        long Dfs(string workflow, (string name, R range)[] curParts)
        {
            if (workflow == "A")
                return curParts.Product(p => p.range.Len);
            if (workflow == "R")
                return 0;
            var count = 0L;
            foreach (var rule in ws[workflow])
            {
                if (rule.Exit is null)
                {
                    count += Dfs(rule.Expr, curParts);
                    break;
                }

                var acceptParts = curParts
                    .Select(p =>
                        p.name == rule.RatingName
                            ? p with { range = p.range.SafeIntersectWith(rule.AcceptRange) }
                            : p)
                    .ToArray();
                if (acceptParts.All(p => p.range.Len > 0))
                    count += Dfs(rule.Exit, acceptParts);
                curParts = curParts
                    .Select(
                        p =>
                            p.name == rule.RatingName
                                ? p with { range = p.range.SafeIntersectWith(rule.RejectRange) }
                                : p).ToArray();
            }

            return count;
        }

        var count = Dfs("in", new[]
        {
            ("x", new R(1, 4000)),
            ("m", new R(1, 4000)),
            ("a", new R(1, 4000)),
            ("s", new R(1, 4000))
        });
        count.Part2();
    }
}