import re
from aoc import *
import numpy as np
from functools import *
from itertools import *
from vm24 import *
#from vmm24 import vms2
from collections import defaultdict


def printvms():
    inp = read()
    i = 0
    s = "vms=["
    for inst in inp:
        match inst:
            case 'inp', name:
                if i > 0:
                    print("    return z")
                print(f"def vm{i}(z,w):")
                s += f"vm{i}, "
                i += 1
            case 'mul', a, 0:
                print(f"    {a} = 0")
            case 'mul', a, b:
                print(f"    {a} *= {b}")
            case 'add', a, b:
                print(f"    {a} += {b}")
            case 'div', a, b:
                print(f"    {a} //= {b}")
            case 'mod', a, b:
                print(f"    {a} %= {b}")
            case 'eql', a, b:
                print(f"    {a} = 1 if {a} == {b} else 0")
    print("    return x,y,z")
    print(f"{s}]")


# printvms()

@cache
def find(d, target_z):
    res = set()
    for z in range(100000):
        for w in range(1, 10):
            r1 = vms2[d](z, w)
            if target_z == r1:
                res.add(z)
    return res


def get_dp(d):
    dp = defaultdict(list)
    for z in range(1000000):
        for w in range(1, 10):
            r1 = vms2[d](z, w)
            dp[r1].append((z, str(w)))

    print("  dp", len(dp))
    return dp

def run():
    variants = set()
    variants.add(0)
    for d in range(13, -1, -1):
        next_variants = set()
        print("  start", d, len(variants))
        dp = get_dp(d)
        for z in variants:
            for tz, tw in dp[z]:
                next_variants.add(tz)
        variants = next_variants
        probe = str(variants) if len(variants) < 50 else "many"
        print("end", d, len(variants), probe)


@cache
def search(depth, target):
    variants = dp[(depth, target)]
    if variants is None:
        return None
    for z, w in variants:
        if depth == 0:
            return str(w), []
        res = search(depth - 1, z)
        if res is not None:
            print(depth, res)
            w0, z0 = res
            return str(w) + w0, [z] + z0
    return None

def check(w):
    z = 0
    for i, vm in enumerate(vms):
        wi = int(w[i])
        z = vm(z, wi)
        print(i, z)

check('97919997299495')


# print(search(13, 0))

"""

"""
