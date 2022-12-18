using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;

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
        sums.Max()
            .Out("Part 1: ").ShouldBe(69912);

        /*
         * Part 1
         * Найти три группы с максимальной суммой чисел и вывести эту сумму чисел из этих трех групп.
         */
        sums.OrderDescending().Take(3).Sum()
            .Out("Part 2: ").ShouldBe(208180);
    }
}