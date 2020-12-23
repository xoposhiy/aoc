using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Linq.Enumerable;
using static System.Math;

public class Day23
{
    public void Solve()
    {
        var cups = File.ReadAllText("23.txt").ToCharArray().Select(c => c - (int)'0').ToList();
        var circle = PlayTurns(new Cups(cups, cups.Count), 10);
        var ans1 = circle.FindNode(1).EnumerateAll().Skip(1).StrJoin("");
        Console.WriteLine($"Part One: {ans1}");

        var circle2 = PlayTurns(new Cups2(cups, cups.Count), 10);
        ans1 = circle2.EnumerateAll(1).Skip(1).StrJoin("");
        Console.WriteLine($"Part One: {ans1}");

        var sw = Stopwatch.StartNew();
        circle = PlayTurns(new Cups(cups, 1000000), 10000000);
        var one = circle.FindNode(1);
        var ans2 = (long)one.Next.Value * one.Next.Next.Value;
        Console.WriteLine($"Part Two: {ans2} in {sw.ElapsedMilliseconds} ms");


        sw = Stopwatch.StartNew();
        circle2 = PlayTurns(new Cups2(cups, 1000000), 10000000);
        ans2 = (long)circle2.Next[1] * circle2.Next[circle2.Next[1]];
        Console.WriteLine($"Part Two: {ans2} in {sw.ElapsedMilliseconds} ms");
    }

    public class Node
    {
        public Node(Node next, int value)
        {
            Next = next;
            Value = value;
        }

        public Node Next;
        public readonly int Value;

        public IEnumerable<int> EnumerateAll()
        {
            var node = this;
            while (true)
            {
                yield return node.Value;
                node = node.Next;
                if (node == this) break;
            }
        }
    }

    public class Cups
    {
        private Node cups;
        private readonly Node[] index;
        public int Count { get; }

        public Cups(IReadOnlyCollection<int> startCups, int count)
        {
            Count = count;
            cups = null;
            index = new Node[count + 1];
            Node firstNode = null;
            var values = startCups.Concat(Range(startCups.Count + 1, count - startCups.Count)).Reverse();
            foreach (var value in values)
            {
                cups = new Node(cups, value);
                firstNode ??= cups;
                index[value] = cups;
            }
            firstNode!.Next = cups;
        }

        public int CurrentValue => cups.Value;

        public Node ExtractNext3()
        {
            var res = cups.Next;
            var lastNode = cups.Next.Next.Next;
            cups.Next = lastNode.Next;
            lastNode.Next = res;
            return res;
        }

        public void InsertRange3(Node destination, Node range3)
        {
            var tail = destination.Next;
            destination.Next = range3;
            range3.Next.Next.Next = tail;
        }

        public Node FindNode(int value) => index[value];
        public void MoveNext() => cups = cups.Next;

    }

    private static Cups PlayTurns(Cups circle, int turnsCount)
    {
        int Prev(int n) => n == 1 ? circle.Count : n - 1;
        for (int i = 0; i < turnsCount; i++)
        {
            var pickedUp = circle.ExtractNext3();
            var destination = circle.FindNode(Prev(circle.CurrentValue));
            while (pickedUp.Value == destination.Value
                   || pickedUp.Next.Value == destination.Value
                   || pickedUp.Next.Next.Value == destination.Value)
                destination = circle.FindNode(Prev(destination.Value));
            circle.InsertRange3(destination, pickedUp);
            circle.MoveNext();
        }
        return circle;
    }


    // Alternative solution: linked lists on arrays

    public class Cups2
    {
        public readonly int[] Next;
        public int Count { get; }

        public Cups2(IReadOnlyCollection<int> startCups, int count)
        {
            Count = count;
            Next = new int[count + 1];
            CurrentValue = -1;
            var values = startCups.Concat(Range(startCups.Count + 1, count - startCups.Count)).Reverse();
            var firstValue = -1;
            foreach (var value in values)
            {
                if (CurrentValue < 0)
                    firstValue = value;
                else
                    Next[value] = CurrentValue;
                CurrentValue = value;
            }
            Next[firstValue] = CurrentValue;
        }

        public int CurrentValue { get; private set; }

        public int ExtractNext3()
        {
            var item1 = Next[CurrentValue];
            var item2 = Next[item1];
            var item3 = Next[item2];
            Next[CurrentValue] = Next[item3];
            return item1;
        }

        public void InsertRange3(int destination, int range3)
        {
            var tail = Next[destination];
            Next[destination] = range3;
            Next[Next[Next[range3]]] = tail;
        }

        public void MoveNext() => CurrentValue = Next[CurrentValue];

        public IEnumerable<int> EnumerateAll(int startValue)
        {
            yield return startValue;
            var node = Next[startValue];
            while (node != startValue)
            {
                yield return node;
                node = Next[node];
            }
        }
    }

    private static Cups2 PlayTurns(Cups2 circle, int turnsCount)
    {
        int Prev(int n) => n == 1 ? circle.Count : n - 1;
        for (int i = 0; i < turnsCount; i++)
        {
            var pickedUp = circle.ExtractNext3();
            var destination = Prev(circle.CurrentValue);
            while (pickedUp == destination 
                   || circle.Next[pickedUp] == destination 
                   || circle.Next[circle.Next[pickedUp]] == destination)
                destination = Prev(destination);
            circle.InsertRange3(destination, pickedUp);
            circle.MoveNext();
        }

        return circle;
    }
}