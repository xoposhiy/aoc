public class Day08
{
    public void Solve(char[][] map)
    {
        var w = map[0].Length;
        var h = map.Length;

        V EndOfSight(V from, V dir)
        {
            var p = from + dir;
            while (p.InRange(w, h) && map[p.Y][p.X] < map[from.Y][from.X])
                p += dir;
            return p;
        }

        int SightDist(V from, V dir)
        {
            var p = EndOfSight(from, dir);
            return p.InRange(w, h) ? p.MDistTo(from) : p.MDistTo(from) - 1;
        }

        var visibleCount = map.Indices().Count(
            tree => V.Directions4.Any(d => !EndOfSight(tree, d).InRange(w, h)));
        Console.WriteLine($"Part1: {visibleCount}");


        var maxScore = map.Indices().Max(
            tree => V.Directions4.Product(dir => SightDist(tree, dir)));
        Console.WriteLine($"Part2: {maxScore}");
    }
}