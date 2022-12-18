using Shouldly;

public class Day18
{
    // ���� ���������� �������� (3D-��������).
    public void Solve(V3[] points)
    {
        // Part 1:
        // ����� ������� ����������� ������ �� ��������.
        var pointsSet = points.ToHashSet();
        points.Sum(p => p.Area6().Count(n => !pointsSet.Contains(n)))
            .Out("Part 1: ").ShouldBe(4310);

        // Part 2:
        // ����� ������� ������� ����������� ������ �� ��������.
        // �� ���� ����������� ���������� �������� �� ���������.
        var boundingBox = BBox3.FromPoints(points).Grow(1);
        var maxSearchDistance = boundingBox.XSize + boundingBox.YSize + boundingBox.ZSize + 3;
        var outerSurface = GraphSearch.Bfs(
                p => p.State.Area6().Where(n => !pointsSet.Contains(n) && boundingBox.Contains(n)),
                maxSearchDistance,
                starts: boundingBox.Min)
            .Select(p => p.State).ToHashSet();
        points.Sum(p => p.Area6().Count(outerSurface.Contains))
            .Out("Part 2: ").ShouldBe(2466);
    }
}


