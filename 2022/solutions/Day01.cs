using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day01
{
    /*
     * Даны группы чисел разделенные пустой строкой.
     */
    public void Solve(params int[][] blocks)
    {
        var sums = blocks.Select(b => b.Sum()).ToList();

        /*
         * Part 1
         * Найти группу с максимальной суммой чисел и вывести эту сумму.
         */
        Console.WriteLine($"Part 1: {sums.Max()}");

        /*
         * Part 1
         * Найти три группы с максимальной суммой чисел и вывести эту сумму чисел из этих трех групп.
         */
        Console.WriteLine($"Part 2: {sums.OrderDescending().Take(3).Sum()}");
    }
}