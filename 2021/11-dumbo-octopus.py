from aoc import *

matrix = read_map()


def step():
    flashes = flatten(map(flash, cells(matrix)))
    for x, y in flashes:
        matrix[y][x] = 0
    return len(flashes)


def flash(p):
    x, y, _ = p
    if matrix[y][x] > 9:
        return
    matrix[y][x] += 1
    if matrix[y][x] > 9:
        yield x, y
        yield from flatmap(flash, neighbours8(x, y, matrix))


total = 0
for i in range(100500):
    f = step()
    total += f
    if i == 99:
        print("Part One", total)
    if f == 100:
        print("Part Two", i + 1)
        break
