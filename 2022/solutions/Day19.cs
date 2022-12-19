using Shouldly;

[Template("Blueprint @Index: Each ore robot costs @OreRobotOre ore. " +
          "Each clay robot costs @ClayRobotOre ore. " +
          "Each obsidian robot costs @ObsidianRobotOre ore and @ObsidianRobotClay clay. " +
          "Each geode robot costs @GeodeRobotOre ore and @GeodeRobotObsidian obsidian.")]

public record Blueprint(int Index, 
    int OreRobotOre, 
    int ClayRobotOre, 
    int ObsidianRobotOre, int ObsidianRobotClay, 
    int GeodeRobotOre, int GeodeRobotObsidian)
{
    public int MaxOre { get; } = Math.Max(Math.Max(OreRobotOre, ClayRobotOre), Math.Max(ObsidianRobotOre, GeodeRobotOre));
}

public readonly record struct SearchState(
    int Ore, int Clay, int Obsidian, int Geode,
    int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots)
{
    public SearchState BuildOreRobot(Blueprint bp) => this with
    {
        Ore = Ore - bp.OreRobotOre, 
        OreRobots = OreRobots+1
    };
    
    public SearchState BuildClayRobot(Blueprint bp) => this with
    {
        Ore = Ore - bp.ClayRobotOre, 
        ClayRobots = ClayRobots+1
    };
    
    public SearchState BuildObsidianRobot(Blueprint bp) => this with
    {
        Ore = Ore - bp.ObsidianRobotOre, 
        Clay = Clay - bp.ObsidianRobotClay, 
        ObsidianRobots = ObsidianRobots+1
    };
    
    public SearchState BuildGeodeRobot(Blueprint bp) => this with
    {
        Ore = Ore - bp.GeodeRobotOre, 
        Obsidian = Obsidian - bp.GeodeRobotObsidian, 
        GeodeRobots = GeodeRobots+1
    };

    public SearchState RobotsDoJob()
    {
        return this with
        {
            Ore = Ore + OreRobots,
            Clay = Clay + ClayRobots,
            Obsidian = Obsidian + ObsidianRobots,
            Geode = Geode + GeodeRobots
        };
    }

    public SearchState TrimResources(Blueprint bp, int minutesLeft) =>
        this with
        {
            Ore = Math.Min(Ore, minutesLeft * bp.MaxOre - OreRobots * (minutesLeft - 1)),
            Clay = Math.Min(Clay, minutesLeft * bp.ObsidianRobotClay - ClayRobots * (minutesLeft - 1)),
            Obsidian = Math.Min(Obsidian, minutesLeft * bp.GeodeRobotObsidian - ObsidianRobots * (minutesLeft - 1))
        };
}

public class Day19
{
    public void Solve(Blueprint[] blueprints)
    {
        blueprints.Sum(b => b.Index * GetMaxGeodesCount(b, 24))
            .Out("Part 1: ").ShouldBe(2160);
        blueprints.Take(3).Product(b => GetMaxGeodesCount(b, 32))
            .Out("Part 2: ").ShouldBe(13340);
    }

    private int GetMaxGeodesCount(Blueprint blueprint, int minutesCount)
    {
        var pathItems = GraphSearch.Bfs(
            s => GetNextStates(blueprint, s, minutesCount), minutesCount,
            new SearchState(0, 0, 0, 0, OreRobots: 1, 0, 0, 0));
        var geode = pathItems.Max(p => p.State.Geode);
        (blueprint.Index, pathItems.Count, geode).Out("blueprintIndex, QueueSize, GeodesCount: ");
        return geode;
    }

    private IEnumerable<SearchState> GetNextStates(Blueprint bp, PathItem<SearchState> pathItem, int maxMinutes)
    {
        var minutesLeft = maxMinutes - pathItem.Distance;
        var (ore, clay, obsidian, geode, oreRobots, clayRobots, obsidianRobots, geodeRobots) = pathItem.State;
        var newState = pathItem.State.RobotsDoJob();
        if (ore >= bp.GeodeRobotOre && obsidian >= bp.GeodeRobotObsidian)
        {
            yield return newState.BuildGeodeRobot(bp).TrimResources(bp, minutesLeft);
            yield break;
        }
        if (ore >= bp.OreRobotOre && oreRobots < bp.MaxOre)
            yield return newState.BuildOreRobot(bp).TrimResources(bp, minutesLeft);
        if (ore >= bp.ClayRobotOre && clayRobots < bp.ObsidianRobotClay)
            yield return newState.BuildClayRobot(bp).TrimResources(bp, minutesLeft);
        if (ore >= bp.ObsidianRobotOre && clay >= bp.ObsidianRobotClay && obsidianRobots < bp.GeodeRobotObsidian)
            yield return newState.BuildObsidianRobot(bp).TrimResources(bp, minutesLeft);
        if (ore < bp.MaxOre || clay < bp.ObsidianRobotClay || obsidian < bp.GeodeRobotObsidian)
            yield return newState;
    }
}