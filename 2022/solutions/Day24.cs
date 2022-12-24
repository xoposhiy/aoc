

// https://adventofcode.com/2022/day/24
public class Day24
{
    // Дана карта прямоугольной комнаты на которой есть вихри (> < v ^).
    // Вихри движутся в своем направлении одну клетку в минуту. 
    // Когда достигают стены комнаты (#), перемещаются к противоположной стене, сохраняя направление.
    // Вихри проходят друг через друга. А нас убивают.
    // Мы тоже ходим со скоростью одна клетка в секунду.
    // Вход в комнату в левом верхнем углу, выходы − в правом нижнем.
    public void Solve(string[] lines)
    {
        // Part 1. Найти длину кратчайшего пути из верхнего левого входа в нижний правый выход.
        var exit = new V(lines[0].Length-2, lines.Length-1);
        var entrance = new V(1, 0);
        var map = lines.Select(row => row.ToCharArray()).ToArray();
        var goThere = SearchPath(map, (entrance, 0), exit).State;
        goThere.time.Part1();

        // Part 2. Найди длину кратчайшего пути вход → выход → вход → выход
        var goBack = SearchPath(map, goThere, entrance).State;
        var goThereWithSnacks = SearchPath(map, goBack, exit).State;
        goThereWithSnacks.time.Part2();
    }

    private PathItem<(V pos, int time)> SearchPath(char[][] map, (V pos, int time) start, V endPos) =>
        GraphSearch.BfsLazy(p => GetMoves(map, p.State), start)
            .First(p => p.State.pos == endPos)
            .Visualize(s => s.pos, s=>$"Time: {s.time} Pos: {s.pos}");

    private IEnumerable<(V pos, int time)> GetMoves(char[][] map, (V pos, int time) cur) =>
        cur.pos.Area5()
            .Where(n => n.InRange(map) && map.Get(n) != '#' && !IsBlizzardAt(map, n, cur.time + 1))
            .Select(n => (n, cur.time + 1));

    private bool IsBlizzardAt(char[][] map, V pos, int time)
    {
        var h = map.Length-2;
        var w = map[0].Length-2;
        if (!pos.Y.InRange(1, h+1) || !pos.X.InRange(1, w+1)) return false;
        return map[pos.Y][(pos.X - time - 1).ModPositive(w) + 1] == '>' ||
               map[pos.Y][(pos.X + time - 1).ModPositive(w) + 1] == '<' ||
               map[(pos.Y - time - 1).ModPositive(h) + 1][pos.X] == 'v' ||
               map[(pos.Y + time - 1).ModPositive(h) + 1][pos.X] == '^';
    }
}