using Shouldly;

[Template(@"Blueprint @Index:\s+Each ore robot costs @OreRobotOre ore.\s+" +
          @"Each clay robot costs @ClayRobotOre ore.\s+" +
          @"Each obsidian robot costs @ObsidianRobotOre ore and @ObsidianRobotClay clay.\s+" +
          @"Each geode robot costs @GeodeRobotOre ore and @GeodeRobotObsidian obsidian.")]
public record Blueprint(int Index, 
    int OreRobotOre, 
    int ClayRobotOre, 
    int ObsidianRobotOre, int ObsidianRobotClay, 
    int GeodeRobotOre, int GeodeRobotObsidian)
{
    public int MaxOre { get; } = Math.Max(Math.Max(OreRobotOre, ClayRobotOre), Math.Max(ObsidianRobotOre, GeodeRobotOre));
}

public class Day19
{
    // Даны схемы (blueprint) производства роботов 4 типов: Ore, Clay, Obsidian, Geode.
    // В схеме для каждого типа робота указано количество ресурсов, которое необходимо потратить, чтобы его построить.
    // Каждую минуту можно построить одного какого-то робота, если хватает ресурсов.
    // Каждую минуту каждый робот производит одну единицу своего ресурса.
    public void Solve(Blueprint[] blueprints)
    {
        // Для каждой схемы найти максимальное возможное количество добытых жеодов за 24 минуты,
        // умножить на номер схемы и вернуть сумму таких слагаемых для всех схем.
        blueprints.Sum(b => b.Index * GetMaxGeodesCount(b, 24))
            .Out("Part 1: ").ShouldBe(2160);

        // Для трёх первых схем посчитать максимально возможное количество добытых жеодов за 32 минуты
        // и вернуть их произведение.
        blueprints.Take(3).Product(b => GetMaxGeodesCount(b, 32))
            .Out("Part 2: ").ShouldBe(13340);
    }

    private int GetMaxGeodesCount(Blueprint blueprint, int minutesCount)
    {
        var pathItems = GraphSearch.Dijkstra(
                s => GetNextStates(blueprint, s),
                s => -s.MinutesLeft,
                new SearchState(0, 0, 0, 0, OreRobots: 1, 0, 0, 0, minutesCount))
            .ToList();
        var geode = pathItems.Max(p => p.State.Geode);
        
        (blueprint.Index, pathItems.Count, geode).Out("blueprintIndex, QueueSize, GeodesCount: ");
        return geode;
    }
    
    private IEnumerable<SearchState> GetNextStates(Blueprint bp, SearchState state)
    {
        var minutesLeft = state.MinutesLeft;
        if (minutesLeft <= 0) yield break;
        var next1 = state.BuildGeodeRobot(bp);
        if (next1 != null) yield return next1.Value.TrimResources(bp);
        var next2 = state.BuildObsidianRobot(bp);
        if (next2 != null) yield return next2.Value.TrimResources(bp);
        var next3 = state.BuildClayRobot(bp);
        if (next3 != null) yield return next3.Value.TrimResources(bp);
        var next4 = state.BuildOreRobot(bp);
        if (next4 != null) yield return next4.Value.TrimResources(bp);
        if ((next1 ?? next2 ?? next3 ?? next4) == null)
            yield return (state with
            {
                Ore = state.Ore + minutesLeft * state.OreRobots,
                Clay = state.Clay + minutesLeft * state.ClayRobots,
                Obsidian = state.Obsidian + minutesLeft * state.ObsidianRobots,
                Geode = state.Geode + minutesLeft * state.GeodeRobots,
                MinutesLeft = 0
            }).TrimResources(bp);
    }
}

