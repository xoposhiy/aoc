import aoc
import more_itertools


inp = aoc.read_ints()
c1 = sum(a < b for a, b in zip(inp, inp[1:]))
print("Part One", c1)

c2 = sum(a < b for a, b in zip(inp, inp[3:]))
c22 = sum(sum(inp[i:i + 3]) < sum(inp[i + 1:i + 4]) for i in range(0, len(inp) - 3))
c23 = sum(w[0] < w[-1] for w in more_itertools.windowed(inp, 4))
c24 = len([1 for a, b in zip(inp, inp[3:]) if a < b])

print("Part Two", c2, c22, c23, c24)
