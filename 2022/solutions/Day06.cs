using Shouldly;
using static System.Linq.Enumerable;

public class Day06
{
    public void Solve(string inp)
    {
        // Найти первую позицию в строке (нумерация с 1), на которой заканчивается последовательность из markerSize уникальных символов.
        int FindMarker(int markerSize) =>
            Range(0, inp.Length - markerSize)
                .TakeWhile(i => inp[i..(i + markerSize)].Distinct().Count() < markerSize)
                .Count() + markerSize;

        FindMarker(4)
            .Out("Part 1: ").ShouldBe(1480);
        FindMarker(14)
            .Out("Part 2: ").ShouldBe(2746);
    }
}