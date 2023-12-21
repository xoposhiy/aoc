// https://adventofcode.com/2023/day/20

public class Day20
{
    public abstract record Chip(string Name, string[] Outputs)
    {
        public readonly List<string> Inputs = [];

        public abstract int? Receive(string input, int level);

        public static Chip Parse(string desc, string[] outputs)
        {
            var name = desc.Trim('%', '&');
            var type = desc[0];
            return type switch
            {
                '%' => new FlipFlop(name, outputs),
                '&' => new Nand(name, outputs),
                _ => new PlainChip(name, outputs),
            };
        }
    }

    public record PlainChip(string Name, string[] Outputs) : Chip(Name, Outputs)
    {
        public override int? Receive(string input, int level) => level;
    }

    public record FlipFlop(string Name, string[] Outputs) : Chip(Name, Outputs)
    {
        public int State { get; set; }

        public override int? Receive(string input, int level)
        {
            if (level == 1) return null;
            State = 1-State;
            return State;
        }
    }

    public record Nand(string Name, string[] Outputs) : Chip(Name, Outputs)
    {
        private readonly Dictionary<string, int> inputLevels = new();
        public override int? Receive(string input, int level)
        {
            inputLevels[input] = level;
            if (inputLevels.Count < Inputs.Count) return 1;
            return inputLevels.Values.All(inputHigh => inputHigh == 1) ? 0 : 1;
        }
    }

    public void Solve([Separators(" ->,")](string Desc, string[] Outputs)[] cps)
    {
        var chips = cps.Select(c => Chip.Parse(c.Desc, c.Outputs))
            .ToDictionary(c => c.Name);
        chips.Add("rx", new PlainChip("rx", []));
        foreach (var src in chips.Values)
        {
            foreach (var dst in src.Outputs) 
                chips[dst].Inputs.Add(src.Name);
        }
        SaveGraphViz(chips);

        var pulsesCount = new[] { 0, 0 };
        var presses = 0;
        var rxSourcePresses = new Dictionary<string, int>();
        var rxSource = chips[chips["rx"].Inputs.Single()];
        void Pulse(string initialInput, int initialLevel)
        {
            presses++;
            var q = new Queue<(string input, int level)>();
            q.Enqueue((initialInput, initialLevel));
            pulsesCount[initialLevel]++;
            while (q.Any())
            {
                var (src, lvl) = q.Dequeue();
                foreach (var dest in chips[src].Outputs)
                {
                    //Console.WriteLine($"{src} -{lvl}-> {dest}");
                    pulsesCount[lvl]++;
                    if (rxSource.Inputs.Contains(dest) && lvl == 0) 
                        rxSourcePresses.TryAdd(dest, presses);
                    var outputPulse = chips[dest].Receive(src, lvl);
                    if (!outputPulse.HasValue) continue;
                    q.Enqueue((dest, outputPulse.Value));
                }
            }
        }

        for (int i = 0; i < 1000; i++) 
            Pulse("broadcaster", 0);
        pulsesCount.Product().Part1();
        

        while(rxSourcePresses.Count < 4) 
            Pulse("broadcaster", 0);
        rxSourcePresses.Values.LeastCommonMultiple().Part2();
    }

    private static void SaveGraphViz(Dictionary<string, Chip> cps)
    {
        var nandNodes = cps.Values.OfType<Nand>()
            .Select(c => $"{c.Name} [shape=invhouse, label=\"{c.Name}\"];")
            .StrJoin("\n");
        var edges = cps.Values
            .SelectMany(chip => chip.Outputs.Select(dest => $"{chip.Name} -> {dest};")).StrJoin("\n");
        var graph = "digraph Day20 {\n" + nandNodes + "\n" + edges + "\n}";
        File.WriteAllText("day20.gv", graph);
    }
}