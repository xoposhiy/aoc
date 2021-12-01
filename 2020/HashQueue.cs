﻿using System.Collections;
using System.Collections.Generic;

public class HashQueue : IReadOnlyCollection<int>
{
    private readonly Queue<int> queue = new();
    public long Hash { get; private set; }

    static HashQueue()
    {
        basePowers[0] = 1;
        for (var i = 1; i < basePowers.Length; i++)
            basePowers[i] = (basePowers[i - 1] * primeBase) % primeMod;
    }

    public HashQueue(IEnumerable<int> values)
    {
        foreach (var value in values)
            Enqueue(value);
    }

    public void Enqueue(int value)
    {
        // hash = x_0 * p^(size-1) + x_1 * p^(size-2) + ... + x_last =
        //      = p * (x_0 * p^(size-2) + x_1 * p^(size-3) + ...) + x_last =
        //      = p * (hash_prev) + x_last
        Hash = (Hash * primeBase + value) % primeMod;
        queue.Enqueue(value);
    }

    public int Dequeue()
    {
        // hash_prev = x_0 * p^(size-1) + x_1 * p^(size-2) + ... + x_last =
        // hash      =                    x_1 * p^(size-2) + ... + x_last =
        //           = hash_prev - x_0 * p^(size-1)
        var value = queue.Dequeue();
        Hash -= value * basePowers[queue.Count];
        if (Hash < 0) Hash = (Hash + primeMod * primeBase) % primeMod;
        return value;
    }

    public IEnumerator<int> GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => queue.Count;

    private static readonly long[] basePowers = new long[50];
    private const long primeMod = 1000000007L;
    private const long primeBase = 37L;
}