import re
from aoc import *
import numpy as np
from functools import *
from itertools import *


inp = read(sep="[:=,. ]|target|area|x|y")[0]


def simulate(vx, vy):
    x1, x2, y1, y2 = inp
    x = 0
    y = 0
    h = y
    while x <= x2 and y >= y1:
        x += vx
        y += vy
        h = max(h, y)
        if vx > 0:
            vx -= 1
        vy -= 1
        if x1 <= x <= x2 and y1 <= y <= y2:
            return h
    return -100500


max_h = -100500
count = 0
for vx in range(300):
    for vy in range(-200, 300):
        h = simulate(vx, vy)
        if h > -100500:
            count += 1
        if h > max_h:
            max_h = h
            print(max_h, vx, vy)

print("Part One", max_h)

print("Part Two", count)
