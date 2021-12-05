import itertools
import sys
import os
import re
from typing import TypeVar
from collections.abc import Iterator

InputValue = int | float | str
InputLine = list[InputValue]
InputBlock = list[InputLine]
T = TypeVar('T')


def freq(items):
    return [(len(list(g[1])), g[0]) for g in itertools.groupby(sorted(items))]


def sign(x):
    if x == 0:
        return 0
    return -1 if x < 0 else 1


def flatten(list2):
    return [x for sublist in list2 for x in sublist]


def transpose(list2):
    return list(map(list, zip(*list2)))


def last(iterator: Iterator[T]) -> T:
    *_, last_item = iterator
    return last_item


def read_lines() -> list[str]:
    fn = os.path.basename(sys.argv[0])[0:2]
    fh = open(fn + ".txt", "r")
    try:
        return fh.read().splitlines()
    finally:
        fh.close()


def f() -> list[int]:
    return [None, "sdf", 42]


def read(sep: str = None) -> list[InputLine]:
    return [parse_line(line, sep) for line in read_lines()]


def read_blocks(sep: str = None) -> list[InputBlock]:
    lines = read(sep)
    blocks: list[InputBlock] = []
    block: InputBlock = []
    for line in lines:
        if len(line) == 0:
            blocks.append(block)
            block = []
        else:
            block.append(line)
    blocks.append(block)
    return blocks


def parse_line(line: str, sep: str = None) -> InputLine:
    parts: list[str] = line.split() if sep is None else re.split(sep, line)
    return [parse_value(item) for item in parts if item != '']


def parse_value(value: str) -> InputValue:
    i = try_parse_int(value)
    if i is not None:
        return i
    f = try_parse_float(value)
    if f is not None:
        return f
    return value


def try_parse_int(value: str) -> int | None:
    try:
        return int(value)
    except ValueError:
        return None


def try_parse_float(value: str) -> float | None:
    try:
        return float(value)
    except ValueError:
        return None
