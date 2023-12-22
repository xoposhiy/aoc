// https://adventofcode.com/2023/day/22

public class Day22
{
    
    public void Solve([Separators(",~")]BBox3[] bricks)
    {
        bricks = bricks.OrderBy(b => b.Min.Z).ToArray();
        var supportedBy = new Dictionary<BBox3, HashSet<BBox3>>();
        var supports = new Dictionary<BBox3, List<BBox3>>();
        var zs = new int[10, 10];
        var topBricks = new BBox3?[10, 10];
        foreach (var b in bricks)        
        {
            var (x1, y1, _) = b.Min;
            var (x2, y2, _) = b.Max;
            var z = 0;
            var supporters = new HashSet<BBox3>();
            for (var x = x1; x <= x2; x++)
            for (var y = y1; y <= y2; y++)
                z = Max(z, zs[x, y]);
            
            for (var x = x1; x <= x2; x++)
            for (var y = y1; y <= y2; y++)
            {
                var topBrick = topBricks[x, y];
                if (topBrick is not null && zs[x,y] == z)
                    supporters.Add(topBrick);
            }

            foreach (var down in supporters) 
                supports[down].Add(b);
            supportedBy.Add(b, supporters);
            
            supports.Add(b, []);
            for (var x = x1; x <= x2; x++)
            for (var y = y1; y <= y2; y++)
            {
                zs[x, y] = z + b.ZSize;
                topBricks[x, y] = b;
            }
        }
        
        bricks.Where(CanBeRemoved).Count().Part1();
        bricks.Sum(CountFalls).Part2();
        return;

        bool CanBeRemoved(BBox3 b) => 
            !supports.ContainsKey(b) 
            || supports[b].All(upper => supportedBy[upper].Count != 1);

        int CountFalls(BBox3 desintegratedBrick)
        {
            var fallenBricks = new HashSet<BBox3> { desintegratedBrick };
            foreach (var brick in bricks)
            {
                if (supportedBy[brick].Any() && supportedBy[brick].All(fallenBricks.Contains)) 
                    fallenBricks.Add(brick);
            }
            return fallenBricks.Count - 1;
        }
    }
}