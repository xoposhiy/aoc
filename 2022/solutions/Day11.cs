using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day11
{
    public void Solve(string[][] blocks)
    {
        Monkey[] monkeys = blocks.Select(ParseBlock).ToArray();
        
        RunMonkeys(monkeys, w => w / 3).ElementAt(19)
            .OrderByDescending(m => m.Activity)
            .Take(2).Product(x => x.Activity)
            .Out("Part 1: ");

        monkeys = blocks.Select(ParseBlock).ToArray();
        var modulo = monkeys.Product(m => m.IsDivisibleTo);
        RunMonkeys(monkeys, w => w % modulo).ElementAt(9999)
            .OrderByDescending(m => m.Activity)
            .Take(2).Product(x => x.Activity)
            .Out("Part 2: ");
    }

    private IEnumerable<Monkey[]> RunMonkeys(Monkey[] monkeys, Func<long, long> limitWorryLevel)
    {
        while (true)
        {
            foreach (var monkey in monkeys)
            {
                foreach (var worry in monkey.Items)
                {
                    long arg = monkey.Argument == "old" ? worry : monkey.Argument.ToInt();
                    var newWorry = checked(monkey.Operation == '+' ? (worry + arg) : (worry * arg));
                    newWorry = limitWorryLevel(newWorry);
                    if (newWorry % monkey.IsDivisibleTo == 0)
                        monkeys[monkey.TrueDestination].Items.Add(newWorry);
                    else
                        monkeys[monkey.FalseDestination].Items.Add(newWorry);
                }

                monkey.Activity += monkey.Items.Count;
                monkey.Items.Clear();
                //foreach (var m in monkeys) m.Items.Out(m.Index.ToString() + ": ");
            }



            yield return monkeys;
        }

    }

    private Monkey ParseBlock(string[] lines)
    {
        var index = lines[0].Split(':', ' ')[1].ToInt();
        var items = lines[1].Trim().Split(new[]{':', ' ', ','}, StringSplitOptions.RemoveEmptyEntries).Skip(2).Select(long.Parse).ToList();
        var op = lines[2].Substring("  Operation: new = old ".Length);
        var operation = op[0];
        var arg = op.Substring(2);
        var test = lines[3].Substring("  Test: divisible by ".Length).ToInt();
        var trueDestination = lines[4].Substring("    If true: throw to monkey ".Length).ToInt();
        var falseDestination = lines[5].Substring("    If false: throw to monkey ".Length).ToInt();
        return new Monkey(index, items, operation, arg, test, trueDestination, falseDestination);
    }

    private record Monkey(
        int Index,
        List<long> Items,
        char Operation,
        string Argument,
        int IsDivisibleTo, int TrueDestination, int FalseDestination, int Activity = 0)
    {
        public int Activity { get; set; } = Activity;
    };
}