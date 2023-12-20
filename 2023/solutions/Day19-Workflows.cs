// https://adventofcode.com/2023/day/19

public class Day19
{
    public record Rule(string Expr, string? Exit = null)
    {
        public int RatingIndex => "xmas".IndexOf(Expr[0]);
        public char Op => Expr[1];
        public int Value => int.Parse(Expr[2..]);

        public bool Accept(int[] part) =>
            Exit is null || AcceptRange.Contains(part[RatingIndex]);

        public R AcceptRange => Op == '<' ? new R(1, Value - 1) : new R(Value + 1, 4000);
        public R RejectRange => Op == '<' ? new R(Value, 4000) : new R(1, Value);
    }

    public void Solve(
        [Separators("{},:")] (string Name, Rule[] Rules)[] workflows,
        [Separators("{=,xmas}")]int[][] parts)
    {
        var ws = workflows.ToDictionary(w => w.Name, w => w.Rules);

        bool IsAccepted(int[] part)
        {
            var wName = "in";
            while (!wName.IsOneOf("A", "R"))
            {
                var rule = ws[wName].First(r => r.Accept(part));
                wName = rule.Exit ?? rule.Expr;
            }

            return wName == "A";
        }

        parts.Where(IsAccepted).Sum(p => p.Sum()).Part1();

        long GetAcceptedCombinationsCount(string workflow, IList<R> curParts)
        {
            if (workflow == "A")
                return curParts.Product(p => p.Len);
            if (workflow == "R")
                return 0;
            var count = 0L;
            foreach (var rule in ws[workflow])
            {
                if (rule.Exit is null)
                {
                    count += GetAcceptedCombinationsCount(rule.Expr, curParts);
                    break;
                }

                var acceptParts = curParts.ToArray();
                acceptParts[rule.RatingIndex] = curParts[rule.RatingIndex].IntersectWith(rule.AcceptRange);
                if (acceptParts.All(range => range.Len > 0))
                    count += GetAcceptedCombinationsCount(rule.Exit, acceptParts);
                curParts[rule.RatingIndex] = curParts[rule.RatingIndex].IntersectWith(rule.RejectRange);
            }

            return count;
        }

        GetAcceptedCombinationsCount(
            "in",
            new[]
            {
                new R(1, 4000),
                new R(1, 4000),
                new R(1, 4000),
                new R(1, 4000)
            })
            .Part2();
    }
}