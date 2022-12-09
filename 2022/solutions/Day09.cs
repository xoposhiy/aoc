public class Day09
{
    // Даны список движений головы верёвки (aka змейки).
    // Следующий узел всегда касается предыдущего узла (в том числе по диагонали и в том числе может занимать ту же клетку)
    // При движении предыдущего узла, следующий подползает за ним.
    // Найти количество клеток, в которых побывал хвост веревки длины 2 (Part1) и длины 10 (Part2).
    public void Solve((string Dir, int Len)[] moves)
    {
        IEnumerable<V> GetTailPositions(int size)
        {
            var rope = size.Times(V.Zero).ToList();
            foreach (var move in moves)
            {
                for (int stepIndex = 0; stepIndex < move.Len; stepIndex++)
                {
                    rope[0] += V.FromCompass(move.Dir);
                    for (int iKnot = 0; iKnot < rope.Count - 1; iKnot++)
                    {
                        if (rope[iKnot + 1].CDistTo(rope[iKnot]) > 1)
                            rope[iKnot + 1] += (rope[iKnot] - rope[iKnot + 1]).Signum();
                    }
                    yield return rope.Last();
                }
            }
        }

        var part1 = GetTailPositions(2).Distinct().ToList();
        //part1.CreateMap("##", "  ").Out();
        Console.WriteLine($"Part1: {part1.Count}");

        var part2 = GetTailPositions(10).Distinct().ToList();
        //part2.CreateMap("##", "  ").Out();
        Console.WriteLine($"Part2: {part2.Count}");
    }
}