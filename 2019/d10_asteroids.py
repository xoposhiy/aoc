import math

m1 = [
".#..#",
".....",
"#####",
"....#",
"...##",
]
m2 = [
    "......#.#.",
    "#..#.#....",
    "..#######.",
    ".#.#.###..",
    ".#..#.....",
    "..#....#.#",
    "#..#....#.",
    ".##.#..###",
    "##...#..#.",
    ".#....####"]
m3 = [
    ".###..#######..####..##...#",
    "########.#.###...###.#....#",
    "###..#...#######...#..####.",
    ".##.#.....#....##.#.#.....#",
    "###.#######.###..##......#.",
    "#..###..###.##.#.#####....#",
    "#.##..###....#####...##.##.",
    "####.##..#...#####.#..###.#",
    "#..#....####.####.###.#.###",
    "#..#..#....###...#####..#..",
    "##...####.######....#.####.",
    "####.##...###.####..##....#",
    "#.#..#.###.#.##.####..#...#",
    "..##..##....#.#..##..#.#..#",
    "##.##.#..######.#..#..####.",
    "#.....#####.##........#####",
    "###.#.#######..#.#.##..#..#",
    "###...#..#.#..##.##..#####.",
    ".##.#..#...#####.###.##.##.",
    "...#.#.######.#####.#.####.",
    "#..##..###...###.#.#..#.#.#",
    ".#..#.#......#.###...###..#",
    "#.##.#.#..#.#......#..#..##",
    ".##.##.##.#...##.##.##.#..#",
    "#.###.#.#...##..#####.###.#",
    "#.####.#..#.#.##.######.#..",
    ".#.#####.##...#...#.##...#.",
]

m = m3

w = len(m[0])
h = len(m)

def count(x, y):
    aa = set()
    for ax in range(w):
        for ay in range(h):
            dx = ax-x
            dy = ay-y
            if (dx, dy) != (0, 0) and m[ay][ax] == '#':
                d = math.gcd(abs(dx), abs(dy))
                aa.add((dx//d, dy//d))
    return len(aa)

def angle_positive(x, y):
    a = math.atan2(x, -y)
    if a < 0:
        a += 2*math.pi
    return a

def vaporize(m, station):
    list = [(x, y, angle_positive(x-station[0], y-station[1]), (x-station[0])**2 + (y-station[1])**2) 
        for x in range(w) 
        for y in range(h) 
        if m[y][x] == '#' and (x, y) != station]
    list.sort(key=lambda a: (a[2], a[3]))
    res = []
    angle = -1000
    while len(list)>0:
        dead = next((p for p in list if p[2]>angle), None)
        if dead == None:
            angle = -1000
        else:
            res.append(dead)
            list.remove(dead)
            angle = dead[2]
            #print("FIRE ", dead)
    return res

candidates = [(x, y, count(x,y)) for x in range(0, w) for y in range(0, h) if m[y][x] == '#']
station = max(candidates, key=lambda x:x[2])
print(station)
print("Part One: ", station[2])
sequence = vaporize(m, station)
#print ("\n".join(map(str, sequence[0:200])))
print(sequence[199])
print("Part Two: ", sequence[199][0]*100+sequence[199][1])
