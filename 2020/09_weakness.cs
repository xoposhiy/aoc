using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Day9
{
    public void Solve()
    {
        var numbers = File.ReadLines("09.txt").Select(long.Parse).ToList();
        
        var invalidNumber = GetInvalidNumber(numbers, 25);
        Console.WriteLine($"Part One: {invalidNumber}");
        
        var seq = GetSliceWithSum(numbers, invalidNumber);
        Console.WriteLine($"Part Two: {seq.Min() + seq.Max()}");
    }

    private static long GetInvalidNumber(List<long> numbers, int size)
    {
        //                  window
        // numbers: 1 2 3 [4 5 6 7 8] 9 10 ...
        
        var window = new Queue<long>();
        foreach (var n in numbers)
        {
            if (window.Count == size)
            {
                if (!IsSum(n, window)) 
                    return n;
                window.Dequeue();
            }
            window.Enqueue(n);
        }
        throw new Exception("no weakness");
    }

    private static bool IsSum_Naïve(long targetSum, IReadOnlyCollection<long> window)
    {
        // 25*25 = 625
        foreach (var a in window)
            foreach (var b in window)
                if (a + b == targetSum)
                    return true;
        return false;
    }

    private static bool IsSum_Better(long targetSum, IReadOnlyCollection<long> window)
    {
        var set = window.ToHashSet();
        foreach (var a in window)
            if (set.Contains(targetSum - a))
                return true;
        return false;
    }

    private static bool IsSum(long targetSum, IReadOnlyCollection<long> window)
    {
        var set = window.ToHashSet();
        return window.Any(a => a != targetSum - a && set.Contains(targetSum - a)); // window.some(...)
    }


    public IReadOnlyCollection<long> GetSliceWithSum_Naïve(List<long> numbers, in long targetSum)
    {
        // targetSum = 8
        // [] 1 2 3 4 3 1 10

        var slice = new Queue<long>();
        foreach (var num in numbers)
        {
            slice.Enqueue(num);
            while (slice.Sum() > targetSum)
                slice.Dequeue();
            if (slice.Sum() == targetSum && slice.Count > 1)
                return slice;
        }
        throw new Exception("no weakness");
    }

    public IReadOnlyCollection<long> GetSliceWithSum(List<long> numbers, in long targetSum)
    {
        // targetSum = 8
        // [] 1 2 3 4 3 1 10
        
        var slice = new Queue<long>();
        long sliceSum = 0;
        foreach (var num in numbers)
        {
            sliceSum += num;
            slice.Enqueue(num);
            while (sliceSum > targetSum)
                sliceSum -= slice.Dequeue();
            if (sliceSum == targetSum && slice.Count > 1)
                return slice;
        }
        throw new Exception("no weakness");
    }
    

}