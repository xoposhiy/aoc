// https://adventofcode.com/2023/day/13

public class Day13
{
    public void Solve(params char[][][] maps)
    {
        maps.Sum(m => Score(m)).Part1();
        maps.Sum(m => Score(m, 1)).Part2();
        return;

        long Score(char[][] map, int expectedDiffs = 0)
        {
            var y = GetReflectionLineY(map, expectedDiffs);
            if (y != 0) return 100 * y;
            return GetReflectionLineY(map.Columns(), expectedDiffs);
        }

        long GetReflectionLineY(char[][] map, int diff) => 
            Range(1, map.Height() - 1).FirstOrDefault(y => GetDiffsInReflection(y, map) == diff);

        int GetDiffsInReflection(int reflectionLineY, char[][] map) =>
            (
                from y in Range(0, reflectionLineY)
                let symY = 2 * reflectionLineY - 1 - y
                where symY < map.Height()
                select map[y].Zip(map[symY]).Count(x => x.First != x.Second)
            ).Sum();
    }
}