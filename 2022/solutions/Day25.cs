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
        var numbers = lines.Select(Parse).ToList().Out();
        var sum = numbers.Sum().Out();
        ToSnufu(sum).Part1();
    }

    private string ToSnufu(long sum)
    {
        var snufu = "";
        while (sum != 0)
        {
            var digit = (int)(sum.ModPositive(5));
            snufu = "012=-"[digit] + snufu;
            var carry = digit > 2 ? 1 : 0;
            sum = sum / 5 + carry;
        }
        return snufu;
    }

    private long Parse(string arg) =>
        arg.Select(d => d switch
            {
                '2' or '1' or '0' => d - '0',
                '-' => -1,
                '=' => -2,
                _ => throw new Exception(d.ToString())
            })
            .Aggregate(0L, (current, digitValue) => checked(current * 5 + digitValue));
}