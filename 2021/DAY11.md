# Ищем интересное в AoC 2021 

## Day 11. Dumbo octopus

Условие задачи: https://adventofcode.com/2021/day/11

TLDR: 

1. Каждую итерацию энергия осьминога увеличивается на 1. 
2. Как только она достигает 10, осьминог вспыхивает:
   1. Энергия сбрасывается на 0. 
   2. У всех соседних ещё не вспыхнувших осьминогов энергия дополнительно вырастает на 1 и они тоже могут вспыхнуть и так далее.

Посчитайте количество вспышек и момент, когда все осьминоги синхронизируются

### DFS

Интересно, что DFS в итоге короче, чем решение в лоб (менять пока меняется).
Тут несколько вспомогательных функций общего назначения, 
которые я собираю в отдельном модуле. 
Должно быть понятно по их именам, что они делают.

```python
def step():
    flashes = flatten(map(flash, cells(matrix)))
    for x, y in flashes:
        matrix[y][x] = 0
    return len(flashes)


def flash(p):
    x, y, _ = p
    if matrix[y][x] > 9:
        return
    matrix[y][x] += 1
    if matrix[y][x] > 9:
        yield x, y
        yield from flatmap(flash, neighbours8(x, y, matrix))

```


### Convolution!

Очень надеялся найти на реддите красивые решения с векторными операциями numpy. Но нашлось только одно.

Всё просто! Достаточно сделать свертку девяток с единичным ядром и новая энергия готова! :)

```python
KERNEL = np.ones((3, 3), dtype=int)


def step(octos):
    octos += 1

    flashed = np.zeros_like(octos, dtype=bool)
    while (flashing := ((octos > 9) & ~flashed)).any():
        octos += convolve(flashing.astype(int), KERNEL, mode="constant")
        flashed |= flashing

    octos[flashed] = 0
    return flashed.sum()
```


---

Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
