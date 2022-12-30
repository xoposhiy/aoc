using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

// https://adventofcode.com/2022/day/25
public class Day25
{
    public void Solve(string[] lines)
    {
        var sum = lines.Select(FromSnufu).Sum();
        ToSnufu(sum).Part1();
    }

    private string ToSnufu(long num)
    {
        var snufu = "";
        while (num != 0)
        {
            var digit = (int)num.ModPositive(5);
            snufu = "012=-"[digit] + snufu;
            num = num / 5 + (digit > 2 ? 1 : 0);
        }
        return snufu;
    }

    private long FromSnufu(string snufu) => snufu
        .Select(d => "210-=".IndexOf(d)-2).Aggregate(0L, (num, digit) => num * 5 + digit);
}

