using Shouldly;

public class Day03
{
    // input:
    // ZNNvFWHqLNPZHHqPTHHnTGBhrrpjvmwfMmpfpjBjwpmw
    // ...
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
        lines
            .Select(line => line[..(line.Length / 2)].Intersect(line[(line.Length / 2)..]).Single())
            .Sum(Priority)
            .Out("Part 1: ").ShouldBe(7878);

        /*
         * Part2
         * Для каждой тройки строк найти единственный символ, который есть во всех трех строках.
         * Найти сумму приоритетов таких символов.
         */
        lines.Chunk(3)
            .Select(group => group.IntersectAll().Single())
            .Sum(Priority)
            .Out("Part 2: ").ShouldBe(2760);
    }
}