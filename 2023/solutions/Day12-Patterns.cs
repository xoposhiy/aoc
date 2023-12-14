// https://adventofcode.com/2023/day/12

using System.Numerics;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;

public class Day12
{
    public record Record(string Pattern, int[] Blocks)
    {
        public T CountWays<T>() where T : INumber<T>
        {
            //dp[i, j] - number of ways to place wirst j blocks to the Pattern[0..i] prefix.
            var dp = new T[Pattern.Length + 1, Blocks.Length + 1].FillWith(-T.One);

            T WayCount(int nChars, int nBlocks)
            {
                if (nChars <= 0) return nBlocks == 0 ? T.One : T.Zero;
                if (nBlocks < 0) return T.Zero;
                if (dp[nChars, nBlocks] != -T.One) return dp[nChars, nBlocks];
                var count = T.Zero;

                // Оставляем клетку пустой:
                if (Pattern[nChars-1] is not '#') 
                    count += WayCount(nChars-1, nBlocks);
                
                // На клетке можем закончить блок:
                if (nBlocks > 0 && FitBlockAt(nChars-Blocks[nBlocks-1], Blocks[nBlocks-1])) 
                    count += WayCount(nChars-Blocks[nBlocks-1]-1, nBlocks-1);
                return dp[nChars, nBlocks] = count;
            }

            var res = WayCount(Pattern.Length, Blocks.Length);
            
            //(dp.Cast<T>().Count(c => c != -T.One) / (Pattern.Length + 1.0) / (Blocks.Length + 1)).Out("dp size: ");

            
            return res;
        }

        public T CountWaysDp<T>() where T : INumber<T>
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

        public T CountWaysCached<T>() where T : INumber<T>
        {
            return CountWaysCached<T>(Pattern.Length, Blocks.Length);
        }
        
        [Cache]
        public T CountWaysCached<T>(int nChars, int nBlocks) where T : INumber<T>
        {
            if (nChars <= 0) return nBlocks == 0 ? T.One : T.Zero;
            if (nBlocks < 0) return T.Zero;
            var count = T.Zero;

            // Оставляем клетку пустой:
            if (Pattern[nChars-1] is not '#') 
                count += CountWaysCached<T>(nChars-1, nBlocks);
                
            // На клетке можем закончить блок:
            if (nBlocks > 0 && FitBlockAt2(nChars-Blocks[nBlocks-1], Blocks[nBlocks-1])) 
                count += CountWaysCached<T>(nChars-Blocks[nBlocks-1]-1, nBlocks-1);
            return count;
        }
        
        bool FitBlockAt(int start, int len)
        {
            if (start < 0) return false;
            if (start+len > Pattern.Length) return false;
            return 
                Pattern[start..(start + len)].All(c => c is not '.')  // no holes in block
                &&  (start == 0 || Pattern[start - 1] is not '#'); // no # just before

        }

        [Cache]
        bool FitBlockAt2(int start, int len)
        {
            if (start < 0) return false;
            if (start+len > Pattern.Length) return false;
            if (len == 0) return start == 0 || Pattern[start - 1] is not '#';
            return Pattern[start+len-1] is not '.' && FitBlockAt2(start, len-1); // no # just before
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
        CachingServices.DefaultBackend = new MemoryCachingBackend();
        lines.Sum(l => l.CountWays<int>()).Part1();
        lines.Sum(l => l.Unfold(5).CountWays<long>()).Part2();
        // lines.Sum(l => l.CountWaysDp<int>()).Out("DP1: ");
        // lines.Sum(l => l.Unfold(10).CountWays<decimal>()).Out("Part 3: ");
        // lines.Sum(l => l.Unfold(10).CountWays<BigInteger>()).Out("Part 4: ");
        //lines.Sum(l => l.CountWaysCached<int>()).Out("Cached 1: ");
    }
}