import aoc
from functools import reduce

x = y = aim = 0
for cmd, n in aoc.read():
    match cmd:
        case "forward":
            x += n
            y += aim * n
        case "down":
            aim += n
        case "up":
            aim -= n

print("Part One", x * aim)
print("Part Two", x * y)

inp = aoc.read()


def process(state, command):
    x, y, aim = state
    match command:
        case "forward", arg:
            return [x+arg, y+arg*aim, aim]
        case "up", arg:
            return [x, y, aim-arg]
        case "down", arg:
            return [x, y, aim+arg]
        case _:
            raise Exception(str(command))


final_x, final_y, final_aim = reduce(process, inp, [0, 0, 0])
print(final_x * final_y)
print(final_x * final_aim)

