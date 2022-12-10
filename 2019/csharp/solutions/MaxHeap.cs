using System.Runtime.CompilerServices;

public class MaxHeap<T> where T : IComparable<T>
{
    private readonly List<T?> values;

    public MaxHeap()
    {
        values = new() { default };
    }

    public int Count => values.Count - 1;
    public T Max => values[1]!;

    public override string ToString()
    {
        var max = values.Count > 1 ? Max.ToString() : "NA";
        return $"Count = {values.Count} Max = {max}";
    }

    public bool TryDequeue(out T? max)
    {
        var count = Count;
        if (count == 0)
        {
            max = default;
            return false;
        }

        max = Max;
        values[1] = values[count];
        values.RemoveAt(count);

        if (values.Count > 1)
            BubbleDown(1);

        return true;
    }

    public void Add(T item)
    {
        values.Add(item);
        BubbleUp(Count);
    }

    private void BubbleUp(int index)
    {
        int parent = index / 2;

        while (index > 1 && CompareResult(parent, index) < 0)
        {
            Exchange(index, parent);
            index = parent;
            parent /= 2;
        }
    }

    private void BubbleDown(int index)
    {
        while (true)
        {
            var left = index * 2;
            var right = index * 2 + 1;

            int max;
            if (left < values.Count &&
                CompareResult(left, index) > 0)
                max = left;
            else
                max = index;
        
            if (right < values.Count &&
                CompareResult(right, max) > 0)
                max = right;

            if (max != index)
            {
                Exchange(index, max);
                index = max;
            }
            else
                return;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CompareResult(int index1, int index2) => 
        values[index1]!.CompareTo(values[index2]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Exchange(int index1, int index2) => 
        (values[index1], values[index2]) = (values[index2], values[index1]);
}