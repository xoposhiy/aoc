from aoc import *


def read_bingo():
    draw_block, *cards = read_blocks(sep="[, ]+")
    return cards, draw_block[0]


def win(card, x, y):
    size = len(card)
    return card[y].count(None) == size or transpose(card)[x].count(None) == size


def winners(cards, draw):
    won_cards = []
    for n in draw:
        for card in cards:
            for y, row in enumerate(card):
                for x, _ in enumerate(row):
                    if row[x] == n:
                        row[x] = None
                        if win(card, x, y) and card not in won_cards:
                            won_cards.append(card)
                            yield n * sum(v for v in flatten(card) if v is not None)


print("Part One", next(winners(*read_bingo())))
print("Part Two", last(winners(*read_bingo())))
