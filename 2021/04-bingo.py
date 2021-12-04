from aoc import *
from typing import List
from collections.abc import Iterator

Card = List[List[int]]


def read_bingo() -> (List[Card], List[int]):
    draw_block, *cards = read_blocks(sep="[, ]+")
    return cards, draw_block[0]


def win(card: Card, x: int, y: int) -> bool:
    size = len(card)
    return card[y].count(-1) == size or transpose(card)[x].count(-1) == size


def winners(cards: List[Card], draw: List[int]) -> Iterator[int]:
    won: List[Card] = []
    for n in draw:
        for card in cards:
            for y, row in enumerate(card):
                for x, _ in enumerate(row):
                    if row[x] == n:
                        row[x] = -1
                        if win(card, x, y) and card not in won:
                            won.append(card)
                            yield n * sum(v for v in flatten(card) if v is not None)


print("Part One", next(winners(*read_bingo())))
print("Part Two", last(winners(*read_bingo())))
