using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;

public class Day17
{
    private string ps = $$"""
        ####

        .#.
        ###
        .#.

        ..#
        ..#
        ###

        #
        #
        #
        #

        ##
        ##
        """;
    public void Solve(string movements)
    {
        var lines = ps.Split("\r\n");
        V[][] pieces = lines.SplitBy(string.IsNullOrEmpty)
            .Select(ParsePiece)
            .ToArray();
        var field = new HashSet<V>();
        var seq = Simulate(movements, pieces, field).Take(5000).ToList();
        seq[2021].Out("Part 1: ");

        
        //PrintField(field, piece, piecePos);
        var diffs = seq.Zip(seq.Skip(1), (a, b) => b - a).Prepend(0).ToList();
        var s = diffs.StrJoin();
        
        s.Out();
        s.Length.Out("len: ");
        var patternStart = s.Length - 30;
        var pattern = s[patternStart..^1]
            //.Out("pattern")
            ;
        
        var start = s.IndexOf(pattern, 100);
        var nextStart = s.IndexOf(pattern, start+pattern.Length);
        var period = nextStart - start;
        (start, period).Out("Loop: ");
        diffs.Skip(start).Zip(diffs.Skip(start + period)).IndexOf(t => t.First != t.Second).Out("diffIndex ");
        
        var cycleSum = diffs.Skip(start).Take(period).Sum().Out("cycle sum: ");
        var prefixSum = seq[start-1];
        var query = 1000000000000;
        long periodsCount = (query - start) / period;
        long rest = (query - start) % period;
        int restSum = diffs.Skip(start).Take((int)rest).Sum();
        var sum = prefixSum + periodsCount * cycleSum + restSum;
        (prefixSum, periodsCount, rest, restSum, sum).Out("Part 2: ");
    }

    private IEnumerable<int> Simulate(string movements, V[][] pieces, HashSet<V> field)
    {
        var width = 7;
        var rocksStopped = 0;

        V PlacePiece(int pieceIndex)
        {
            var top = field.MinBy(p => p.Y)?.Y ?? 1;
            return new V(2, top - 4);
        }

        V MovePiece(V[] vs, V originalPos, char movement)
        {
            var pos = originalPos + (movement == '>' ? V.Right : V.Left);
            if (vs.Any(p => field.Contains(p + pos) || p.X + pos.X < 0 || p.X + pos.X >= width))
                return originalPos;
            return pos;
        }

        V DropPiece(V[] vs, V originalPos)
        {
            var pos = originalPos + V.Down;
            if (vs.Any(p => field.Contains(p + pos) || p.Y + pos.Y > 0))
                return originalPos;
            return pos;
        }

        var movementIndex = 0;
        var piecePos = PlacePiece(0);
        var piece = pieces[0];
        PrintField(field, piece, piecePos);
        while (true)
        {
            var movement = movements[movementIndex % movements.Length];
            //Console.WriteLine(movement + " " + movementIndex);
            var movedPos = MovePiece(piece, piecePos, movement);
            //if (movedPos != piecePos)
            {
                movementIndex++;
                piecePos = movedPos;
            }

            var newPos = DropPiece(piece, piecePos);
            if (newPos == piecePos)
            {
                foreach (var p in piece)
                    field.Add(p + piecePos);
                rocksStopped++;
                piecePos = PlacePiece(rocksStopped % pieces.Length);
                piece = pieces[rocksStopped % pieces.Length];
                var height = Math.Abs(field.Min(p => p.Y - 1));
                yield return height;
                //if (seq.Count %100 == 0)
                //seq.Out("Part 1: ");
            }
            else
            {
                piecePos = newPos;
            }
        }
    }

    private (int cycleStart, int period) FindSequencePeriod(List<int> sequence)
    {
        var a = 0;
        var b = 0;
        return (a, b);
    }

    private void PrintField(HashSet<V> field, V[] piece, V piecePos)
    {
        var top = (field.MinBy(p => p.Y)?.Y ?? 0) - 10;
        
        for (int y = top; y <= 0; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                if (piece.Any(p => p + piecePos == new V(x, y)))
                    Console.Write("O");
                else if (field.Contains(new V(x, y)))
                    Console.Write("#");
                else
                    Console.Write(".");
            }
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    private V[] ParsePiece(string[] lines)
    {
        var piece = new List<V>();
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                    piece.Add(new V(x, y - lines.Length+1));
            }
        }
        return piece.ToArray();
    }
}