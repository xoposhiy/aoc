public class Day14
{
    public void Solve([Separator(" ->,")]V[][] paths)
    {
        var walls = paths.SelectMany(path => 
            path.Zip(path.Skip(1), (a, b) => a.SequenceTo(b)).SelectMany(p=>p))
            .ToHashSet();
        var floor = walls.Max(p => p.Y).Out("MaxY: ") + 2;
        var source = new V(500, 0);

        bool DropSandUnit(bool hasFloor)
        {
            var sandUnit = source;
            while (true)
            {
                if (sandUnit.Y >= floor-1)
                {
                    if (!hasFloor) return true;
                    walls.Add(sandUnit);
                    return false;
                }

                var possibleNext = new[] { sandUnit+V.Down, sandUnit+V.Down + V.Left, sandUnit+V.Down + V.Right };
                var next = possibleNext.FirstOrDefault(n => !walls.Contains(n));
                if (next is null)
                {
                    walls.Add(sandUnit);
                    return false;
                }
                sandUnit = next;
            }
            
        }
        var originalMap = walls.ToHashSet();
        int count = 0;
        while (true)
        {
            var outside = DropSandUnit(false);
            if (outside) break;
            count++;
 
        }
        //walls.CreateMap().Out();
        count.Out("Part 1: ");

        walls = originalMap;
        count = 0;
        while (!walls.Contains(source))
        {
            DropSandUnit(true);
            count++;
        }
        count.Out("Part 2: ");

    }

}