// https://adventofcode.com/2023/day/12

using System.Numerics;

public class Day12
{
    public record Record(string Pattern, int[] Blocks)
    {
        public T CountWays<T>() where T : INumber<T>
        {
            var count = new T[Pattern.Length + 1, Blocks.Length + 1];
            count[0, 0] = T.One;
            for (int i = 1; i <= Pattern.Length; i++)
            for (int nBlocks = 0; nBlocks <= Blocks.Length; nBlocks++)
            {
                // Оставляем клетку пустой:
                if (Pattern[i-1] is not '#') 
                    count[i, nBlocks] += count[i-1, nBlocks];

                if (nBlocks == 0) continue;
                // На клетке можем закончить блок:
                var len = Blocks[nBlocks - 1];
                if (!FitBlockAt(i-len, len)) continue;
                var gap = i == len ? 0 : 1;
                count[i, nBlocks] += count[i - len - gap, nBlocks-1];
            }
            return count[Pattern.Length, Blocks.Length];
        }

        bool FitBlockAt(int start, int len)
        {
            if (start < 0) return false;
            if (start+len > Pattern.Length) return false;
            return 
                Pattern[start..(start + len)].All(c => c is not '.')  // no holes in block
                &&  (start == 0 || Pattern[start - 1] is not '#'); // no # just before

        }

        public Record Unfold(int repeats)
        {
            var res = Pattern;
            var lens = Blocks.ToList();
            for (int i = 0; i < repeats-1; i++)
            {
                res += "?" + Pattern;
                lens.AddRange(Blocks);
            }

            return new Record(res, lens.ToArray());
        }
    }
    
    public void Solve(Record[] lines)
    {
        lines.Sum(l => l.CountWays<int>()).Part1();
        lines.Sum(l => l.Unfold(5).CountWays<long>()).Part2();
    }
}