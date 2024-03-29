﻿public record MapPathItem(V Pos, MapPathItem? Prev, int Len);

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
                    var nextPathItem = new PathItem<TState>(state, pathItem, pathItem.Len + 1);
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
            if (path.Len < maxDistance)
                queue.AddRange(getNextStates(path)
                    .Where(visited.Add)
                    .Select(nextState => new PathItem<TState>(nextState, path, path.Len + 1)));
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
                .Select(nextState => new PathItem<TState>(nextState, path, path.Len + 1));
            foreach (var pathItem in pathItems)
                queue.Enqueue(pathItem);
        }
    }

    public static IEnumerable<MapPathItem> BfsLazy<T>(this T[][] map, V[] neighbors, Func<T, T, bool> canPassFromTo, params V[] starts)
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
                queue.Enqueue(new(next, item, item.Len + 1));
            }
        }
    }

    public static IEnumerable<V> GetAll<T>(this T[][] map, Func<T, bool> predicate)
    {
        for (int y = 0; y < map.Length; y++)
        for (int x = 0; x < map[y].Length; x++)
        {
            if (predicate(map[y][x]))
                yield return new(x, y);
        }
    }

    public static V GetPos<T>(this T[][] map, T value)
    {
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[y].Length; x++)
        {
            if (map[y][x]!.Equals(value))
                return new(x, y);
        }
        throw new KeyNotFoundException();
    }


    public static PathItem<TState> Visualize<TState>(
        this PathItem<TState> path,
        Func<TState, V> getPosition, Func<TState, string>? getDescription = null)
    {
        void ShowDescription(int i, TState state)
        {
            Console.SetCursorPosition(1, 0);
            var line = "PathLen: " + i + " " + getDescription?.Invoke(state);
            Console.Write(line.PadRight(Console.WindowWidth - 1));
        }

        var stepByStep = true;
        Console.Clear();
        var first = true;
        var len = 0;
        foreach (var step in path.StepsForward())
        {
            len++;
            var from = getPosition(step.from);
            var to = getPosition(step.to);
            if (first)
            {
                ShowDescription(0, step.from);
                Console.SetCursorPosition(from.X, from.Y+1);
                Console.Write('*');
                first = false;
            }
            Console.SetCursorPosition(to.X, to.Y+1);
            Console.Write((to-from).ToArrow());
            if (stepByStep)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Escape) stepByStep = false;
                var state = step.to;
                ShowDescription(len, state);
            }
        }
        ShowDescription(path.Len, path.State);
        Console.ReadLine();
        return path;
    }

    public static IEnumerable<PathItem<TState>> Visualize<TState>(
        this IEnumerable<PathItem<TState>> paths, int width, int height,
        Func<TState, V> getPosition)
    {
        Console.Clear();
        var visualize = true;
        var toClear = new HashSet<V>();
        foreach (var pathItem in paths)
        {
            yield return pathItem;
            if (!visualize) continue;
            var nextToClear = new HashSet<V>();
            var curItem = pathItem;
            while (curItem != null)
            {
                var pos = getPosition(curItem.State);
                Console.SetCursorPosition(pos.X, pos.Y);
                nextToClear.Add(pos);
                toClear.Remove(pos);
                if (curItem.Prev == null)
                    Console.Write('*');
                else
                {
                    var prevPos = getPosition(curItem.Prev.State);
                    Console.Write((pos - prevPos).ToArrow());
                }
                curItem = curItem.Prev;
            }

            foreach (var v in toClear)
            {
                Console.SetCursorPosition(v.X, v.Y);
                Console.Write(' ');
            }
            toClear = nextToClear;
            var end = getPosition(pathItem.State);
            Console.SetCursorPosition(end.X, end.Y);

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape) visualize = false;
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

public record PathItem<TState>(TState State, PathItem<TState>? Prev, int Len)
{
    public IEnumerable<(TState from, TState to)> StepsForward()
    {
        return StepsBackward().Reverse();
    }

    public IEnumerable<(TState from, TState to)> StepsBackward()
    {
        var cur = this;
        while (cur.Prev != null)
        {
            yield return (cur.Prev.State, cur.State);
            cur = cur.Prev;
        }
    }

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