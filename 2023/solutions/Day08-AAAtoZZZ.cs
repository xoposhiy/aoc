// https://adventofcode.com/2023/day/8

using System.Diagnostics;
using System.Numerics;
using System.Text;
using TextCopy;

public class Day08
{
    public record Node(string From, string Left, string Right);
    public void Solve(string actions, [Separators(fieldBound:" (),=")]Node[] lines)
    {
        var graph = lines.ToDictionary(line => line.From);
        //MakeGraphvizANdCopyToCLipboard(graph); return; 
        GetStepsCount("AAA")
            .Part1();

        var ps = graph.Keys
            .Where(k => k.EndsWith("A"))
            .Select(GetStepsCount)
            .ToList();
            
        ps.LeastCommonMultiple()
            .Part2();
        return;
        
        long GetStepsCount(string start)
        {
            var cycle = GraphSearch.GetCycle(
                (Name: start, Index: 0),
                n => 
                    (
                        actions[n.Index % actions.Length] == 'L' ? graph[n.Name].Left : graph[n.Name].Right,
                    (n.Index + 1)%actions.Length
                        ));
            // Constraints necessary for this solution:
            Debug.Assert(cycle.Single(n => n.node.Name.EndsWith("Z")).index == cycle.Count);
            Debug.Assert(cycle.Count(n => n.node.Name.EndsWith("Z")) == 1);
            return cycle.Count;
        }
    }

    private static void MakeGraphvizANdCopyToCLipboard(Dictionary<string, Node> g)
    {
        var sb = new StringBuilder("digraph G {\n");
        foreach (var node in g.Keys)
        {
            var shape = "oval";
            if (node.EndsWith("A"))
                shape = "box";
            if (node.EndsWith("Z"))
                shape="doubleoctagon";
            sb.AppendLine($"\"{node}\" [shape={shape}];");
        }
        foreach (var node in g.Keys)
        {
            sb.AppendLine("\"" + node + "\" -> \"" + g[node].Left + "\";");
            sb.AppendLine("\"" + node + "\" -> \"" + g[node].Right + "\";");
        }

        sb.AppendLine("}");
        ClipboardService.SetText(sb.ToString());
    }
}