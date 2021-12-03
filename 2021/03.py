import aoc


def most_common_bits(nums, bit_index):
    digits = list(zip(*nums))[bit_index]
    return ['0', '1'] if digits.count('0') > digits.count('1') else ['1', '0']


def from_binary(bits):
    return int(''.join(bits), 2)


def filter_report(lines, most_common):
    for i in range(0, num_len):
        bit_value = most_common_bits(lines, i)[0 if most_common else 1]
        lines = [x for x in lines if x[i] == bit_value]
        if len(lines) == 1:
            return lines[0]


inp = aoc.read_lines()
num_len = len(inp[0])
gamma = [most_common_bits(inp, i)[0] for i in range(num_len)]
epsilon = [most_common_bits(inp, i)[1] for i in range(num_len)]
print("Part One:", from_binary(gamma) * from_binary(epsilon))

ox = int(filter_report(inp, most_common=True), 2)
co2 = int(filter_report(inp, most_common=False), 2)
print("Part Two:", ox * co2)
