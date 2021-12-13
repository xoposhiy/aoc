from aoc import *
import numpy as np
import matplotlib.pyplot as plt


def fold(points, instr):
    _, _, axis, v = instr
    coord = 1 if axis == 'y' else 0
    should_fold = points[:, coord] > v
    points[should_fold, coord] = v - (points[should_fold, coord] - v)


inp = read_blocks(sep="[, =]")
ps = np.array(inp[0])
ins = inp[1]

fold(ps, ins[0])
print("Part One", len(np.unique(ps, axis=0)))

for i in ins[1:]:
    fold(ps, i)

f, ax = plt.subplots(figsize=(16, 2))
ax.scatter(ps[:, 0], -ps[:, 1], s=300)
ax.set_title("Part Two")
plt.show()
