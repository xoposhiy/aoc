using Shouldly;
using static System.Linq.Enumerable;

// https://adventofcode.com/2022/day/22
public class Day22
{
    record Me(V Pos, V Dir);

    // Дана карта лабиринта. И путь в нём. Мы стартуем из самой левой свободной точки в верхней строке, и смотрим направо.
    // Надо найти позицию, в которой мы окажемся, если пройдем путь.
    // И посчитать пароль по координатам и направлению в финальной позиции.
    public void Solve(string[] rows, string path)
    {
        var map = rows.Select(row => row.ToCharArray()).ToArray();
        
        // Part 1:
        // Каждая строка и столбец замкнуты циклически.
        GetPassword(SimulateMoves(map, path).Last())
            .Part1().ShouldBe(97356);

        // Part 1:
        // Это не карта, а развёртка кубика. Надо собрать кубик и найти, где окажемся, если будем идти по кубику.
        GetPassword(SimulateMoves(map, path, onCube:true).Last())
            .Part2().ShouldBe(120175);
    }

    private static int GetPassword(Me me)
    {
        return (me.Pos.Y+1) * 1000 + (me.Pos.X+1) * 4 + new[] { V.Right, V.Down, V.Left, V.Up }.IndexOf(me.Dir);
    }

    private IEnumerable<Me> SimulateMoves(char[][] map, string path, bool onCube = false)
    {
        var rowDesc = map
            .Select(r => (size: r.SkipWhile(c => c == ' ').Count(), start: r.TakeWhile(c => c == ' ').Count()))
            .ToArray();
        var width = map.Max(r => r.Length);
        var height = map.Length;
        var colDesc = Range(0, width)
            .Select(x => (
                    size: Range(0, height).Count(y => x < map[y].Length && map[y][x] != ' '),
                    start: Range(0, height).First(y => x < map[y].Length && map[y][x] != ' ')
                    ))
            .ToArray();
        
        V WrapX(V p)
        {
            var (size, start) = rowDesc![p.Y];
            return new V((p.X - start + size) % size + start, p.Y);
        }
        V WrapY(V p)
        {
            var (size, start) = colDesc![p.X];
            return new V(p.X, (p.Y - start + size) % size + start);
        }
        
        Me Wrap(Me me) => 
            me with { Pos = me.Dir.Y == 0 ? WrapX(me.Pos + me.Dir) : WrapY(me.Pos + me.Dir) };


        Me WrapCube(Me me)
        {
            var (p, dir) = me;
            if (dir.CLen > 1)
                throw new Exception(me.ToString());
            const int size = 50;
            var tileFrom = p / size;
            var tileTo = (p + dir + new V(size, size)) / size - new V(1, 1);
            if (tileFrom == tileTo) return new Me(p + dir, dir);
            var inTile = p % size;
            var res = (tileFrom, tileTo) switch
            {
                ((1, 0), (0, 0)) => new Me(new V(0, 3*size-1 - inTile.Y), V.Right),
                ((1, 0), (1, -1)) =>new Me(new V(0, 3*size + inTile.X), V.Right),
                ((2, 0), (2, -1)) =>new Me(new V(inTile.X, 4*size-1), V.Up),
                ((2, 0), (3, 0)) => new Me(new V(2*size-1, 3*size-1 - inTile.Y), V.Left),
                ((2, 0), (2, 1)) => new Me(new V(2*size - 1, size + inTile.X), V.Left),
                ((1, 1), (2, 1)) => new Me(new V(2*size + inTile.Y, size-1), V.Up),
                ((1, 2), (2, 2)) => new Me(new V(3*size - 1, size-1 - inTile.Y), V.Left),
                ((1, 2), (1, 3)) => new Me(new V(size-1, 3*size + inTile.X), V.Left),
                ((0, 3), (1, 3)) => new Me(new V(size + inTile.Y, 3*size-1), V.Up),
                ((0, 3), (0, 4)) => new Me(new V(2*size + inTile.X, 0), V.Down),
                ((0, 3), (-1, 3)) =>new Me(new V(size + inTile.Y, 0), V.Down),
                ((0, 2), (-1, 2)) =>new Me(new V(size, size-1 - inTile.Y), V.Right),
                ((0, 2), (0, 1)) => new Me(new V(size, size + inTile.X), V.Right),
                ((1, 1), (0, 1)) => new Me(new V(inTile.Y, 2*size), V.Down),
                _ => new Me(p + dir, dir)
            };
            if (map.Get(res.Pos) == ' ')
                throw new Exception($"{map.Get(res.Pos)}");
            return res;
        }

        Me Move(Me me, int len)
        {
            Func<Me, Me> wrap = onCube ? WrapCube : Wrap;
            if (me.Dir.CLen > 1)
                throw new Exception(me.ToString());
            var nextMe = wrap(me);
            while (map.Get(nextMe.Pos) != '#')
            {
                me = nextMe;
                len--;
                if (len == 0) break;
                nextMe = wrap(me);
                var backNextMe = nextMe with { Dir = nextMe.Dir.RotateCW().RotateCW() };
                var nextNextMe = wrap(backNextMe);
                var backNextNextMe = nextNextMe with { Dir = nextNextMe.Dir.RotateCW().RotateCW() };
                if (backNextNextMe != me)
                    throw new Exception($"{backNextNextMe} != {me}");
                if (map.Get(nextMe.Pos) == ' ')
                    throw new Exception($"{nextMe.Pos} is empty");
            }
            return me;
        }
        

        var x = map[0].IndexOf('.');
        var me = new Me(new V(x, 0), V.Right);
        var i = 0;
        var steps = 0;
        while (i < path.Length)
        {
            var c = path[i++];
            if (char.IsDigit(c))
                steps = steps * 10 + c - '0';
            else
            {
                if (steps > 0)
                    yield return me = Move(me, steps);
                steps = 0;
                yield return me = me with
                {
                    Dir = c == 'L' 
                        ? me.Dir.RotateCCW() 
                        : me.Dir.RotateCW()
                };
            }
        }
        if (steps > 0)
            yield return Move(me, steps);
    }
}