using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable once ClassNeverInstantiated.Global


public record Move(string MoveToken, int Count, string FromToken, int From, string ToToken, int To);

public class Day05
{
    public void Solve(string[] stacksView, Move[] moves)
    {
        var stacks = ParseStacks(stacksView, 9);
        ExecutePart1(moves, stacks);
        Console.WriteLine($"Part1: {stacks.Select(s => s.Peek()).StrJoin()}");
        
        stacks = ParseStacks(stacksView, 9);
        ExecutePart2(moves, stacks);
        Console.WriteLine($"Part2: {stacks.Select(s => s.Peek()).StrJoin()}");
    }

    private static void ExecutePart1(Move[] moves, Stack<char>[] stacks)
    {
        foreach (var move in moves)
        {
            for (int i = 0; i < move.Count; i++)
            {
                var c = stacks[move.From - 1].Pop();
                stacks[move.To - 1].Push(c);
            }
        }
    }

    private static void ExecutePart2(Move[] moves, Stack<char>[] stacks)
    {
        foreach (var move in moves)
        {
            var tmp = new Stack<char>();
            for (int i = 0; i < move.Count; i++)
            {
                var c = stacks[move.From - 1].Pop();
                tmp.Push(c);
            }

            for (int i = 0; i < move.Count; i++)
                stacks[move.To - 1].Push(tmp.Pop());
        }
    }

    private static Stack<char>[] ParseStacks(string[] stacksView, int stacksCount)
    {
        var stacks = new Stack<char>[stacksCount];
        for (int i = 0; i < stacksCount; i++)
        {
            stacks[i] = new Stack<char>();
            var c = 1 + i * 4;
            for (int y = stacksView.Length - 2; y >= 0; y--)
            {
                if (stacksView[y][c] == ' ')
                    break;
                stacks[i].Push(stacksView[y][c]);
            }
        }

        return stacks;
    }
}