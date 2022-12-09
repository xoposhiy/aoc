public class Day09
{
    public void Solve((string Dir, int Len)[] moves)
    {
        IEnumerable<V> GetTailPositions(int size)
        {
            var rope = Enumerable.Repeat(V.Zero, size).ToList();
            foreach (var move in moves)
            {
                for (int i = 0; i < move.Len; i++)
                {
                    rope[0] += V.FromCompass(move.Dir);
                    for (int j = 0; j < rope.Count - 1; j++)
                    {
                        if (rope[j + 1].CDistTo(rope[j]) > 1)
                            rope[j + 1] += (rope[j] - rope[j + 1]).Sign();
                    }
                    yield return rope.Last();
                }
            }
        }
        Console.WriteLine($"Part1: {GetTailPositions(2).Distinct().Count()}");
        Console.WriteLine($"Part2: {GetTailPositions(10).Distinct().Count()}");
    }
}