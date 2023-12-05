// https://adventofcode.com/2023/day/4

public class Day04
{
    // ## Input:
    // Card   1: 30 48 49 69  1 86 94 68 12 85 | 86 57 89  8 81 85 82 68  1 22 90  2 74 12 30 45 69 92 62  4 94 48 47 64 49
    // ...
    //
    // ## Day 1
    // Find total score after each card played. Count number of matches: values in the first part which are present in the second part. Rise 2 to that power if it is greater than 0.
    // That is the score for that card.
    //
    // ## Day 2
    // Now playing each card, you win copies of the scratchcards below the winning card equal to the number of matches.
    // So, if card 10 were to have 5 matching numbers, you would win one copy each of cards 11, 12, 13, 14, and 15. 
    // Find the number of cards after the game finished.

    public record Card(string _, int Id, [Separators(fieldBound:" ", arrayBound:"|")]int[] My, int[] Win)
    {
        public int Matches => My.Count(Win.Contains);
    }

    public void Solve(Card[] cards)
    {
        cards.Sum(card => card.Matches == 0 ? 0 : 1 << (card.Matches - 1))
            .Part1();

        var counts = cards.Select(c => 1).ToArray();
        for (var i = 0; i < cards.Length; i++)
        for (var k = i + 1; k <= i + cards[i].Matches; k++)
            counts[k] += counts[i];
        counts.Sum().Part2();
    }
}