using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day11
{
    [Template("""
    Monkey @Index:
      Starting items: @Items
      Operation: new = old @Operation @Argument
      Test: divisible by @Divisor
        If true: throw to monkey @TrueDestination
        If false: throw to monkey @FalseDestination
    """)]
    public record Monkey(
        int Index,
        List<long> Items,
        char Operation, 
        string Argument,
        int Divisor, 
        int TrueDestination,
        int FalseDestination)
    {
        public int Activity { get; set; }
    };

    public void Part1(params Monkey[] monkeys)
    {
        RunMonkeys(monkeys, w => w / 3).ElementAt(19)
            .OrderByDescending(m => m.Activity)
            .Take(2).Product(x => x.Activity)
            .Out("Part 1: ");
    }

    public void Part2(params Monkey[] monkeys)
    {
        var modulo = monkeys.Product(m => m.Divisor);
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
                    if (newWorry % monkey.Divisor == 0)
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
}
