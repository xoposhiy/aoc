using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day10
{
    public void Solve(string[][] lines)
    {
        var data = lines;
        var ip = 0;
        var cycle = 0;
        var x = 1;
        var sum = 0L;
        var pixels = new List<V>();

        void EndCycle()
        {
            cycle++;
            if ((cycle - 20) % 40 == 0)
            {
                var v = cycle * x;
                sum += v;
            }

            var col = (cycle-1)%40;
            var row = (cycle-1) / 40;
            var isLit = Math.Abs(col - x) <= 1;

            if (isLit)
            {
                pixels.Add(new V(col, row));
            }
        }
        while (ip < data.Length)
        {
            EndCycle();

            var cmd = data[ip];
             if (cmd[0] == "addx") 
            {
                EndCycle();
                x += cmd[1].ToInt();
            }
            ip++;
        }

        Console.WriteLine($"Part1: {sum}");

        //pixels.Out();

        Console.WriteLine($"Part2:\n{pixels.CreateMap().StrJoin("\n")}");
    }
}