from aoc import *


def read_bingo():
    blocks = read_blocks()
    draw = list(map(int, blocks[0][0][0].split(",")))
    cards = blocks[1:]
    return cards, draw


def win(card, x, y):
    size = len(card)
    return sum(1 for xx in range(size) if card[y][xx] is None) == size \
        or sum(1 for yy in range(size) if card[yy][x] is None) == size


def winners(cards, draw):
    won_cards = []
    for i, n in enumerate(draw):
        for card in cards:
            for y, row in enumerate(card):
                for x, _ in enumerate(row):
                    if row[x] == n:
                        row[x] = None
                        if win(card, x, y) and card not in won_cards:
                            won_cards.append(card)
                            yield n * sum(c for r in card for c in r if c is not None)


print("Part One", next(winners(*read_bingo())))
print("Part Two", last(winners(*read_bingo())))
