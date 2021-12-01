import math

class Part:
    def __init__(self, name, count):
        self.name = name
        self.count = count

    def __str__(self):
        return  str(self.count)+" "+self.name

class Rule:
    def __init__(self, parts, res):
        self.parts = parts
        self.res = res

    def __str__(self):
        return ", ".join(map(str, self.parts)) + " => " + str(self.res)

def parse_part(line):
    [count, name] = line.split(' ')
    return Part(name, int(count))

def parse_rule(line):
    [left, right] = line.strip().split(" => ")
    parts = left.split(", ")
    return Rule(list(map(parse_part, parts)), parse_part(right))

def read_rules(fn):
  with open(fn) as f:
    return list(map(parse_rule, f.readlines()))

def find_ore_count(rs, fuel):
    required = {"FUEL": fuel}
    while True:
        x = next((n for n in required.keys() if n != "ORE" and required[n] > 0), None)
        if x == None: break
        rule = next(r for r in rs if r.res.name == x)
        factor = math.ceil(required[x] / rule.res.count)
        required[x] -= rule.res.count * factor
        for p in rule.parts:
            if not (p.name in required):
                required[p.name] = 0
            required[p.name] += factor*p.count
    return required["ORE"]

def find_fuel_count(rs, total_ore):
    l = 0
    r = total_ore
    while r - l > 1:
        fuel = (l + r) // 2
        ore = find_ore_count(rs, fuel)
        if ore <= total_ore:
            l = fuel
        else:
            r = fuel
    return l

rs = read_rules("14.txt")
print("Part One: ", find_ore_count(rs, 1))
print("Part Two: ", find_fuel_count(rs, 1000000000000))