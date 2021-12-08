from aoc import *

inp = read(sep=" \\| ")

entries = [(entry[0].split(' '), entry[1].split(' ')) for entry in inp]
ans = sum(sum(1 for p in entry[1] if len(p) in [2, 3, 4, 7]) for entry in entries)

print("Part One", ans)

digits = {
    'abcefg': 0,
    'cf': 1,
    'acdeg': 2,
    'acdfg': 3,
    'bcdf': 4,
    'abdfg': 5,
    'abdefg': 6,
    'acf': 7,
    'abcdefg': 8,
    'abcdfg': 9
}


def solve():
    ans2 = 0
    for signals, displays in entries:
        def find_signal(segments_count, other_digit='', intersection_size=0):
            candidates = [
                s for s in signals
                if len(s) == segments_count
                and len(set(other_digit) & set(s)) == intersection_size]
            assert len(candidates) == 1
            return ''.join(sorted(candidates[0]))

        d1 = find_signal(2)
        d4 = find_signal(4)
        d7 = find_signal(3)
        d8 = find_signal(7)
        d3 = find_signal(5, d1, 2)
        d2 = find_signal(5, d4, 2)
        d5 = find_signal(5, d2, 3)
        d6 = find_signal(6, d1, 1)
        d9 = find_signal(6, d3, 5)
        d0 = find_signal(6, d5, 4)
        mapping = dict([(d1, 1), (d2, 2), (d3, 3), (d4, 4), (d5, 5), (d6, 6), (d7, 7), (d8, 8), (d9, 9), (d0, 0)])

        n = 0
        for display in displays:
            n = n*10 + mapping[''.join(sorted(display))]
        ans2 += n
    return ans2


def solve_sets():
    ans2 = 0
    for signals, displays in entries:

        def contains_all_segments(signal, required_segments):
            return len(required_segments - set(signal)) == 0

        def find_signal_with(segments_count, required_segments=set()):
            signal = next(
                s for s in signals if len(s) == segments_count and contains_all_segments(s, required_segments))
            return set(signal)

        d1 = find_signal_with(2)
        d4 = find_signal_with(4)
        d8 = find_signal_with(7)
        d7 = find_signal_with(3)
        fc = d1
        a = d7 - d1
        d3 = find_signal_with(5, a | fc)
        dg = d3 - fc - a
        d9 = find_signal_with(6, dg | fc)
        b = d9 - dg - fc - a
        d = d4 - fc - b
        g = dg - d
        e = d8 - d9
        d6 = find_signal_with(6, a | dg | e)
        c = d8 - d6
        f = fc - c

        letters = [list(x)[0] for x in [a, b, c, d, e, f, g]]
        mapping = {}
        for i in range(7):
            mapping[letters[i]] = 'abcdefg'[i]
        n = 0

        for d in displays:
            d2 = ''.join(sorted(map(lambda ch: mapping[ch], d)))
            digit = digits[d2]
            n = n * 10 + digit
        ans2 += n
    return ans2


print("Part Two", solve_sets())
print("Part Two", solve())
