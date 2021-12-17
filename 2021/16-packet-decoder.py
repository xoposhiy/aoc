import functools
import re
from aoc import *
import numpy as np
from functools import *
from itertools import *

d = {'0': '0000', '1': '0001', '2': '0010', '3': '0011', '4': '0100',
     '5': '0101', '6': '0110', '7': '0111', '8': '1000', '9': '1001',
     'A': '1010', 'B': '1011', 'C': '1100', 'D': '1101', 'E': '1110', 'F': '1111'}

inp = list(map(str, flatten([d[c] for c in read()[0][0]])))

ver_sum = 0


def parse_literal(bits, i):
    num = []
    while True:
        chunk = bits[i:i + 5]
        num.append(chunk[1:])
        i += 5
        if chunk[0] == '0':
            v = int(''.join(flatten(num)), 2)
            return i, v


def sum_versions(type_id, args, version):
    if type_id == 4:
        return version
    return version + sum(args)

def apply(type_id, args, _):
    match type_id:
        case 0:
            return sum(args)
        case 1:
            return functools.reduce(lambda a, b: a * b, args)
        case 2:
            return min(args)
        case 3:
            return max(args)
        case 4:
            return args[0]
        case 5:
            return 1 if args[0] > args[1] else 0
        case 6:
            return 1 if args[0] < args[1] else 0
        case 7:
            return 1 if args[0] == args[1] else 0
    raise Exception(str(type_id))


def apply_str(type_id, args, _):
    match type_id:
        case 0:
            return '(' + '+'.join(map(str, args)) + ')'
        case 1:
            return '(' + '*'.join(map(str, args)) + ')'
        case 2:
            return 'min(' + ','.join(map(str, args)) + ')'
        case 3:
            return 'max(' + ','.join(map(str, args)) + ')'
        case 4:
            return args[0]
        case 5:
            return f'({args[0]} > {args[1]})'
        case 6:
            return f'({args[0]} < {args[1]})'
        case 7:
            return f'({args[0]} == {args[1]})'
    raise Exception(str(type_id))


def parse(bits, index, app):
    global ver_sum
    version = int(''.join(bits[index:index + 3]), 2)
    index += 3
    ver_sum += version
    type_id = int(''.join(bits[index:index + 3]), 2)
    index += 3
    if type_id == 4:
        index, v = parse_literal(bits, index)
        return index, app(type_id, [v], version)
    elif bits[index] == '0':
        index += 1
        bits_count = int(''.join(bits[index:index + 15]), 2)
        index += 15
        start = index
        args = []
        while index < start + bits_count:
            index, value = parse(bits, index, app)
            args.append(value)
        v = app(type_id, args, version)
        return index, v
    else:
        index += 1
        packet_count = int(''.join(bits[index:index + 11]), 2)
        index += 11
        args = []
        for i in range(packet_count):
            index, value = parse(bits, index, app)
            args.append(value)
        v = app(type_id, args, version)
        return index, v


_, result = parse(inp, 0, sum_versions)
print("Part One", ver_sum)
print("Part One", result)

_, result = parse(inp, 0, apply)
print(result)

print("Part Two", result)

_, text = parse(inp, 0, apply_str)
print(text)
