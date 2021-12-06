import functools

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


def oneliner(cooldowns, days):
    return functools.reduce(lambda counts, _: np.roll(counts, -1) + counts[0] * np.eye(9, dtype=int)[6], range(days), np.unique(cooldowns + list(range(0, 9)), return_counts=True)[1] - [1] * 9).sum()


def smart_solve(cooldowns, days):
    counts = [0] * 9
    for c in cooldowns:
        counts[c] += 1
    for t in range(days):
        counts[(7 + t) % 9] += counts[t % 9]
    return sum(counts)


def matrix_solve(cooldowns, days):
    counts = [0] * 9
    for c in cooldowns:
        counts[c] += 1
    matrix = np.roll(np.eye(9, dtype=object), 1, 1)
    matrix[6][0] = 1
    return np.dot(np.linalg.matrix_power(matrix, days), np.array(counts)).sum()


magics = {
    80: [1401, 1191, 1154, 1034, 950],
    256: [6206821033, 5617089148, 5217223242, 4726100874, 4368232009]
}


def precalc_solve(cooldowns, days):
    return sum(cooldowns.count(cooldown) * magic for cooldown, magic in enumerate(magics[days], 1))


inp = read(sep=",")[0]
print("Part One", solve(inp, 80))
print("Part Two", solve(inp, 256))
print("Part Two (numpy)", numpy_solve(inp, 256))
print("Part Two (smart)", smart_solve(inp, 256))
print("Part Two (matrix)", matrix_solve(inp, 256))
print("Part Two (oneliner)", oneliner(inp, 256))
print("Part Two (precalc)", precalc_solve(inp, 256))



