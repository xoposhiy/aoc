from aoc import *
import numpy as np
from collections import Counter

inp = read(sep=",| -> ")


def points(line, ignore_diagonals=False):
    x0, y0, x1, y1 = line
    dy = np.sign(y1 - y0)
    dx = np.sign(x1 - x0)
    if ignore_diagonals and dx != 0 and dy != 0:
        return
    yield x0, y0
    while x0 != x1 or y0 != y1:
        x0 += dx
        y0 += dy
        yield x0, y0


def cross_count(ignore_diagonals):
    counter = Counter(flatten(points(line, ignore_diagonals) for line in inp))
    return sum(1 for group in counter.most_common() if group[1] > 1)


print("Part One", cross_count(ignore_diagonals=True))
print("Part Two", cross_count(ignore_diagonals=False))
