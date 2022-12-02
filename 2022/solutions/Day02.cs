public class Day02
{
    public void Solve(string[] lines)
    {
        var rounds = lines.ParseLines<char[]>().Select(p => (p[0]-'A', p[1]-'X')).ToList();
        Console.WriteLine("Part1: " + rounds.Sum(r => Score(r.Item1, r.Item2)));
        Console.WriteLine("Part2: " + rounds.Sum(r => Score2(r.Item1, r.Item2)));
    }

    private int Score(int opp, int me) => 
        me + ((me - opp + 4) % 3)*3 + 1;

    private int Score2(int opp, int outcome) => 
        Score(opp, (opp + outcome - 1 + 3) % 3);
}