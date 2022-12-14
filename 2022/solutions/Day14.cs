public class Day14
{
    public void Solve([Separator(" ->,")]V[][] paths)
    {
        var floor = paths.SelectMany(p => p).Max(p => p.Y).Out() + 2;
        var map = new char[500][];
        for (int y = 0; y < map.Length; y++)
        {
            map[y] = new char[1000];
            for (int x = 0; x < map[y].Length; x++)
                map[y][x] = '.';
        }

        foreach (var item in paths)
        {
            var first = item[0];
            foreach (var second in item.Skip(1))
            {
                
                var step = second - first;
                var d = Math.Max(Math.Abs(step.X), Math.Abs(step.Y));
                for (int i = 0; i <= d; i++)
                {
                    var next = first + step.Signum() * i;
                    map.Set(next, '#');
                }
                first = second;
            }
        }

        bool DropSandUnit(char[][] map, bool hasFloor)
        {
            var p = new V(500, 0);
            while (true)
            {
                p += V.Down;
                if (p.Y >= floor)
                {
                    if (hasFloor)
                    {
                        p += V.Up;
                        map.Set(p, 'O');
                        return false;
                    }
                    else return true;
                }
                if (map[p.Y][p.X] != '.') p += V.Left;
                if (!p.X.InRange(0, map[0].Length)) return true;
                if (map[p.Y][p.X] != '.') p += V.Right*2;
                if (!p.X.InRange(0, map[0].Length)) return true;
                if (map[p.Y][p.X] != '.')
                {
                    p += V.Up+V.Left;
                    map.Set(p, 'O');
                    return false;
                }
            }
            
        }
        int count = 0;
        var original = map.Select(a => a.ToArray()).ToArray();
        count = 0;
        while (map[0][500] == '.')
        {
            var outside = DropSandUnit(map, false);
            if (outside) break;
            count++;
        }
        count.Out("Part 1: ");

        map = original;
        count = 0;
        while (map[0][500] == '.')
        {
            var outside = DropSandUnit(map, true);
            if (outside) break;
            count++;
        }
        //for (int i = 0; i < floor; i++)
        //{
        //    for (int x = 490; x < 560; x++)
        //        Console.Write(map[i][x]);
        //    Console.WriteLine();
        //}
        count.Out("Part 2: ");

    }

}