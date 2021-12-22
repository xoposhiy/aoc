import re
from aoc import *
import numpy as np
from functools import *
from itertools import *
from collections import Counter
import time

start = time.time()

orientations1 = [
    [0, 1, 2],
    [0, 2, 1],
    [1, 2, 0],
    [1, 0, 2],
    [2, 0, 1],
    [2, 1, 0],
]
orientations2 = [
    [1, 1, 1],
    [-1, 1, 1],
    [1, -1, -1],
    [1, -1, 1],
    [-1, 1, -1],
    [1, 1, -1],
    [-1, -1, 1],
    [-1, -1, -1],
]

def transform(data, o1, o2):
    for d in data:
        yield o2[0] * d[o1[0]], o2[1] * d[o1[1]], o2[2] * d[o1[2]]


inp = read_blocks(sep=",")
ns = len(inp)
used = {0}
ss = [[0, 0, 0, 0]]
bs = {0: set(map(tuple, inp[0][1:]))}
print(ns)


def dist(a, b):
    return max(abs(a[0]-b[0]), abs(a[1]-b[1]), abs(a[2]-b[2]))


def analyse_scanner_pos(scanner_pos, data, known_beacons):
    errors = 0
    matches = 0
    for b in known_beacons:
        if dist(b, scanner_pos) >= 1000:
            continue
        b_relative_to_scanner = (b[0]-scanner_pos[0], b[1] - scanner_pos[1], b[2] - scanner_pos[2])
        if b_relative_to_scanner in data:
            matches += 1
        else:
            errors += 1
            return errors, matches
    return 0, matches


def try_match_scanner(data, known_beacons):
    for b_known in known_beacons:
        for b_cur in data:
            scanner_pos = (
                b_known[0] - b_cur[0],
                b_known[1] - b_cur[1],
                b_known[2] - b_cur[2]
            )
            errors, matches = analyse_scanner_pos(scanner_pos, data, known_beacons)
            if errors == 0 and matches >= 3:
                print(f"matched scanner {i_scanner}. Matches {matches}. position: {scanner_pos}")
                return scanner_pos
    return None


def add(a, b):
    return a[0] + b[0], a[1] + b[1], a[2] + b[2]


def try_locate_beacons(data, known_beacons):
    for io1, o1 in enumerate(orientations1):
        for io2, o2 in enumerate(orientations2):
            if (io1 + io2) % 2 == 1: continue
            transformed_data = list(transform(data, o1, o2))
            scanner_pos = try_match_scanner(transformed_data, known_beacons)
            if scanner_pos is not None:
                print(f"orientation: {o1} {o2}")
                return scanner_pos, list(map(lambda pos: add(pos, scanner_pos), transformed_data))
    return None, None

scanners = []
while len(used) < ns:
    for i_scanner in range(1, ns):
        if i_scanner not in used:
            known_scanners = list(used)
            for i_known_scanner in known_scanners:
                # print(f"try match {i_scanner} by known {i_known_scanner}")
                known_beacons = bs[i_known_scanner]
                scanner_pos, new_beacons = try_locate_beacons(inp[i_scanner][1:], known_beacons)
                if scanner_pos is None:
                    continue
                scanners.append(scanner_pos)
                print("known scanner matched with: ", i_known_scanner)
                used.add(i_scanner)
                bs[i_scanner] = new_beacons
                # print(f"beacons: {len(bs)} {set(new_beacons) & set(known_beacons)}")
                break
    print(f"repeat. used {used}")


print("Part One", len(set(list(flatten(bs.values())))))


def mdist(a, b):
    return abs(a[0]-b[0]) + abs(a[1]-b[1]) + abs(a[2]-b[2])


print("Part Two", max([mdist(s1, s2) for s1 in scanners for s2 in scanners]))

print("time", time.time()-start)
