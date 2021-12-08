import numpy as np
from intcode import run

inp = np.loadtxt('07.txt', delimiter=',', dtype=int)

# Part 1
x = int(np.median(inp))
delta = abs(inp - x)
print('Part One', delta.sum())


# Part 2
#
# $$f = \sum_{i=1..n} \frac{|x_i - x|(|x_i - x| + 1)}{2} = $$
# $$= \frac{1}{2}\sum_{i=1..n} (x_i - x)^2 + \frac{1}{2}\sum_{i=1..n} |x_i - x| =$$
# $$= \frac{1}{2}\sum_{i=1..n} (x_i - x)^2 + \frac{1}{2}\sum_{i=1..n, x_i > x} (x_i - x)  + \frac{1}{2}\sum_{i=1..n, x_i \le x} (x - x_i)$$
# $$ f'(x) = 0$$
# $$ f'(x) = \frac{1}{2} \sum_{i=1..n} (x_i^2 - 2 x_i x + x^2)' - \frac{m}{2}  + \frac{n-m}{2} = $$
# $$ = \sum_{i=1..n} (- x_i + x) - \frac{2m - n}{2} = $$
# $$ = x n - \sum_{i=1..n} x_i - n \epsilon, \; \epsilon \in [-0.5 .. 0.5]$$
# $$ f'(x) = 0 $$
# $$ x = \frac{1}{n} \sum_{i=1..n} x_i + \epsilon $$
#
# https://www.overleaf.com/read/jkmrbmfpqqrf

candidates = []
for dx in [-1, 0, 1]:
    x = np.mean(inp) + dx
    delta = abs(inp - x).round().astype(int)
    candidates.append((delta * (delta + 1) // 2).sum())

print('Part Two', min(candidates))

p, output = run(inp, [])
print(output)
