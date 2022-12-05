public class Day02
{
    // input:
    // A X
    // ...
    public void Solve((char opp, char me)[] moves)
    {
        int Score(int opp, int me) =>
            me + ((me - opp + 4) % 3) * 3 + 1;

        int Score2(int opp, int outcome) =>
            Score(opp, (opp + outcome - 1 + 3) % 3);

        var rounds = moves.Select(p => (p.opp-'A', p.me-'X')).ToList();
        
        /*
         * Part 1
         * Даны раунды в камень ножницы бумага − что показал я и мой оппонент.
         * Найти сколько я заработал очков.
         * Очки зависят от исхода раунда и от того, что я показал.
         */
        Console.WriteLine("Part1: " + rounds.Sum(r => Score(r.Item1, r.Item2)));
        
        /*
         * Part 2
         * Даны раунды в камень ножницы бумага − что показал мой оппонент и какой был исход раунда.
         * Найти сколько я заработал очков.
         */
        Console.WriteLine("Part2: " + rounds.Sum(r => Score2(r.Item1, r.Item2)));
    }
}