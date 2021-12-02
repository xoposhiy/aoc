import aoc

x = 0
y = 0
aim = 0
for cmd, n in aoc.read():
    if cmd == "forward":
        x += n
        y += aim * n
    if cmd == "down":
        aim += n
    if cmd == "up":
        aim -= n

print("Part One", x * aim)
print("Part Two", x * y)
