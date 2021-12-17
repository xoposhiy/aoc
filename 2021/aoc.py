import itertools
import sys
import os
import re
from typing import TypeVar
from collections.abc import Iterator
import time

InputValue = int | float | str
InputLine = list[InputValue]
InputBlock = list[InputLine]
T = TypeVar('T')


def cells(matrix):
    for y, row in enumerate(matrix):
        for x, v in enumerate(row):
            yield x, y, v


def neighbours8(x: int, y: int, matrix):
    for xx, yy in [(x - 1, y - 1), (x, y - 1), (x + 1, y - 1),
                   (x - 1, y), (x + 1, y),
                   (x - 1, y + 1), (x, y + 1), (x + 1, y + 1)]:
        if 0 <= yy < len(matrix) and 0 <= xx < len(matrix[0]):
            yield xx, yy, matrix[yy][xx]


def neighbours4(x: int, y: int, matrix):
    w = len(matrix[0])
    h = len(matrix)
    for xx, yy in [(x - 1, y), (x + 1, y), (x, y + 1), (x, y - 1)]:
        if 0 <= yy < h and 0 <= xx < w:
            yield xx, yy, matrix[yy][xx]


def measure(name, f):
    start = time.time()
    print(name, f(), time.time() - start)


def maze_dfs(maze, passable, x, y, used=None):
    if used is None:
        used = set()
    used.add((x, y))
    yield x, y
    for xx, yy, value in neighbours4(x, y, maze):
        if (xx, yy) not in used and passable(value):
            used.add((xx, yy))
            yield from maze_dfs(maze, passable, xx, yy, used)


def sign(x):
    if x == 0:
        return 0
    return -1 if x < 0 else 1


def flatmap(f, items):
    return flatten(map(f, items))


def flatten(list2):
    return [x for sublist in list2 for x in sublist]


def transpose(list2):
    return list(map(list, zip(*list2)))


def last(iterator: Iterator[T]) -> T:
    *_, last_item = iterator
    return last_item


def read_map() -> list[list[int]]:
    return [list(map(int, line)) for line in read_lines()]


def read_mapdict() -> dict[(int, int), InputValue]:
    matrix = read_map()
    return {(x, y): matrix[y][x] for x, y in cells(matrix)}


def read_lines() -> list[str]:
    fn = os.path.basename(sys.argv[0])[0:2]
    fh = open(fn + ".txt", "r")
    try:
        return fh.read().splitlines()
    finally:
        fh.close()


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
