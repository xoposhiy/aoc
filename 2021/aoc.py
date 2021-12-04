import sys
import os
import re


def flatten(list2):
    return [x for sublist in list2 for x in sublist]


def transpose(list2):
    return list(zip(*list2))


def last(iterable):
    *_, last_item = iterable
    return last_item


def read_lines():
    fn = os.path.basename(sys.argv[0])[0:2]
    fh = open(fn + ".txt", "r")
    try:
        return fh.read().splitlines()
    finally:
        fh.close()


def read(sep=None):
    return [parse_line(line, sep) for line in read_lines()]


def read_blocks(sep=None):
    lines = read(sep)
    blocks = []
    block = []
    for line in lines:
        if len(line) == 0:
            blocks.append(block)
            block = []
        else:
            block.append(line)
    blocks.append(block)
    return blocks


def parse_line(line, sep=None):
    parts = line.split() if sep is None else re.split(sep, line)
    return [parse_value(item) for item in parts if item != '']


def parse_value(value):
    i = try_parse_int(value)
    if i is not None:
        return i
    f = try_parse_float(value)
    if f is not None:
        return f
    return value


def try_parse_int(value):
    try:
        return int(value)
    except ValueError:
        return None


def try_parse_float(value):
    try:
        return float(value)
    except ValueError:
        return None


def read_ints():
    return list(map(int, read_lines()))
