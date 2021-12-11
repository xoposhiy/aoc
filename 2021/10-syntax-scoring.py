import re
from aoc import *
import numpy as np
from functools import *
from itertools import *
scores = {
    ')': 3,
    ']': 57,
    '}': 1197,
    '>': 25137,
}

scores2 = {
    '(': 1,
    '[': 2,
    '{': 3,
    '<': 4,
}

pairs = {
    '(': ')',
    '[': ']',
    '<': '>',
    '{': '}',
}

inp = read_lines()

score = 0
totals = []
print(len(inp))
for line in inp:
    stack = []
    incorrect = False
    for i, c in enumerate(line):
        if c in pairs:
            stack.append(c)
        else:
            if c != pairs[stack.pop()]:
                score += scores[c]
                incorrect = True
                break
    if not incorrect:
        total = 0
        for c in reversed(stack):
            total = total * 5 + scores2[c]
        totals.append(total)


print("Part One", score)
print(totals)
print("Part Two", np.median(totals))
