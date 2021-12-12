from collections import defaultdict
from aoc import *

edges = read(sep="-")


def other(e, u):
    return e[0] if e[0] != u else e[1]


def dfs(start, end, visited, max_visits):
    if start == end:
        return 1
    c = 0
    for edge in edges:
        n = other(edge, start)
        if start not in edge or visited[n] >= max_visits:
            continue
        if n.islower():
            visited[n] += 1
        c += dfs(n, end, visited, min(max_visits, 1 if visited[n] >= 2 else 2))
        if n.islower():
            visited[n] -= 1
    return c


v = defaultdict(int)
v['start'] = 2
print("Part One", dfs("start", "end", v, 1))

v = defaultdict(int)
v['start'] = 2
print("Part Two", dfs("start", "end", v, 2))
