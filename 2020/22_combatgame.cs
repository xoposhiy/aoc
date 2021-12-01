using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Math;

public class Day22
{
    public void Solve()
    {
        var inp = File.ReadAllLines("22.txt");
        var cards1 = inp.TakeWhile(s => s != "").Skip(1).Select(int.Parse).ToArray();
        var cards2 = inp.SkipWhile(s => s != "").Skip(2).Select(int.Parse).ToArray();

        Console.WriteLine($"Part One: {PlayCombat(cards1, cards2)}");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();
        Console.WriteLine("Part Two: " + PlayRecursiveCombat(cards1, cards2));
        Console.WriteLine(sw.Elapsed);
        Console.WriteLine();

        sw.Restart();
        Console.WriteLine("Part Two (with hashing): " + PlayRecursiveCombat_WithHashing(cards1, cards2));
        Console.WriteLine(sw.Elapsed);
    }

    private long PlayCombat(IEnumerable<int> deck1cards, IEnumerable<int> deck2cards)
    {
        var deck1 = new Queue<int>(deck1cards);
        var deck2 = new Queue<int>(deck2cards);
        while (deck1.Any() && deck2.Any())
        {
            var card1 = deck1.Dequeue();
            var card2 = deck2.Dequeue();
            var winnerDeck = card1 > card2 ? deck1 : deck2;
            winnerDeck.Enqueue(Max(card1, card2));
            winnerDeck.Enqueue(Min(card1, card2));
        }
        return GetWinnerScore(deck1, deck2);
    }


    private (int winner, long score) PlayRecursiveCombat(IEnumerable<int> deck1cards, IEnumerable<int> deck2cards)
    {
        var deck1 = new Queue<int>(deck1cards);
        var deck2 = new Queue<int>(deck2cards);
        var prevStates = new HashSet<string>();

        int GetRoundWinner(int card1, int card2)
        {
            if (card1 <= deck1.Count && card2 <= deck2.Count)
                return PlayRecursiveCombat(deck1.Take(card1), deck2.Take(card2)).winner;
            return card1 > card2 ? 1 : 2;
        }

        while (deck1.Any() && deck2.Any())
        {
            var state = string.Join(" ", deck1) + "|" + string.Join(" ", deck2);
            if (!prevStates.Add(state))
                return (1, GetWinnerScore(deck1, deck2));
            var card1 = deck1.Dequeue();
            var card2 = deck2.Dequeue();
            var winner = GetRoundWinner(card1, card2);
            var winnerDeck = winner == 1 ? deck1 : deck2;
            winnerDeck.Enqueue(winner == 1 ? card1 : card2);
            winnerDeck.Enqueue(winner == 1 ? card2 : card1);
        }

        return (winner: deck1.Any() ? 1 : 2, GetWinnerScore(deck1, deck2));
    }

    private (int winner, long score) PlayRecursiveCombat_WithHashing(IEnumerable<int> deck1cards,
        IEnumerable<int> deck2cards)
    {
        var deck1 = new HashQueue(deck1cards);
        var deck2 = new HashQueue(deck2cards);
        var cache = new HashSet<(long, long)>();

        int GetRoundWinner(int card1, int card2)
        {
            if (card1 <= deck1.Count && card2 <= deck2.Count)
                return PlayRecursiveCombat_WithHashing(deck1.Take(card1), deck2.Take(card2)).winner;
            return card1 > card2 ? 1 : 2;
        }

        while (deck1.Any() && deck2.Any())
        {
            var state = (deck1.Hash, deck2.Hash);
            if (!cache.Add(state))
                return (1, GetWinnerScore(deck1, deck2));
            var card1 = deck1.Dequeue();
            var card2 = deck2.Dequeue();
            var winner = GetRoundWinner(card1, card2);
            var winnerDeck = winner == 1 ? deck1 : deck2;
            winnerDeck.Enqueue(winner == 1 ? card1 : card2);
            winnerDeck.Enqueue(winner == 1 ? card2 : card1);
        }

        return (winner: deck1.Any() ? 1 : 2, GetWinnerScore(deck1, deck2));
    }

    private static long GetWinnerScore(IEnumerable<int> q1, IEnumerable<int> q2)
    {
        return q1.Concat(q2).Reverse().Select((n, i) => (long) n * (i + 1)).Sum();
    }
}