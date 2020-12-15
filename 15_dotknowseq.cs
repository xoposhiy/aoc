using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day15
{
    public void Solve()
    {
        var input = File.ReadAllText("15.txt").Split(",").Select(int.Parse).ToArray();
        Console.WriteLine($"Test: {GetNth(new []{0, 3, 6}, 2020)} ?= 436");
        Console.WriteLine($"Part One: {GetNth(input, 2020)}");
        Console.WriteLine($"Part Two: {GetNth(input, 30000000)}");
    }

    private int GetNth(int[] input, int n, bool log = false)
    {
        var d = new Dictionary<int, int>();
        var say = 0;
        var lastSay = input[0];
        for (int i = 1; i < n; i++)
        {
            if (i < input.Length)
                say = input[i];
            else
                say = i - d.GetValueOrDefault(lastSay, i);
            d[lastSay] = i;
            lastSay = say;
            if (log) Console.WriteLine(i + "\t" + say);
        }
        return say;
    }

}