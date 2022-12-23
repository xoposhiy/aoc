using Shouldly;

// https://adventofcode.com/2022/day/23
public class Day23
{
    // Дана карта, на которой эльфы обозначены символом #.
    // Эльфы движутся по замороченным правилам:
    // 1. Если на 8 соседних клетках нет других эльфах - стоят на месте.
    // 2. Эльф ищет первое из 4 направлений, где нет другого эльфа и также нет эльфов на двух клетках по диагонали в этом направлении,
    //      то они хотят в продвинутся на одну клетку в этом направлении.
    // 3. Если в клетку хочет только один эльф − он туда идет. Иначе, остается на месте.
    // 4. После каждого раунда перемещений, упорядоченный список направлений для п.2 циклически сдвигается на 1 вверх.
    
    public void Solve(string[] lines)
    {
        var points = lines.SelectMany((line, y) => line.Select((_, x) => new V(x, y)).Where(v => lines[v.Y][v.X] == '#'))
            .ToHashSet();
        
        // Part 1: Найти, количество клеток без эльфов, в boundingBox эльфов после 10 раундов.
        var end = Simulate(points).Take(10).Last();
        var area = R2.FromPoints(end).Area - end.Count;
        area.Part1().ShouldBe(3996);
        
        // Part 2: Найти номер раунда, на котором эльфы перестают двигаться.
        var rounds = Simulate(points).Count();
        rounds.Part2().ShouldBe(908);
    }

    private IEnumerable<HashSet<V>> Simulate(HashSet<V> points)
    {
        var dirs = new[]
        {
            new[] { V.N, V.NE, V.NW },
            new[] { V.S, V.SE, V.SW },
            new[] { V.W, V.NW, V.SW },
            new[] { V.E, V.NE, V.SE },
        };
        while (true)
        {
            var targets = points
                .Where(p => p.Area8().Any(points.Contains))
                .Select(p => (p, ds: dirs.FirstOrDefault(ds => ds.All(d => !points.Contains(d + p)))))
                .Where(t => t.ds != null)
                .Select(t => (t.p, dest: t.p + t.ds![0]))
                .GroupBy(t => t.dest).ToList()
                .Where(g => g.Count() == 1)
                .ToDictionary(g => g.Single().p, g => g.Key);
            var endRound = new HashSet<V>();
            foreach (var point in points)
            {
                if (point.Area8().Any(points.Contains))
                    endRound.Add(targets.TryGetValue(point, out var dest) ? dest : point);
                else
                    endRound.Add(point);
            }
            yield return endRound;
            if (points.All(endRound.Contains)) yield break;
            points = endRound;
            dirs = dirs[1..].Append(dirs[0]).ToArray();
        }
    }
}