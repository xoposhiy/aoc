using Shouldly;

public class Day02
{
    // Даны раунды в камень ножницы бумага − что показал мой оппонент и ещё что-то второе.
    // input:
    // A X
    // ...
    public void Solve((char opponent, char something)[] moves)
    {
        int Score(int opp, int me) =>
            me + (me - opp + 1).ModPositive(3) * 3 + 1;

        int Score2(int opp, int outcome) =>
            Score(opp, (opp + outcome - 1).ModPositive(3));

        var rounds = moves.Select(p => (p.opponent-'A', p.something-'X')).ToList();
        
        /*
         * Part 1
         * Даны раунды в камень ножницы бумага − что показал я и мой оппонент.
         * Найти сколько я заработал очков.
         * Очки зависят от исхода раунда и от того, что я показал.
         */
        rounds.Sum(r => Score(r.Item1, r.Item2))
            .Out("Part 1: ").ShouldBe(10816);
        
        /*
         * Part 2
         * Даны раунды в камень ножницы бумага − что показал мой оппонент и какой был исход раунда.
         * Найти сколько я заработал очков.
         */
        rounds.Sum(r => Score2(r.Item1, r.Item2))
            .Out("Part 2: ").ShouldBe(11657);
    }
}