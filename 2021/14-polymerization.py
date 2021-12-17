from aoc import *
from collections import Counter
from collections import defaultdict

words, rules_list = read_blocks(sep=" -> ")
rules = dict(rules_list)
word = words[0][0]


def solve(iterations_count):
    bigrams = defaultdict(int)
    for c1, c2 in zip(word, word[1:]):
        bigrams[c1 + c2] += 1
    for i in range(iterations_count):
        next_bigrams = defaultdict(int)
        for k in bigrams:
            count = bigrams[k]
            next_bigrams[rules[k] + k[1]] += count
            next_bigrams[k[0] + rules[k]] += count
        bigrams = next_bigrams
    c = defaultdict(int)
    for k in bigrams:
        c[k[0]] += bigrams[k]
    c[word[-1]] += 1
    mc = Counter(c).most_common()
    return mc[0][1] - mc[-1][1]


print("Part One", solve(10))
print("Part Two", solve(40))
assert solve(10) == 2621
assert solve(40) == 2843834241366
