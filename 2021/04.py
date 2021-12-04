import aoc


def read():
    inp = aoc.read()
    draw = list(map(int, inp[0][0].split(",")))
    cards = []
    next_card = []
    for i in range(2, len(inp)):
        if len(inp[i]) == 0:
            cards.append(next_card)
            next_card = []
        else:
            next_card.append(inp[i])
    cards.append(next_card)
    return cards, draw


def win(card, x, y):
    if sum(1 for xx in range(5) if card[y][xx] is None) == 5:
        return True
    if sum(1 for yy in range(5) if card[yy][x] is None) == 5:
        return True


def part1():
    cards, draw = read()
    for i in range(len(draw)):
        n = draw[i]
        for card in cards:
            for y in range(len(card)):
                row = card[y]
                for x in range(len(row)):
                    if row[x] == n:
                        row[x] = None
                        if win(card, x, y):
                            print(card)
                            return sum(c for r in card for c in r if c is not None) * n
    return None


def handle(cards, n):
    score = None
    won = []
    for card in cards:
        for y in range(len(card)):
            row = card[y]
            for x in range(len(row)):
                if row[x] == n:
                    row[x] = None
                    if win(card, x, y):
                        won.append(card)
                        score = sum(c for r in card for c in r if c is not None) * n
    for w in won:
        cards.remove(w)
    return score


def part2():
    cards, draw = read()
    score = 0
    for i in range(len(draw)):
        n = draw[i]
        s = handle(cards, n)
        if s is not None:
            score = s
    return score


print("Part One", part1())
print("Part Two", part2())
