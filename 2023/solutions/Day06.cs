﻿// https://adventofcode.com/2023/day/6

using Shouldly;
using static System.Math;

public class Day06
{
    public void Solve((string what, int[] records)[] lines)
    {
        var times = lines[0].records;
        var records = lines[1].records;

        int WaysToWinNaive(long time, long record) =>
            Enumerable.Range(1, (int)time).Count(button => button * (time - button) > record);

        int WaysToWin(long time, long record)
        {
            // x * (time - x) > record
            // x * time - x² > record
            // x² - x * time + record < 0
            // x = (time ± √(time² - 4 * record)) / 2
            var x1 = (time - Sqrt(time * time - 4 * record)) / 2.0;
            var x2 = (time + Sqrt(time * time - 4 * record)) / 2.0;
            return (int)Ceiling(x2) - (int)Floor(x1) - 1;
        }

        Enumerable.Range(0, times.Length)
            .Product(i => WaysToWin(times[i], records[i]))
            .Part1();

        var time = long.Parse(times.StrJoin("")).Out("time: ");
        var record = long.Parse(records.StrJoin("")).Out("duration: ");
        WaysToWin(time, record).Part2();
    }
}