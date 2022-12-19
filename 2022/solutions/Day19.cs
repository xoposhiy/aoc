using Shouldly;

[Template("Blueprint @Index: Each ore robot costs @OreRobotOre ore. " +
          "Each clay robot costs @ClayRobotOre ore. " +
          "Each obsidian robot costs @ObsidianRobotOre ore and @ObsidianRobotClay clay. " +
          "Each geode robot costs @GeodeRobotOre ore and @GeodeRobotObsidian obsidian.")]

public record Blueprint(int Index, 
    int OreRobotOre, int ClayRobotOre, 
    int ObsidianRobotOre, int ObsidianRobotClay, 
    int GeodeRobotOre, int GeodeRobotObsidian)
{
    public int MaxOre { get; } = Math.Max(Math.Max(OreRobotOre, ClayRobotOre), Math.Max(ObsidianRobotOre, GeodeRobotOre));
}

public record SearchState(
    int Ore, int Clay, int Obsidian, int Geode, 
    int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots);

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
        var geode = pathItems
            .Max(p => p.State.Geode);
        (blueprint.Index, pathItems.Count, geode).Out("blueprintIndex, QueueSize, GeodesCount: ");
        return geode;
    }

    private IEnumerable<SearchState> GetNextStates(Blueprint bp, PathItem<SearchState> pathItem, int maxMinutes)
    {
        var minutesLeft = maxMinutes - pathItem.Distance;

        SearchState TrimState(
            int ore, int clay, int obsidian, int geode, 
            int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots)
        {
            return new SearchState(
                Math.Min(ore, minutesLeft * bp.MaxOre - oreRobots * (minutesLeft - 1)),
                Math.Min(clay, minutesLeft * bp.ObsidianRobotClay - clayRobots * (minutesLeft - 1)),
                Math.Min(obsidian, minutesLeft * bp.GeodeRobotObsidian - obsidianRobots * (minutesLeft - 1)),
                geode,
                Math.Min(oreRobots, bp.MaxOre),
                Math.Min(clayRobots, bp.ObsidianRobotClay),
                Math.Min(obsidianRobots, bp.GeodeRobotObsidian),
                geodeRobots
            );
        }

        {
            var state = pathItem.State;
            var (ore, clay, obsidian, geode, oreRobots, clayRobots, obsidianRobots, geodeRobots) = state;
            var newOre = ore + oreRobots;
            var newClay = clay + clayRobots;
            var newObsidian = obsidian + obsidianRobots;
            var newGeode = geode + geodeRobots;
            if (ore >= bp.GeodeRobotOre && obsidian >= bp.GeodeRobotObsidian)
            {
                yield return TrimState(
                    newOre - bp.GeodeRobotOre, newClay, newObsidian - bp.GeodeRobotObsidian, newGeode,
                    oreRobots, clayRobots, obsidianRobots, geodeRobots + 1);
                yield break;
            }

            if (ore >= bp.OreRobotOre)
                yield return TrimState(
                    newOre - bp.OreRobotOre, newClay, newObsidian, newGeode,
                    oreRobots + 1, clayRobots, obsidianRobots, geodeRobots);
            if (ore >= bp.ClayRobotOre)
                yield return TrimState(
                    newOre - bp.ClayRobotOre, newClay, newObsidian, newGeode,
                    oreRobots, clayRobots + 1, obsidianRobots, geodeRobots);
            if (ore >= bp.ObsidianRobotOre && clay >= bp.ObsidianRobotClay)
                yield return TrimState(
                    newOre - bp.ObsidianRobotOre, newClay - bp.ObsidianRobotClay, newObsidian, newGeode,
                    oreRobots, clayRobots, obsidianRobots + 1, geodeRobots);
            yield return TrimState(
                newOre, newClay, newObsidian, newGeode,
                oreRobots, clayRobots, obsidianRobots, geodeRobots);
        }
    }
}