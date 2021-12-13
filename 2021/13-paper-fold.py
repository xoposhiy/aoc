from aoc import *

inp = read_blocks(sep="[, =]")


def fold(p, instr):
    _, _, axis, v = instr
    if axis == 'y' and p[1] > v:
        return p[0], v - (p[1] - v)
    if axis == 'x' and p[0] > v:
        return v - (p[0] - v), p[1]
    return p[0], p[1]


ps = inp[0]
ins = inp[1]

ps1 = set(fold(p, ins[0]) for p in ps)
print("Part One", len(ps1))

for i in ins:
    ps = {fold(p, i) for p in ps}

print("Part Two")
for y in range(6):
    for x in range(80):
        print("##" if (x, y) in ps else "  ", end='')
    print()
