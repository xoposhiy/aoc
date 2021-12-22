from functools import *
from itertools import *


def get_scores(pos):
    player_scores = [0, 0]
    player = 0
    dice = 1
    dice_rolls = 0
    while True:
        pos[player] += dice
        dice = (dice + 1) % 100
        pos[player] += dice
        dice = (dice + 1) % 100
        pos[player] += dice
        dice = (dice + 1) % 100
        pos[player] = (pos[player] - 1) % 10 + 1
        player_scores[player] += pos[player]
        dice_rolls += 3
        if player_scores[player] >= 1000:
            break
        player = 1 - player
    return player_scores, dice_rolls


scores, rolls = get_scores([4, 9])
print("Part One", scores, rolls, min(scores) * rolls)


@cache
def get_wins(pos, score, player):
    if score[1 - player] >= 21:
        return (0, 1) if player == 0 else (1, 0)
    total0, total1 = 0, 0
    for dices in product(*[[1, 2, 3]]*3):
        dice = sum(dices)
        pos2 = (pos[player] + dice - 1) % 10 + 1
        score2 = score[player] + pos2
        next_pos = (pos2, pos[1]) if player == 0 else (pos[0], pos2)
        next_score = (score2, score[1]) if player == 0 else (score[0], score2)
        w0, w1 = get_wins(next_pos, next_score, 1 - player)
        total0 += w0
        total1 += w1
    return total0, total1


print("Part Two", max(get_wins((9, 3), (0, 0), 0)))
