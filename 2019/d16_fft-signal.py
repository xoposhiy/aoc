import math

base_pattern = [0, 1, 0, -1]

def phase(ns):
    return [mult(ns, base_pattern, pos) for pos in range(len(ns))]

def mult(line, pattern, pos):
    i = 1
    s = 0
    for x in line:
        s += x*pattern[(i//(pos+1)) % len(pattern)]
        i += 1
    return abs(s) % 10

inp2 = "03036732577212944063491565474664"
inp = "59762574510031092870627555978901048140761858379740610694074091049186715780458779281173757827279664853239780029412670100985236587608814782710381775353184676765362101185238452198186925468994552552398595814359309282056989047272499461615390684945613327635342384979527937787179298170470398889777345335944061895986118963644324482739546009761011573063020753536341827987918039441655270976866933694280743472164322345885084587955296513566305016045735446107160972309130456411097870723829697443958231034895802811058095753929607703384342912790841710546106752652278155618050157828313372657706962936077252259769356590996872429312866133190813912508915591107648889331"

line = list(map(int,inp))

def part1(line):
    for i in range(100):
        line = phase(line)

    return "".join(map(str, line))[0:8]

def part2(line, offset):
    res = []
    left = len(line)*10000 - offset
    n = 100
    for i in range(8):
        m = 1
        s = 0
        for j in range(left):
            x = line[(i+offset+j) % len(line)]
            s = (s + x * m) % 10
            m = (m * (n+j) // (j+1))
        res.append(s)
    return "".join(map(str, res))


# print("Part One:", part1(line))
n = 100
m = 1
for j in range(1):
    print(m % 10)
    m = (m * (n+j) // (j+1))

offset = int(inp[0:7])
print("offset: ", offset)
print("Part Two:", part2(line, offset))



