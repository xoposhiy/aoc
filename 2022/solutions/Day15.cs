using System.Security.AccessControl;

[Template(@"Sensor at @Pos: closest beacon is at @Beacon")]
public record Sensor([Separator("yx=, ")] V Pos, [Separator("yx=, ")] V Beacon)
{
    public int Distance => Pos.MDistTo(Beacon);
}

public class Day15
{
    public void Solve(Sensor[] sensors)
    {
        var beacons = sensors.Select(x => x.Beacon).ToHashSet();
        var y = 2_000_000;
        var rs = GetCoverageSegments(sensors, y).Out();
        var impossiblePositionsCount = rs.Sum(r => r.Len);
        var beaconsCount = beacons.Count(b => b.Y == y);
        (impossiblePositionsCount-beaconsCount).Out("Part 1: ");

        var pos = GetSomeBeaconPositions(sensors).Distinct().OrderBy(p => p.X).Out().First().Out("Beacon: ");
        long res = pos.X * 4000000L + pos.Y;
        res.Out("Part 2 (Fast): ");
        return;
    }

    public List<R> GetCoverageSegments(Sensor[] sensors, int y)
    {
        var ranges = sensors
            .Select(s => (x: s.Pos.X, r: s.Distance - Math.Abs(s.Pos.Y - y)))
            .Where(s => s.r >= 0)
            .Select(xr => new R(xr.x - xr.r, xr.x + xr.r));
        var mergedSegments = ranges
            .OrderBy(r => r.Start)
            .Aggregate(new List<R>(), (acc, r) =>
            {
                if (acc.Count == 0 || !acc.Last().Overlaps(r))
                    acc.Add(r);
                else
                    acc[^1] = acc[^1] with { End = Math.Max(acc[^1].End, r.End) };
                return acc;
            });
        return mergedSegments;
    }

    // Решение в одно выражение
    public IEnumerable<V> GetSomeBeaconPositions(Sensor[] sensors)
    {
        var visibleAreaCorners = new[]
        {
            new V(0, 0), new V(4_000_000, 0), new V(0, 4_000_000), new V(4_000_000, 4_000_000)
        };
        var beaconCandidatePositions = 
            from sensor1 in sensors
            from sensor2 in sensors
            from corner1 in new[] { V.Left, V.Up }
            from corner2 in new[] { V.Left, V.Down }
            let mainDiagonalLeftTop = sensor1.Pos + corner1 * (sensor1.Distance + 1)
            let minorDiagonalLeftBottom = sensor2.Pos + corner2 * (sensor2.Distance + 1)
            let doubledX =
                minorDiagonalLeftBottom.X
                + minorDiagonalLeftBottom.Y
                + mainDiagonalLeftTop.X
                - mainDiagonalLeftTop.Y
            from dx in new[] { 0, 1 }
            from dy in new[] { 0, 1 }
            let intersectionX = (doubledX + dx) / 2
            let intersectionY = intersectionX - mainDiagonalLeftTop.X + mainDiagonalLeftTop.Y
            let intersectionPoint = new V(intersectionX, intersectionY)
            where intersectionPoint.InRange(4_000_001, 4_000_001)
            select intersectionPoint;
        return beaconCandidatePositions
            .Concat(visibleAreaCorners)
            .Where(p => sensors.All(sensor => sensor.Pos.MDistTo(p) > sensor.Distance));
    }

    public IEnumerable<V> GetSomeBeaconPositions_CleanCode_Version(Sensor[] sensors)
    {
        return
            from sensor1 in sensors
            from sensor2 in sensors
            from mainDiagonal in GetOuterMainDiagonalBorders(sensor1)
            from minorDiagonal in GetOuterMinorDiagonalBorders(sensor2)
            from point in IntersectSides(mainDiagonal, minorDiagonal)
            where point.InRange(4_000_001, 4_000_001)
            where sensors.All(s => s.Pos.MDistTo(point) > s.Distance)
            select point;
    }

    private IEnumerable<V> IntersectSides(V mainDiagonal, V minorDiagonal)
    {
        var doubledX = minorDiagonal.X + minorDiagonal.Y + mainDiagonal.X - mainDiagonal.Y;
        return
            from dx in new[] { 0, 1 }
            from dy in new[] { 0, 1 }
            select new V((doubledX + dx) / 2, (doubledX + dy) / 2 - mainDiagonal.X + mainDiagonal.Y);
    }

    private IEnumerable<V> GetOuterMinorDiagonalBorders(Sensor sensor)
    {
        var r = sensor.Distance + 1;
        yield return sensor.Pos + new V(-r, 0);
        yield return sensor.Pos + new V(0, r);
    }

    private IEnumerable<V> GetOuterMainDiagonalBorders(Sensor sensor)
    {
        var r = sensor.Pos.MDistTo(sensor.Beacon) + 1;
        yield return sensor.Pos + new V(-r, 0);
        yield return sensor.Pos + new V(0, -r);
    }
    
    // Alternative (slower) solution:
    public IEnumerable<V> GetUnknownBeaconPositions_Slow(Sensor[] sensors)
    {
        var windowRange = new R(0, 4_000_000);
        for (int y = 0; y <= 4_000_000; y++)
        {
            var rs = GetCoverageSegments(sensors, y).Where(r => r.Overlaps(windowRange)).ToList();
            for (int x = 0; x < rs[0].Start; x++)
                yield return new(x, y);
            for (int i = 0; i < rs.Count - 1; i++)
            for (int x = rs[i].End+1; x < rs[i+1].Start; x++)
                yield return new(x, y);
            for (int x = rs.Last().End+1; x <= windowRange.End; x++)
                yield return new(x, y);
        }
    }
}
