using Shouldly;

public class Day08
{
    public void Solve(char[][] map)
    {
        // Дана квадратная карта высот.

        // Part 1
        // Найти все клетки карты, которые не загораживаются ни одной другой клеткой при взгляде снаружи карты вдоль осей координат.
        V EndOfSight(V from, V dir)
        {
            var p = from + dir;
            while (p.InRange(map) && map[p.Y][p.X] < map[from.Y][from.X])
                p += dir;
            return p;
        }

        map.Indices()
            .Count(tree => V.Directions4.Any(d => !EndOfSight(tree, d).InRange(map)))
            .Out("Part 1: ").ShouldBe(1698);

        // Part 2
        // Найти клетку с максимальным sightScore. SightScore равен произведению дальности видимости по четырем направлениям.
        // Дальность видимости - количество идущих подряд клеток в заданном направлении, чья высота меньше высоты исходной клетки.
        int SightDist(V from, V dir)
        {
            var p = EndOfSight(from, dir);
            return p.InRange(map) ? p.MDistTo(from) : p.MDistTo(from) - 1;
        }

        map.Indices()
            .Max(tree => V.Directions4.Product(dir => SightDist(tree, dir)))
            .Out("Part 2: ").ShouldBe(672280);
    }
}