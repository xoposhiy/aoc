using Shouldly;

public record RobotCost(int Ore, int Clay, int Obsidian);
public record Blueprint(int Index, RobotCost OreRobot, RobotCost ClayRobot, RobotCost ObsidianRobot, RobotCost GeodeRobot);
public record SearchState(
    int Ore, int Clay, int Obsidian, int Geode, 
    int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots);

public class Day19
{
    public void Solve(string[] lines)
    {
        var blueprints = lines.Select(ParseBlueprint).ToList();
        blueprints.Sum(b => b.Index * GetMaxGeodesCount(b, 24))
            .Out("Part 1: ").ShouldBe(2160);
        blueprints.Take(3).Product(b => GetMaxGeodesCount(b, 32))
            .Out("Part 2: ").ShouldBe(13340);
    }

    private int GetMaxGeodesCount(Blueprint blueprint, int minutesCount)
    {
        //Console.WriteLine($"### Blueprint {blueprint.Index}:");
        var pathItems = GraphSearch.Bfs(
            s => GetNextStates(blueprint, s), minutesCount,
            new SearchState(0, 0, 0, 0, OreRobots: 1, 0, 0, 0));
        //var pi = pathItems.MaxBy(p => p.State.Geode);
        //while (pi != null)
        //{
        //    Console.WriteLine(pi.State);
        //    pi = pi.Prev;
        //}
        var geode = pathItems
            .Max(p => p.State.Geode);
        (blueprint.Index, pathItems.Count, geode).Out("blueprintIndex, QueueSize, GeodesCount: ");
        return geode;
    }

    private IEnumerable<SearchState> GetNextStates(Blueprint bp, PathItem<SearchState> pathItem)
    {
        var (ore, clay, obsidian, geode, oreRobots, clayRobots, obsidianRobots, geodeRobots) = pathItem.State;
        var newOre = ore + oreRobots;
        var newClay = clay + clayRobots;
        var newObsidian = obsidian + obsidianRobots;
        var newGeode = geode + geodeRobots;
        var build = false;
        var buildG = false;
        if (ore >= bp.GeodeRobot.Ore && obsidian >= bp.GeodeRobot.Obsidian)
        {
            yield return new SearchState(newOre - bp.GeodeRobot.Ore, newClay, newObsidian - bp.GeodeRobot.Obsidian, newGeode, oreRobots, clayRobots, obsidianRobots, geodeRobots + 1);
            build = buildG = true;
        }
        if (!buildG && ore >= bp.OreRobot.Ore && ore < 6)
        {
            yield return new SearchState(newOre - bp.OreRobot.Ore, newClay, newObsidian, newGeode, oreRobots + 1, clayRobots, obsidianRobots, geodeRobots);
            build = true;
        }

        if (!buildG && ore >= bp.ClayRobot.Ore && clay <= 20)
        {
            yield return new SearchState(newOre - bp.ClayRobot.Ore, newClay, newObsidian, newGeode, oreRobots, clayRobots + 1, obsidianRobots, geodeRobots);
            build = true;

        }

        if (!buildG && ore >= bp.ObsidianRobot.Ore && clay >= bp.ObsidianRobot.Clay && obsidian <= 20)
        {
            yield return new SearchState(newOre - bp.ObsidianRobot.Ore, newClay-bp.ObsidianRobot.Clay, newObsidian, 
                newGeode, oreRobots, clayRobots, obsidianRobots + 1, geodeRobots);
            build = true;

        }

        if (!build)
            yield return new SearchState(newOre, newClay, newObsidian, newGeode, oreRobots, clayRobots, obsidianRobots, geodeRobots);
        else
            if (ore < Math.Max(Math.Max(bp.OreRobot.Ore , bp.ClayRobot.Ore)+2, Math.Max(bp.ObsidianRobot.Ore , bp.GeodeRobot.Ore)+2)
                && clay < bp.ObsidianRobot.Clay+20
                && obsidian < bp.GeodeRobot.Obsidian+2)
            yield return new SearchState(newOre, newClay, newObsidian, newGeode, oreRobots, clayRobots, obsidianRobots, geodeRobots);
    }

    private Blueprint ParseBlueprint(string arg)
    {
        var ps = arg.Split(' ', ':').Where(p => int.TryParse(p, out var _)).Select(int.Parse).ToList();
        var i = 0;
        var index = ps[i++];
        var ore = new RobotCost(ps[i++], 0, 0);
        var clay = new RobotCost(ps[i++], 0, 0);
        var obsidian = new RobotCost(ps[i++], ps[i++], 0);
        var geode = new RobotCost(ps[i++], 0, ps[i++]);
        return new Blueprint(index, ore, clay, obsidian, geode);
    }
}