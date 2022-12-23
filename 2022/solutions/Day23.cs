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
        var points = lines.ToPoints('#').ToHashSet();
        
        // Part 1: Найти, количество клеток без эльфов, в boundingBox эльфов после 10 раундов.
        var end = Simulate(points).Take(10).Last();
        var area = R2.FromPoints(end).Area - end.Count;
        area.Part1().ShouldBe(3996);
        
        // Part 2: Найти номер раунда, на котором эльфы перестают двигаться.
        var rounds = Simulate(points).Count();
        rounds.Part2().ShouldBe(908);
    }

    private V GetProposedTarget(V elf, HashSet<V> points, (V, int)[] dirs)
    {
        var mask = 0;
        foreach (var neighbor in elf.Area8())
        {
            mask <<= 1;
            if (points.Contains(neighbor)) 
                mask++;
        }
        if (mask == 0) return elf;
        foreach (var (dir, dirMask) in dirs)
        {
            if ((mask & dirMask) == 0)
                return elf + dir;
        }
        return elf;
    }

    private IEnumerable<HashSet<V>> Simulate(HashSet<V> elfs)
    {
        var orderedDirections = new[]{
            (V.N, 0b00000111),
            (V.S, 0b01110000),
            (V.W, 0b00011100),
            (V.E, 0b11000001),
        };
        while (true)
        {
            var elfsGroupedByDestination = elfs
                .Select(elf => (elf, dest: GetProposedTarget(elf, elfs, orderedDirections)))
                .GroupBy(t => t.dest, t => t.elf).ToList();
            var endRound = new HashSet<V>();
            foreach (var elfsWithSameDestination in elfsGroupedByDestination)
            {
                if (elfsWithSameDestination.Count() == 1)
                    endRound.Add(elfsWithSameDestination.Key);
                else
                {
                    foreach (var elf in elfsWithSameDestination)
                        endRound.Add(elf);
                }
            }
            //endRound.CreateMap().Out();
            yield return endRound;
            if (elfs.All(endRound.Contains)) yield break;
            elfs = endRound;
            orderedDirections = orderedDirections[1..].Append(orderedDirections[0]).ToArray();
        }
    }
}