[Template(@"Sensor at @Sensor: closest beacon is at @Beacon")]
public record SensorData([Separator("yx=, ")]V Sensor, [Separator("yx=, ")] V Beacon)
{
    public int Distance => Sensor.MDistTo(Beacon);
};
public class Day15
{
    public void Solve(SensorData[] sensors)
    {
        var beacons = sensors.Select(x => x.Beacon).ToHashSet();
        var y = 2_000_000;
        var rs = GetCoverageSegments(sensors, y).Out();
        var impossiblePositionsCount = rs.Sum(r => r.Len);
        var beaconsCount = beacons.Count(b => b.Y == y);
        (impossiblePositionsCount-beaconsCount).Out("Part 1: ");

        var pos = GetUnknownBeaconPositions(sensors).Single().Out("Beacon: ");
        long res = pos.X * 4000000L + pos.Y;
        res.Out("Part 2: ");
    }

    public List<R> GetCoverageSegments(SensorData[] sensors, int y)
    {
        var ranges = sensors
            .Select(s => (x: s.Sensor.X, r: s.Distance - Math.Abs(s.Sensor.Y - y)))
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

    public IEnumerable<V> GetUnknownBeaconPositions(SensorData[] sensors)
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
