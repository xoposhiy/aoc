using System;
using System.Collections.Generic;
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
        var circle = PlayTurns(new Cups(cups, cups.Count), 100);
        var ans1 = circle.FindNode(1).EnumerateAll().StrJoin("").Substring(1);
        Console.WriteLine($"Part One: {ans1}");
        
        circle = PlayTurns(new Cups(cups, 1000000), 10000000);
        var one = circle.FindNode(1);
        var ans2 = (long)one.Next.Value * one.Next.Next.Value;
        Console.WriteLine($"Part Two: {ans2}");
    }

    public class Node
    {
        public Node(Node next, int value)
        {
            Next = next;
            Value = value;
        }

        public Node Next;
        public int Value;

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
        private readonly Dictionary<int, Node> index = new ();
        public int Count { get; }

        public Cups(IReadOnlyCollection<int> startCups, int count)
        {
            Count = count;
            cups = null;
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
            while (pickedUp.EnumerateAll().Contains(destination.Value))
                destination = circle.FindNode(Prev(destination.Value));
            circle.InsertRange3(destination, pickedUp);
            circle.MoveNext();
        }
        return circle;
    }
}