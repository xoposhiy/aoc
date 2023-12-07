// https://adventofcode.com/2023/day/7

public class Day07
{
    public void Solve((string cards, int bid)[] hands)
    {
        TotalWinnings(false).Part1();
        TotalWinnings(true).Part2();
        return;

        int TotalWinnings(bool withJoker)
        {
            Array.Sort(hands, (a, b) => CompareHands(a.cards, b.cards, withJoker));
            return hands.Select((hand, index) => (index+1) * hand.bid).Sum();
        }
    }
    

    private static int CompareHands(string a, string b, bool withJoker)
    {
        var cardsOrder = withJoker ? "J23456789TQKA" : "23456789TJQKA";
        var byType = HandType(a, withJoker).CompareTo(HandType(b, withJoker));
        if (byType != 0) return byType;
        return a.Zip(b).Select(pair => 
            cardsOrder.IndexOf(pair.First).CompareTo(cardsOrder.IndexOf(pair.Second))).FirstOrDefault(byCard => byCard != 0);
    }

    private static int HandType(string cards, bool withJoker)
    {
        if (withJoker)
            return "23456789TQKA".Max(joker => HandType(cards.Replace('J', joker), false));
        return cards.CountFrequency().Values.OrderByDescending(v => v).ToArray() switch
        {
            [5, ..] => 6,
            [4, ..] => 5,
            [3, 2, ..] => 4,
            [3, 1, ..] => 3,
            [2, 2, ..] => 2,
            [2, 1, ..] => 1,
            _ => 0
        };
    }
}