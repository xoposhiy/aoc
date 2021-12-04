import aoc
import numpy as np
import scipy.stats


def most_common_bits(nums, bit_index):
    column = list(zip(*nums))[bit_index]
    return ['0', '1'] if column.count('0') > column.count('1') else ['1', '0']


def filter_report(lines, most_common):
    for i in range(0, bits_count):
        bit_value = most_common_bits(lines, i)[0 if most_common else 1]
        lines = [x for x in lines if x[i] == bit_value]
        if len(lines) == 1:
            return lines[0]


inp = aoc.read_lines()
bits_count = len(inp[0])
gamma = int(''.join([most_common_bits(inp, i)[0] for i in range(bits_count)]), 2)
epsilon = ~gamma & ((1 << bits_count) - 1)
print("Part One:", gamma * epsilon)

ox = int(filter_report(inp, most_common=True), 2)
co2 = int(filter_report(inp, most_common=False), 2)
print("Part Two:", ox * co2)



gamma = 0
data = [int(line, 2) for line in inp]

for i in range(bits_count):
    gamma_bit = 2*sum((x >> i) & 1 for x in data) >= len(data)
    gamma |= gamma_bit << i

print(gamma)

# Task 1
inputs = np.array([list(map(int, line)) for line in inp])
gamma = scipy.stats.mode(inputs)[0][0]
epsilon = 1 - gamma

print(int(''.join(map(str, gamma)), 2) * int(''.join(map(str, epsilon)), 2))
