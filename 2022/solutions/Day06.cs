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

        Console.WriteLine($"Part1: {FindMarker(4)}");
        Console.WriteLine($"Part2: {FindMarker(14)}");
    }
}