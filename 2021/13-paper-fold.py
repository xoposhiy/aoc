from aoc import *
import numpy as np
import matplotlib.pyplot as plt


def fold(points, instr):
    axis, v = instr[2:]
    coord = 1 if axis == 'y' else 0
    should_fold = points[:, coord] > v
    points[should_fold, coord] = v - (points[should_fold, coord] - v)


dots, instructions = read_blocks(sep="[, =]")
dots = np.array(dots)
fold(dots, instructions[0])
print("Part One", len(np.unique(dots, axis=0)))

for i in instructions[1:]:
    fold(dots, i)

plt.subplots(figsize=(16, 2))
plt.title("Part Two")
plt.scatter(dots[:, 0], -dots[:, 1], s=300)
plt.show()
