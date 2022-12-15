using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Template(@"Sensor at @Sensor: closest beacon is at @Beacon")]
public record SensorData([Separator("yx=, ")]V Sensor, [Separator("yx=, ")] V Beacon)
{
    public int Distance => Sensor.MDistTo(Beacon);
};
public class Day15
{
    IEnumerable<R> GetCoverageSegments(SensorData[] sensors, int y)
    {
        return sensors
            .Select(s => (x: s.Sensor.X, r: s.Distance - Math.Abs(s.Sensor.Y - y)))
            .Select(xr => new R(xr.x - xr.r, xr.x + xr.r));

    }

    public void Solve(SensorData[] sensors)
    {
        var left = Math.Min(sensors.Min(x => x.Beacon.X), sensors.Min(x => x.Sensor.X));
        var right = Math.Max(sensors.Max(x => x.Beacon.X), sensors.Max(x => x.Sensor.X));
        var beacons = sensors.Select(x => x.Beacon).ToHashSet();
        var margin = sensors.Max(x => x.Sensor.MDistTo(x.Beacon)) + 1;
        margin.Out("Margin: ");
        var impossiblePositionsCount = 0;

        var y = 2000000;
        var rs = GetCoverageSegments(sensors, 2000000).ToList();
        for (int x = left-margin; x <= right+margin;)
        {
            var overlappedRs = rs.Where(r => r.Contains(x)).ToList();
            if (overlappedRs.Any())
            {
                var lastX = overlappedRs.Max(t => t.End);
                impossiblePositionsCount += lastX + 1 - x;
                x = lastX + 1;
            }
            else
            {
                var nextRs = rs.Where(r => r.Start > x).ToList();
                if (nextRs.Any())
                {
                    x = nextRs.Min(t => t.Start);
                }
                else
                    break;
            }
        }

        var beaconsCount = beacons.Count(b => b.Y == y);
        
        (impossiblePositionsCount-beaconsCount).Out("Part 1: ");


        var pos = GetUnknownBeaconPosition(sensors).Out("Beacon: ");
        long res = pos.X * 4000000L + pos.Y;
        res.Out("Part 2: ");
    }

    private V GetUnknownBeaconPosition(SensorData[] sensors)
    {
        for (int y = 0; y <= 4000000; y++)
        {
            var rs = GetCoverageSegments(sensors, y).ToList();
            for (int x = 0; x <= 4000000;)
            {
                var overlappedRs = rs.Where(r => r.Contains(x)).ToList();
                if (overlappedRs.Any())
                {
                    var lastX = overlappedRs.Max(t => t.End);
                    x = lastX + 1;
                }
                else
                {
                    return new V(x, y);
                }
            }
        }

        throw new Exception("No beacon");
    }
}
