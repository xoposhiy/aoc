from aoc import *
import numpy as np
import matplotlib.pyplot as plt


def fold(instr):
    axis, v = instr[2:]
    coord = 1 if axis == 'y' else 0
    should_fold = dots[:, coord] > v
    dots[should_fold, coord] = v - (dots[should_fold, coord] - v)


dots, instructions = read_blocks(sep="[, =]")
dots = np.array(dots)
fold(instructions[0])
print("Part One", len(np.unique(dots, axis=0)))

last(map(fold, instructions[1:]))

plt.subplots(figsize=(16, 2))
plt.title("Part Two")
plt.scatter(dots[:, 0], -dots[:, 1], s=300)
plt.show()
