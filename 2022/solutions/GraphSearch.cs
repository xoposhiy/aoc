public record MapPathItem(V Pos, MapPathItem? Prev, int Distance);

public record PathItem<TState>(TState State, PathItem<TState>? Prev, int Distance)
{
    public PathItem<TState> Out()
    {
        var cur = this;
        while (cur != null)
        {
            cur.State.Out();
            cur = cur.Prev;
        }
        return this;
    }
}

public static class GraphSearch
{
    public static IEnumerable<PathItem<TState>> Dijkstra<TState>(
        Func<TState, IEnumerable<TState>> getNextStates,
        Func<TState, IComparable> getPriority,
        TState start)
    {
        var q = new PriorityQueue<PathItem<TState>, IComparable>();
        var visited = new HashSet<TState>();
        var startPathItem = new PathItem<TState>(start, null, 0);
        q.Enqueue(startPathItem, getPriority(start));
        visited.Add(start);
        yield return startPathItem;
        while (q.Count > 0)
        {
            var pathItem = q.Dequeue();
            foreach (var state in getNextStates(pathItem.State))
            {
                if (visited.Add(state)) 
                {
                    var nextPathItem = new PathItem<TState>(state, pathItem, pathItem.Distance + 1);
                    q.Enqueue(nextPathItem, getPriority(state));
                    yield return nextPathItem;
                }
            }
        }
    }

    public static List<PathItem<TState>> Bfs<TState>(
        Func<PathItem<TState>, IEnumerable<TState>> getNextStates, 
        int maxDistance,
        params TState[] starts)
    {
        var visited = starts.ToHashSet();
        var queue = starts.Select(start => new PathItem<TState>(start, null, 0)).ToList();
        var outQ = 0;
        while (outQ < queue.Count)
        {
            var path = queue[outQ++];
            if (path.Distance < maxDistance)
                queue.AddRange(getNextStates(path)
                    .Where(visited.Add)
                    .Select(nextState => new PathItem<TState>(nextState, path, path.Distance + 1)));
        }
        return queue;
    }
    
    public static IEnumerable<PathItem<TState>> BfsLazy<TState>(
        Func<PathItem<TState>, IEnumerable<TState>> getNextStates, 
        params TState[] starts)
    {
        var visited = starts.ToHashSet();
        var queue = starts.Select(start => new PathItem<TState>(start, null, 0)).ToQueue();
        while (queue.Any())
        {
            var path = queue.Dequeue();
            yield return path;
            var pathItems = getNextStates(path)
                .Where(visited.Add)
                .Select(nextState => new PathItem<TState>(nextState, path, path.Distance + 1));
            foreach (var pathItem in pathItems)
                queue.Enqueue(pathItem);
        }
    }

    public static IEnumerable<MapPathItem> Bfs<T>(this T[][] map, V[] neighbors, Func<T, T, bool> canPassFromTo, params V[] starts)
    {
        var visited = starts.ToHashSet();
        var queue = new Queue<MapPathItem>();
        foreach (var start in starts)
            queue.Enqueue(new(start, null, 0));
        while (queue.Count > 0)
        {
            var item = queue.Dequeue();
            yield return item;
            var from = map.Get(item.Pos);
            foreach (var next in neighbors.Select(d => item.Pos + d))
            {
                if (!next.InRange(map) || !canPassFromTo(from, map.Get(next)) || visited.Contains(next))
                    continue;
                visited.Add(next);
                queue.Enqueue(new(next, item, item.Distance + 1));
            }
        }
    }
    
    public static MapPathItem OutPath(this MapPathItem mapPathEnd, char[][]? map = null)
    {
        var pathItems = new List<MapPathItem>();
        for (var p = mapPathEnd; p != null; p = p.Prev)
            pathItems.Add(p);

        string DirChar(V next, V? prev) =>
            (next - (prev ?? next)) switch
            {
                (0, -1) => "↑",
                (0, 1) => "↓",
                (-1, 0) => "<",
                (1, 0) => "→",
                _ => "*"
            };

        pathItems
            .CreateMap(pi => pi.Pos, v => map == null ? "." : (map.Get(v) + ""), pi => DirChar(pi.Pos, pi.Prev?.Pos))
            .Out();
        return mapPathEnd;
    }

}