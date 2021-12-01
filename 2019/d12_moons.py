import math
from functools import reduce

moons = [
    [
        [15, 0],
        [-5, 0],
        [0, 0],
        [5, 0],
    ],
    [
        [-2, 0],
        [-4, 0],
        [-6, 0],
        [9, 0],

    ],
    [
        [-6, 0],
        [-11, 0],
        [0, 0],
        [6, 0],
    ],
]

def sign(x) -> int:
    if x < 0: return -1
    elif x > 0: return 1
    else: return 0

def lcm(a,b):
    return a*b // math.gcd(a,b)

def sim(moons, _):
    for i in range(3):
        for m1 in moons[i]:
            for m2 in moons[i]:
                m1[1] += sign(m2[0]-m1[0])
    for i in range(3):
        for m1 in moons[i]:
            m1[0] += m1[1]
    return moons

def energy(moons):
    e = 0
    for mi in range(4):
        p = 0
        k = 0
        for i in range(3):
            p += abs(moons[i][mi][0])
            k += abs(moons[i][mi][1])
        e += p*k
    return e

moons1 = reduce(sim, range(1000), moons)
print("Part One: ", energy(moons1))

it = 1
states = [str(m) for m in moons]
periods = {}
while len(periods) < 3:
    sim(moons, 0)

    for i in range(0, 3):
        if i in periods:
            continue
        s = str(moons[i])
        if s == states[i]:
            print("Got period of", i, "it is", it)
            periods[i] = it
    it += 1

ps = list(periods.values())
print("Part Two: ", lcm(lcm(ps[0], ps[1]), ps[2]))
