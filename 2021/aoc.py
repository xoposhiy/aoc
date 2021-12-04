import sys
import os


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


def read():
    return list(map(parse_line, read_lines()))


def read_blocks():
    lines = list(map(parse_line, read_lines()))
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


def parse_line(line):
    return list(map(parse_value, line.split()))


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
