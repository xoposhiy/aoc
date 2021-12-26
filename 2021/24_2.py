import re
from aoc import *
import numpy as np
from functools import *
from itertools import *
from collections import defaultdict


def calc(vm, z, w):
    xx, yy, zz = vm
    x = 0 if z % 26 + xx == w else 1
    return (z // zz) * (25 * x + 1) + (w + yy) * x


def get_results(vm, zs):
    res = {}
    for z in zs.keys():
        # for w in range(9, 0, -1):
        for w in range(1, 10):
            r = calc(vm, z, w)
            if r not in res:
                res[r] = zs[z]*10 + w
    return res


inp = read()
data = []
zs = {
    0: 0
}
for i, inst in enumerate(inp):
    if inst[0] == 'inp':
        xx = inp[i+5][2]
        yy = inp[i+15][2]
        zz = inp[i+4][2]
        vm = (xx, yy, zz)
        zs = get_results(vm, zs)
        minz = min(zs.keys())
        print(len(data), len(zs), "minz", minz, "maxz", max(zs.keys()), "zs[minz]", zs[minz])
        data.append(vm)

def check(w):
    z = 0
    for i, vm in enumerate(data):
        wi = int(w[i])
        z = calc(vm, z, wi)
        print(i, z)

check('97919997299495')



