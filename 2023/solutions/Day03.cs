using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

// https://adventofcode.com/2023/day/3
public class Day03
{
    public void Solve(char[][] map)
    {
        var sp = map.GetPositions(c => !char.IsDigit(c) && c != '.').ToHashSet();
        Console.WriteLine(sp.Count);
        var numbers = new List<(V nPos, V sPos, long number, char symbol)>();
        for (int y = 0; y < map.Length; y++)
        {
            long number = 0;
            List<V> symbols = new List<V>();
            for (int x = 0; x < map[0].Length; x++)
            {
                if (char.IsDigit(map[y][x]))
                {
                    number = number * 10 + map[y][x] - '0';
                    symbols.AddRange(new V(x, y).Area8().Where(sp.Contains));
                }
                else
                {
                    if (symbols.Any())
                        numbers.AddRange(symbols.Select(s => (new V(x-1, y), s, number, map[s.Y][s.X])));
                    symbols.Clear();
                    number = 0;
                }
            }
            if (symbols.Any())
                numbers.AddRange(symbols.Select(s => (new V(map[0].Length-1, y), s, number, map[s.Y][s.X])));
            
        }

        numbers.GroupBy(k => (number:k.number, k.nPos)).Sum(t => t.Key.number).Part1();

        var gears = numbers.Distinct().Where(t => t.symbol == '*').GroupBy(t => t.sPos)
            .Where(g => g.Count() == 2).ToList();
        gears.Out();
        gears
            .Sum(t => t.Product(g => g.number)).Part2();
    }
}