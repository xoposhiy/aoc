using Shouldly;

public class Day18
{
    public void Solve(char[][] map)
    {
        var keyPositions = map.GetAll(char.IsLower).ToDictionary(p => map[p.Y][p.X], p => p);
        var fullKeys = keyPositions.Aggregate(0, (keys, key) => keys.SetBit(key.Key));

        IEnumerable<(V KeyPos, char Key, int Distance)> GetMoves(V pos, int myKeys)
        {
            IEnumerable<V> GetNextPos(V cur)
            {
                foreach (var next in cur.Area4())
                {
                    if (!next.InRange(map)) continue;
                    var c = map.Get(next);
                    if (c.IsOneOf('.', '@')
                        || char.IsLower(c)
                        || char.IsUpper(c) && myKeys.HasBit(c))
                        yield return next;
                }
            }

            var paths = GraphSearch.Bfs(
                prev => GetNextPos(prev.State),
                    int.MaxValue, pos)
                .Where(p => char.IsLower(map.Get(p.State)))
                .ToDictionary(p => p.State);
            return keyPositions
                    .Where(keyPos => !myKeys.HasBit(keyPos.Key) && paths.ContainsKey(keyPos.Value))
                    .Select(keyPos => (keyPos.Value, keyPos.Key, paths[keyPos.Value].Len));
        }

        int Dijkstra(V start)
        {
            var q = new PriorityQueue<(V Pos, int Keys, int Distance), int>();
            q.Enqueue((start, 0, 0), 0);
            var distances = new Dictionary<(V Pos, int Keys), int>();
            var bestKeysCount = 0;
            while (q.Count > 0)
            {
                var (pos, keys, distance) = q.Dequeue();
                var keysCount = int.PopCount(keys);
                if (keysCount > bestKeysCount)
                {
                    (keysCount, distances.Count).Out("keysCount: ");
                    bestKeysCount = keysCount;
                }

                if (keys == fullKeys)
                    return distance;
                foreach (var (nextPos, newKey, stepDist) in GetMoves(pos, keys))
                {
                    var newKeys = keys.SetBit(newKey);
                    var newDist = distance + stepDist;
                    if (distances.TryGetValue((nextPos, newKeys), out var dist) && dist <= newDist)
                        continue;
                    distances[(nextPos, newKeys)] = newDist;
                    q.Enqueue((nextPos, newKeys, newDist), newDist);
                }
            }
            return -1;
        }

        var start = map.GetPos('@');
        Dijkstra(start).Part1().ShouldBe(4868);

        IEnumerable<int> DijkstraPart2(V a, V b, V c, V d)
        {
            var q = new PriorityQueue<(V PosA, V PosB, V PosC, V PosD, int Keys, int Distance), int>();
            q.Enqueue((a, b, c, d, 0, 0), 0);
            var distances = new Dictionary<(V PosA, V PosB, V PosC, V PosD, int Keys), int>();
            var bestKeysCount = 0;
            var minDistance = int.MaxValue;
            while (q.Count > 0)
            {
                var (posA, posB, posC, posD, keys, distance) = q.Dequeue();
                var keysCount = int.PopCount(keys);
                if (keysCount > bestKeysCount)
                {
                    (keysCount, distances.Count).Out("keysCount: ");
                    bestKeysCount = keysCount;
                }

                if (keys == fullKeys && distance < minDistance)
                {
                    yield return minDistance = distance;
                }

                foreach (var (nextPos, newKey, stepDist) in GetMoves(posA, keys))
                {
                    var newKeys = keys.SetBit(newKey);
                    var newDist = distance + stepDist;
                    if (distances.TryGetValue((nextPos, posB, posC, posD, newKeys), out var dist) && dist <= newDist)
                        continue;
                    distances[(nextPos, posB, posC, posD, newKeys)] = newDist;
                    q.Enqueue((nextPos, posB, posC, posD, newKeys, newDist), newDist);
                }
                foreach (var (nextPos, newKey, stepDist) in GetMoves(posB, keys))
                {
                    var newKeys = keys.SetBit(newKey);
                    var newDist = distance + stepDist;
                    if (distances.TryGetValue((posA, nextPos, posC, posD, newKeys), out var dist) && dist <= newDist)
                        continue;
                    distances[(posA, nextPos, posC, posD, newKeys)] = newDist;
                    q.Enqueue((posA, nextPos, posC, posD, newKeys, newDist), newDist);
                }
                foreach (var (nextPos, newKey, stepDist) in GetMoves(posC, keys))
                {
                    var newKeys = keys.SetBit(newKey);
                    var newDist = distance + stepDist;
                    if (distances.TryGetValue((posA, posB, nextPos, posD, newKeys), out var dist) && dist <= newDist)
                        continue;
                    distances[(posA, posB, nextPos, posD, newKeys)] = newDist;
                    q.Enqueue((posA, posB, nextPos, posD, newKeys, newDist), newDist);
                }
                foreach (var (nextPos, newKey, stepDist) in GetMoves(posD, keys))
                {
                    var newKeys = keys.SetBit(newKey);
                    var newDist = distance + stepDist;
                    if (distances.TryGetValue((posA, posB, posC, nextPos, newKeys), out var dist) && dist <= newDist)
                        continue;
                    distances[(posA, posB, posC, nextPos, newKeys)] = newDist;
                    q.Enqueue((posA, posB, posC, nextPos, newKeys, newDist), newDist);
                }
            }
            distances.Count.Out("distances.Count: ");
            q.Count.Out("q.Count: ");

        }

        map[start.Y - 1][start.X - 1] = '@';
        map[start.Y - 1][start.X + 1] = '@';
        map[start.Y + 1][start.X - 1] = '@';
        map[start.Y + 1][start.X + 1] = '@';
        map[start.Y - 1][start.X] = '#';
        map[start.Y + 1][start.X] = '#';
        map[start.Y][start.X - 1] = '#';
        map[start.Y][start.X + 1] = '#';
        map[start.Y][start.X] = '#';

        var dists = DijkstraPart2(start+V.SE, start+V.SW, start+V.NE, start+V.NW);
        foreach (var dist in dists)
        {
            dist.Part2().ShouldBe(1984);
        }
    }
}