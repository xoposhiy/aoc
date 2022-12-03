using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day03
{
    public void Solve(string[] lines)
    {
        int Priority(char c) =>
            c switch
            {
                >= 'a' and <= 'z' => c - 'a' + 1,
                >= 'A' and <= 'Z' => c - 'A' + 27,
                _ => throw new Exception(c.ToString())
            };
        /*
         * Part1
         * Для каждой строки найти единственный символ, который есть и в левой половине строки и в правой.
         * Найти сумму приоритетов таких символов.
         */
        var commonItemsPrioritySum = lines
            .Select(line => line[..(line.Length / 2)].Intersect(line[(line.Length / 2)..]).Single())
            .Sum(Priority);
        Console.WriteLine($"Part1: {commonItemsPrioritySum}");

        /*
         * Part2
         * Для каждой тройки строк найти единственный символ, который есть во всех трех строках.
         * Найти сумму приоритетов таких символов.
         */
        var badgesPrioritySum = lines.GroupBy(3)
            .Select(group => group.IntersectAll().Single())
            .Sum(Priority);
        Console.WriteLine($"Part2: {badgesPrioritySum}");
    }
}