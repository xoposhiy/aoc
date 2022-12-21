using System.Diagnostics;
using Shouldly;

// ## Grove Positioning System
public class Day20
{
    // Дан массив чисел, который нужно интерпретировать, как циклический
    public void Solve(long[] ns)
    {
        // Каждое число  надо сдвинуть на столько позиций, чему равно число
        // Вернуть сумму чисел, которая получится в циклическом массиве на 1000, 2000 и 3000 позициях после нуля
        Decrypt(ns, ns.Length)
            .Out("Part 1: ").ShouldBe(2215);

        // Предварительно умножить каждое число в массиве на 811589153. И повторить процедуру 10 раз.
        // Вернуть сумму чисел, которая получится в циклическом массиве на 1000, 2000 и 3000 позициях после нуля
        var ns2 = ns.Select(x => x * 811589153).ToArray();
        Decrypt(ns2, ns.Length*10)
            .Out("Part 2: ").ShouldBe(8927480683);
    }


    private static long Decrypt(long[] ns, int times)
    {
        var n = ns.Length;
        var next = Enumerable.Range(0, n).Skip(1).Append(0).ToArray();
        var prev = Enumerable.Range(0, n - 1).Prepend(n - 1).ToArray();

        const int farStep = 25;
        var farNext = Enumerable.Range(0, n).Select(i => i % farStep == 0 ? (i + farStep) % n : -1).ToArray();
        var farNextLen = Enumerable.Repeat(farStep, n).Skip(1).Append(0).ToArray();

        void MoveItem(int x, int shift)
        {
            // a → x → b
            // Remove x
            var a = prev[x];
            var b = next[x];
            next[a] = b;
            prev[b] = a;

            // find closest prev far link
            var p = a;
            while (farNext[p] == -1)
                p = prev[p];
            // reduce its length because x is gone!
            farNextLen[p]--;
            // Oops, what if far link leads exactly to x?
            if (farNext[p] == x)
            {
                // redirect it to a instead. And next far link from x replace with same far link from a:
                farNext[p] = a;
                farNext[a] = farNext[x];
                farNextLen[a] = farNextLen[x];
                farNext[x] = -1;
            }

            // Fast search of new x position. Follow far links if can
            var len = shift;
            while (len > 0)
            {
                if (farNext[a] != -1 && farNextLen[a] <= len && farNextLen[a] > 1)
                {
                    len -= farNextLen[a];
                    a = farNext[a];
                }
                else
                {
                    len--;
                    a = next[a];
                }
            }
            b = next[a];
            
            // Insert x after a
            next[a] = x;
            prev[x] = a;
            next[x] = b;
            prev[b] = x;

            // Increase len of rev far link, because x is inserted.
            p = a;
            while (farNext[p] == -1)
                p = prev[p];
            farNextLen[p]++;
        }

        #pragma warning disable CS8321
        void MoveItemNoOptimization(int x, int shift)
        {
            // a → x → b
            // Remove x
            var a = prev[x];
            var b = next[x];
            next[a] = b;
            prev[b] = a;

            //Find new a and b
            for (int i = 0; i < shift; i++)
                a = next[a];
            b = next[a];

            // Insert x between new a and b
            next[a] = x;
            prev[x] = a;
            next[x] = b;
            prev[b] = x;
        }
        #pragma warning restore CS8321


        for (int i = 0; i < times; i++)
        {
            var id = i % ns.Length;
            var period = ns.Length - 1;
            var shift = (int)((ns[id] % period + period) % period);
            //MoveItemNoOptimization(id, shift);
            MoveItem(id, shift);
        }
        var decryptedRing = GetRingContentFromZeroValue(ns, next);
        var x1 = decryptedRing[1000 % ns.Length];
        var x2 = decryptedRing[2000 % ns.Length];
        var x3 = decryptedRing[3000 % ns.Length];
        return x1 + x2 + x3;
    }

    private static List<long> GetRingContentFromZeroValue(long[] ns, int[] next)
    {
        var res = new List<long>();
        var cur = ns.IndexOf(0);
        foreach (var _ in ns)
        {
            res.Add(ns[cur]);
            cur = next[cur];
        }
        return res;
    }
}