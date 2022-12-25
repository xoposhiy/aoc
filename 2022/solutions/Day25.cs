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

    private long Parse(string arg)
    {
        long v = 0;
        foreach (var d in arg)
        {
            var digitValue = d switch
            {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2,
                _ => throw new Exception(d.ToString())
            };
            v = checked(v * 5 + digitValue);
        }
        return v;
    }
}