public readonly record struct SearchState(
    int Ore, int Clay, int Obsidian, int Geode,
    int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots, int MinutesLeft)
{
    public override string ToString()
    {
        return $"Ore={Ore} Clay={Clay} Ob={Obsidian} G={Geode} \t OreR={OreRobots} ClayR={ClayRobots} ObR={ObsidianRobots} GR={GeodeRobots} \t Time={MinutesLeft}";
    }

    public int GeodeEstimate => Geode + MinutesLeft * GeodeRobots;
    public SearchState? BuildOreRobot(Blueprint bp)
    {
        if (OreRobots == 0 || OreRobots >= bp.MaxOre) return null;
        var wait = TimeToGet(bp.OreRobotOre - Ore, OreRobots) + 1;
        if (wait > MinutesLeft) return null;
        return this with
        {
            Ore = Ore + wait * OreRobots - bp.OreRobotOre,
            Clay = Clay + wait * ClayRobots,
            Obsidian = Obsidian + wait * ObsidianRobots,
            Geode = Geode + wait * GeodeRobots,
            OreRobots = OreRobots + 1,
            MinutesLeft = MinutesLeft - wait
        };
    }
    
    public SearchState? BuildClayRobot(Blueprint bp)
    {
        if (OreRobots == 0 || ClayRobots >= bp.ObsidianRobotClay) return null;
        var wait = TimeToGet(bp.ClayRobotOre - Ore, OreRobots) + 1;
        if (wait > MinutesLeft) return null;
        return this with
        {
            Ore = Ore + wait * OreRobots - bp.ClayRobotOre,
            Clay = Clay + wait * ClayRobots,
            Obsidian = Obsidian + wait * ObsidianRobots,
            Geode = Geode + wait * GeodeRobots,
            ClayRobots = ClayRobots + 1,
            MinutesLeft = MinutesLeft - wait
        };

    }
    
    public SearchState? BuildObsidianRobot(Blueprint bp)
    {
        if (OreRobots == 0 || ClayRobots == 0 || ObsidianRobots >= bp.GeodeRobotObsidian) return null;
        var waitOre = TimeToGet(bp.ObsidianRobotOre - Ore, OreRobots);
        var waitClay = TimeToGet(bp.ObsidianRobotClay - Clay, ClayRobots);
        var wait = Math.Max(waitOre, waitClay) + 1;
        if (wait > MinutesLeft) return null;
        return this with
        {
            Ore = Ore + wait * OreRobots - bp.ObsidianRobotOre,
            Clay = Clay + wait * ClayRobots - bp.ObsidianRobotClay,
            Obsidian = Obsidian + wait * ObsidianRobots,
            Geode = Geode + wait * GeodeRobots,
            ObsidianRobots = ObsidianRobots + 1,
            MinutesLeft = MinutesLeft - wait
        };

    }

    public SearchState? BuildGeodeRobot(Blueprint bp)
    {
        if (OreRobots == 0 || ObsidianRobots == 0) return null;
        var waitOre = TimeToGet(bp.GeodeRobotOre - Ore, OreRobots);
        var waitObsidian = TimeToGet(bp.GeodeRobotObsidian - Obsidian, ObsidianRobots);
        var wait = Math.Max(waitOre, waitObsidian) + 1;
        if (wait > MinutesLeft) return null;
        return this with
        {
            Ore = Ore + wait * OreRobots - bp.GeodeRobotOre,
            Clay = Clay + wait * ClayRobots,
            Obsidian = Obsidian + wait * ObsidianRobots - bp.GeodeRobotObsidian,
            Geode = Geode + wait * GeodeRobots,
            GeodeRobots = GeodeRobots + 1,
            MinutesLeft = MinutesLeft - wait
        };
    }

    private int TimeToGet(int amount, int amountPerTick)
    {
        if (amount <= 0) return 0;
        return amount / amountPerTick + (amount % amountPerTick != 0 ? 1 : 0);
    }

    // Optimization: не надо ресурсов больше, чем мы сможем потратить до конца времени.
    // если вот так уменьшить избыточные ресурсы, то все состояния с превышением ресурсов станут равны и Bfs
    // их отсечет сам как посещенные и не будет перебирать их детей.
    public SearchState TrimResources(Blueprint bp) =>
        this with
        {
            Ore = Math.Min(Ore, MinutesLeft * bp.MaxOre - OreRobots * (MinutesLeft - 1)),
            Clay = Math.Min(Clay, MinutesLeft * bp.ObsidianRobotClay - ClayRobots * (MinutesLeft - 1)),
            Obsidian = Math.Min(Obsidian, MinutesLeft * bp.GeodeRobotObsidian - ObsidianRobots * (MinutesLeft - 1))
        };
}