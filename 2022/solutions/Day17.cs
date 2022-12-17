public class Day17
{
    // Есть 5 фиксированных фигурок тетриса, которые появляются по очереди циклически.
    // И стакан для тетриса, шириной в 7 клеток и бесконечной высотой.
    // Дана последовательность движений фигурок влево и вправо.
    //
    // Part 1:
    // Найти высоту башни, которая получится после того, как зафиксируется 2022 фигурки.
    //
    // Part 2:
    // Найти высоту башни, после того, как зафиксируется 1_000_000_000_000 фигурок.

    private const string Pieces = $$"""
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
        """ ;

    public void Solve(string movements)
    {
        var lines = Pieces.Split("\r\n");
        var pieces = lines.SplitBy(string.IsNullOrEmpty)
            .Select(ParsePiece)
            .ToArray();
        var seq = Simulate(movements, pieces).Take(5000).ToList();
        seq[2021].Out("Part 1: ");

        var diffs = seq.Zip(seq.Skip(1), (a, b) => b - a).Prepend(0).ToList();
        var s = diffs.StrJoin();
        var patternStart = s.Length - 30;
        var pattern = s[patternStart..^1];
        var start = s.IndexOf(pattern, 100);
        var nextStart = s.IndexOf(pattern, start+pattern.Length);
        var period = nextStart - start;
        (start, period).Out("loop_start, loop_period: ");
        var cycleSum = diffs.Skip(start).Take(period).Sum();
        var prefixSum = seq[start-1];
        var finalIndex = 1_000_000_000_000;
        long periodsCount = (finalIndex - start) / period;
        long rest = (finalIndex - start) % period;
        int restSum = diffs.Skip(start).Take((int)rest).Sum();
        var sum = prefixSum + periodsCount * cycleSum + restSum;
        sum.Out("Part 2: ");
    }

    private IEnumerable<int> Simulate(string movements, V[][] pieces)
    {
        var field = new HashSet<V>();
        var width = 7;
        var fixedPiecesCount = 0;

        V GetNewPiecePosition()
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
        var piecePos = GetNewPiecePosition();
        var piece = pieces[0];
        PrintField(field, piece, piecePos);
        while (true)
        {
            var movement = movements[movementIndex % movements.Length];
            piecePos = MovePiece(piece, piecePos, movement);
            movementIndex++;

            var newPos = DropPiece(piece, piecePos);
            if (newPos == piecePos)
            {
                foreach (var p in piece)
                    field.Add(p + piecePos);
                fixedPiecesCount++;
                piecePos = GetNewPiecePosition();
                piece = pieces[fixedPiecesCount % pieces.Length];
                yield return Math.Abs(field.Min(p => p.Y - 1));
            }
            else
            {
                piecePos = newPos;
            }
        }
        // ReSharper disable once IteratorNeverReturns
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