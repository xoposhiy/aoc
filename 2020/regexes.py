import functools
import re

inp = """shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags, 1 light yellow bag.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags."""


def extractall(text, regex, parsers=[]):
    ms = re.findall(regex, text)
    res = []
    for m in ms:
        item = []
        for i, g in enumerate(m):
            item.append(parsers[i](g) if i < len(parsers) and parsers[i] else g)
        res.append(item)
    return res


r = extractall(inp,
               r"(.+) bags contain (.+)\.",
               [None, lambda x: extractall(x, r"(\d+) (\w+ \w+) bags?")])
print(r)
