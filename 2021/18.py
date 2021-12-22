import math
import re
from aoc import *
import numpy as np
from functools import *
from itertools import *
import colorama

inp = [eval(line) for line in read_lines()]

colorama.init(autoreset=True)

def log(old, new):
    pass
    #print(new)


def red(num):
    while True:
        old = num
        exploded, num, _, _ = explode(num)
        if not exploded:
            num = split(num)
            if num == old:
                break
        log(old, num)
    return num


def add(a, b):
    return red([a, b])


def split(num):
    if isinstance(num, int):
        if num >= 10:
            return [num // 2, num // 2 + sign(num % 2)]
        else:
            return num
    split_left = split(num[0])
    if split_left != num[0]:
        return [split_left, num[1]]
    split_right = split(num[1])
    return [num[0], split_right]


def add_to_right(num, addition):
    if isinstance(num, int):
        return num + addition
    else:
        return [num[0], add_to_right(num[1], addition)]


def add_to_left(num, addition):
    if isinstance(num, int):
        return num + addition
    else:
        return [add_to_left(num[0], addition), num[1]]


def explode(num, d=0):
    if isinstance(num, int):
        return False, num, 0, 0
    if d < 4:
        exploded, next_num, left, right = explode(num[0], d + 1)
        if exploded:
            num = [next_num, add_to_left(num[1], right)]
            return True, num, left, 0
        exploded, next_num, left, right = explode(num[1], d + 1)
        if exploded:
            num = [add_to_right(num[0], left), next_num]
            return True, num, 0, right
        return False, num, 0, 0
    else:
        return True, 0, num[0], num[1]


def magnitude(num):
    if isinstance(num, int):
        return num
    return magnitude(num[0]) * 3 + magnitude(num[1]) * 2


inp[0] = red(inp[0])
res = reduce(add, inp)

print("Part One", magnitude(res))

max_s = 0
for a in inp:
    for b in inp:
        if a != b:
            c = add(a, b)
            s = magnitude(c)
            if s > max_s:
                max_s = s

print("Part Two", max_s)
