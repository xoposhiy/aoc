import aoc

x = 0
y = 0
aim = 0
for line in aoc.read_lines():
    cmd, arg = line.split()
    n = int(arg)
    if cmd == "forward":
        x += n
        y += aim * n
    if cmd == "down":
        aim += n
    if cmd == "up":
        aim -= n

print("Part One", x * aim)
print("Part Two", x * y)
