using System.Text;
using static System.String;

[Template("Valve @Id has flow rate=@FlowRate; tunnels? leads? to valves? @Neighbors")]
public record InputValve(string Id, int FlowRate, string[] Neighbors);
public record Valve(int FlowRate, int[] NeighborIndices);

public class Day16
{

    public Dictionary<long, int> GetPressureDropForEachValvesMask(Valve[] valves, int startIndex, int maxTime)
    {
        var front = new Dictionary<(long openMask, int valveIndex), int>
        {
            { (0, startIndex), 0 }
        };
        for (int time = 1; time <= maxTime; time++)
        {
            var nextFront = new Dictionary<(long openMask, int valveIndex), int>();
            foreach (var group in front.GroupBy(kv => kv.Key.openMask))
            {
                var openMask = group.Key;
                var thisTickDrop = valves.Select((valve, index) => openMask.HasBit(index) ? valve.FlowRate : 0).Sum();
                foreach (var ((_, valveIndex), totalDrop) in group)
                {
                    var valve = valves[valveIndex];
                    var nextTotalDrop = totalDrop + thisTickDrop;
                    if (!openMask.HasBit(valveIndex) && valve.FlowRate > 0)
                    {
                        var nextOpenMask = openMask.SetBit(valveIndex);
                        nextFront.UpdateMax((nextOpenMask, valveIndex), nextTotalDrop);
                    }
                    foreach (var nextValveIndex in valve.NeighborIndices)
                        nextFront.UpdateMax((openMask, nextValveIndex), nextTotalDrop);
                }
            }
            front = nextFront;
            //(front.Count, front.MaxBy(kv => kv.Value)).Out($"t={time} (Size, BestDrop) = ");
        }
        return front.GroupBy(kv => kv.Key.openMask)
            .ToDictionary(g => g.Key, g => g.Max(kv => kv.Value));
    }

    public void Solve(InputValve[] vs)
    {
        CreateGraphVizFile(vs);
        var valveIndices = vs.Select((v, i) => (v, i)).ToDictionary(t => t.v.Id, t => t.i);
        var valves = vs.Select(v => new Valve(v.FlowRate, v.Neighbors.Select(n => valveIndices[n]).ToArray())).ToArray();
        valves.Length.Out();
        var startValveIndex = valveIndices["AA"].Out("StartValve: ");
        GetPressureDropForEachValvesMask(valves, startValveIndex, 30)
            .Values.Max().Out("Part1: ");
        
        var dp = GetPressureDropForEachValvesMask(valves, startValveIndex, 26);
        dp.Count.Out("Count: ");
        var index = valves.Indices().First(i => valves[i].FlowRate > 0);
        var groups = dp.GroupBy(kv => kv.Key & (1L << index)).ToList();
        var drops = 
            from kv1 in groups[0]
            from kv2 in groups[1]
            where (kv1.Key & kv2.Key) == 0
            select kv1.Value + kv2.Value;
        drops.Max().Out("Part 2: ");
    }

    private static void CreateGraphVizFile(InputValve[] vs)
    {
        var gv = new StringBuilder("graph Day16 {\n");
        foreach (var v in vs)
            gv.AppendLine($"  {v.Id} [size={v.FlowRate}, shape={(v.FlowRate == 0 ? "point" : "circle")}];");
        foreach (var v in vs)
        foreach (var n in v.Neighbors)
            if (Compare(v.Id, n, StringComparison.Ordinal) < 0)
                gv.AppendLine($"  {v.Id} -- {n};");
        gv.AppendLine("}");
        File.WriteAllText("Day16.gv", gv.ToString());
    }
}