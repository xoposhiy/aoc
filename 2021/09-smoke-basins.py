import math
from aoc import *

inp = read_lines()
risk = 0
basins = []
for y, row in enumerate(inp):
    for x, level in enumerate(row):
        if all(nv > level for (_, _, nv) in neighbours4(x, y, inp)):
            risk += 1 + int(level)
            basin = maze_dfs(inp, (lambda v: v != '9'), x, y)
            basins.append(len(list(basin)))

print("Part One", risk)
print("Part Two", math.prod(sorted(basins)[-3:]))
