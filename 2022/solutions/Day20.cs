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
        var next = Enumerable.Range(0, ns.Length).Skip(1).Append(0).ToArray();
        var prev = Enumerable.Range(0, ns.Length - 1).Prepend(ns.Length - 1).ToArray();

        void MoveForward(int x)
        {
            // from: a -> x -> b -> c
            // to:   a -> b -> x -> c
            var a = prev[x];
            var b = next[x];
            var c = next[b];
            next[a] = b;
            next[b] = x;
            next[x] = c;
            prev[b] = a;
            prev[x] = b;
            prev[c] = x;
        }

        for (int i = 0; i < times; i++)
        {
            var id = i % ns.Length;
            var period = ns.Length - 1;
            var shift = (ns[id] % period + period) % period;

            for (int j = 0; j < shift; j++)
                MoveForward(id);
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