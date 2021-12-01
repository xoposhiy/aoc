import aoc

def getFuel(ms):
  return sum(map(getModuleFuel, ms))

def getModuleFuel(m):
  return max(0, m // 3 - 2)

def getModuleFuelRec(m):
  # print(m)
  if m < 9:
    return 0
  f = getModuleFuel(m)
  return f + getModuleFuelRec(f)

def getFullFuel(m):
  res = getModuleFuel(m)
  return res + getModuleFuelRec(res)
  

print(getFullFuel(14))
print(getFullFuel(1969))
print(getFullFuel(100756))

ms = aoc.readInts("./01.txt")
print("Part One: ", getFuel(ms))
print("Part Two: ", sum(map(getFullFuel, ms)))