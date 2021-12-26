import re
from aoc import *
import numpy as np
from functools import *
from itertools import *

inp = [line[0] for line in read()]
h = len(inp)
w = len(inp[0])


def simulate(field):
    nxt_row = ['.']*w
    nxt = [nxt_row.copy() for i in range(h)]
    for y, row in enumerate(field):
        for x, v in enumerate(row):
            match v:
                case 'v':
                    nxt[y][x] = v
                case '>':
                    if field[y][(x + 1) % w] == '.':
                        nxt[y][(x + 1) % w] = v
                    else:
                        nxt[y][x] = v
    field = nxt
    nxt = [nxt_row.copy() for i in range(h)]
    for y, row in enumerate(field):
        for x, v in enumerate(row):
            match v:
                case '>':
                    nxt[y][x] = v
                case 'v':
                    if field[(y + 1) % h][x] == '.':
                        nxt[(y + 1) % h][x] = v
                    else:
                        nxt[y][x] = v
    return nxt


def print_field(f):
    for y, row in enumerate(f):
        print(''.join(row))
    print()


print_field(inp)
r = inp
i = 0
while True:
    r2 = simulate(r)
    i += 1
    if r == r2:
        break
    r = r2
    if i % 1000 == 0:
        print(i)
        print_field(r)

print("Part One", i)

print("Part Two", inp)
