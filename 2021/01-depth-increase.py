import aoc
import more_itertools


input = aoc.read_ints()
c1 = sum(a < b for a, b in zip(input, input[1:]))
print("Part One", c1)

c2 = sum(a < b for a, b in zip(input, input[3:]))
c22 = sum(sum(input[i:i + 3]) < sum(input[i + 1:i + 4]) for i in range(0, len(input) - 3))
c23 = sum(w[0] < w[-1] for w in more_itertools.windowed(input, 4))
c24 = len([1 for a, b in zip(input, input[3:]) if a < b])

print("Part Two", c2, c22, c23, c24)
