import aoc

ds = aoc.read_ints()
c1 = sum([1 for t in zip(ds, ds[1:]) if t[0] < t[1]])
print("Part One", c1)

c2 = sum([1 for t in zip(ds, ds[3:]) if t[0] < t[1]])
print("Part Two", c2)
