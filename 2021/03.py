import aoc
from itertools import groupby

inp = aoc.read_lines()
gamma = epsilon = 0
power2 = 1
i = 0
num_len = len(inp[0])
for i in range(0, num_len):
    bits = sorted([int(line[i]) for line in inp])
    b = sorted([(k, len(list(v))) for k, v in groupby(bits)], key=lambda t: t[1])

    gamma = gamma * 2 + b[-1][0]
    epsilon = epsilon * 2 + b[0][0]
print("Part One", gamma, epsilon, gamma*epsilon)


def filter_report(lines, index):
    for i in range(0, num_len):
        bits = sorted([int(line[i]) for line in lines])
        b = sorted([(k, len(list(v))) for k, v in groupby(bits)], key=lambda t: t[1])
        lines = [line for line in lines if line[i] == str(b[index][0])]
        if len(lines) == 1:
            return lines[0]


ox = int(filter_report(inp, -1), 2)
co2 = int(filter_report(inp, 0), 2)
print("Part Two", ox, co2, ox*co2)
