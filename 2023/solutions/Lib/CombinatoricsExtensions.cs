public static class CombinatoricsExtensions
{
    
    public static IEnumerable<(T left, T right)> Pairs<T>(this IReadOnlyList<T> items)
    {
        for (int i = 0; i < items.Count; i++)
        for (int j = i + 1; j < items.Count; j++)
        {
            yield return (items[i], items[j]);
        }
    }

    public static IEnumerable<(T left, T right)> Bigrams<T>(this IReadOnlyList<T> items)
    {
        for (int i = 1; i < items.Count; i++)
            yield return (items[i-1], items[i]);
    }

    public static IEnumerable<(int left, int right)> PairsIndices(this int n)
    {
        for (int i = 0; i < n; i++)
        for (int j = i+1; j < n; j++)
            yield return (i, j);
    }

    public static IEnumerable<List<T>> Combinations<T>(this T[] items, int r)
    {
        int n = items.Length;

        if (r > n)
            yield break;

        int[] indices = Enumerable.Range(0, r).ToArray();

        yield return indices.Select(x => items[x]).ToList();

        while (true)
        {
            int i = indices.Length - 1;
            while (i >= 0 && indices[i] == i + n - r)
                i -= 1;

            if (i < 0)
                yield break;

            indices[i] += 1;

            for (int j = i + 1; j < r; j += 1)
                indices[j] = indices[j - 1] + 1;

            yield return indices.Select(x => items[x]).ToList();
        }
    }

    public static bool NextPermutation(this int[] a)
    {
        int j = a.Length - 2;
        while (j != -1 && a[j] >= a[j + 1])
            j--;
        if (j == -1)
            return false;
        int k = a.Length - 1;
        while (a[j] >= a[k])
            k--;
        (a[j], a[k]) = (a[k], a[j]);
        int l = j + 1, r = a.Length - 1;
        while (l < r)
        {
            int i = l++;
            int j1 = r--;
            (a[i], a[j1]) = (a[j1], a[i]);
        }

        return true;
    }

    public static IEnumerable<T[]> Permutations<T>(this T[] items)
    {
        int[] indices = Enumerable.Range(0, items.Length).ToArray();
        do
        {
            yield return indices.Select(x => items[x]).ToArray();
        } while (NextPermutation(indices));
    }
}