from aoc import *
import numpy as np


def solve(cooldowns, days):
    groups = [0] * 9
    for c in cooldowns:
        groups[c] += 1
    for _ in range(days):
        groups[7] += groups[0]
        groups = groups[1:] + [groups[0]]
    return sum(groups)


def numpy_solve(cooldowns, days):
    counts = np.unique(cooldowns + list(range(0, 9)), return_counts=True)[1] - [1] * 9
    for _ in range(days):
        counts = np.roll(counts, -1) + counts[0] * np.eye(9, dtype=int)[6]
    return counts.sum()


def smart_solve(cooldowns, days):
    counts = [0] * 9
    for c in cooldowns:
        counts[c] += 1
    for t in range(days):
        counts[(7 + t) % 9] += counts[t % 9]
    return sum(counts)


inp = read(sep=",")[0]
print("Part One", solve(inp, 80))
print("Part Two", solve(inp, 256))
print("Part Two (numpy)", numpy_solve(inp, 256))
print("Part Two (smart)", smart_solve(inp, 256))
