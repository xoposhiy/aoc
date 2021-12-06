from aoc import *


def solve(counts, days):
    groups = [0] * 9
    for d in counts:
        groups[d] += 1
    for day in range(days):
        groups[7] += groups[0]
        groups = groups[1:] + [groups[0]]
    return sum(groups)


inp = read(sep=",")[0]
print("Part One", solve(inp, 80))
print("Part Two", solve(inp, 256))
