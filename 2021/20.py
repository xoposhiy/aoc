import re
from aoc import *
import numpy as np
from functools import *
from itertools import *

inp = read_blocks()
algo = inp[0][0][0]
print(algo)
raw_field = inp[1]
f = {}
for y, line in enumerate(raw_field):
    for x, c in enumerate(line[0]):
        f[(x, y)] = c


def step2(alive):
    res = {}
    candidates = set()
    x0 = min(p[0] for p in alive)
    y0 = min(p[1] for p in alive)
    x1 = max(p[0] for p in alive)
    y1 = max(p[1] for p in alive)
    for x in range(x0-3, x1+4):
        for y in range(y0-3, y1+4):
            index = 0
            for ny in [y - 1, y, y + 1]:
                for nx in [x - 1, x, x + 1]:
                    index = index * 2
                    if (nx, ny) in alive and alive[(nx, ny)] == '#':
                        index += 1
            res[(x, y)] = algo[index]
    alive = res
    res = {}
    for x in range(x0-2, x1+3):
        for y in range(y0-2, y1+3):
            index = 0
            for ny in [y - 1, y, y + 1]:
                for nx in [x - 1, x, x + 1]:
                    index = index * 2
                    if (nx, ny) in alive and alive[(nx, ny)] == '#':
                        index += 1
            res[(x, y)] = algo[index]

    return res


f = step2(f)

res = sum(1 for p in f.keys() if f[p] == '#')
print(algo)
print("Part One", res)

for i in range(24):
    print()
    f = step2(f)
res = sum(1 for p in f.keys() if f[p] == '#')

print("Part Two", res)
