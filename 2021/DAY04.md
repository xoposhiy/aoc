# Ищем интересное в AoC 2021 

## Day 4. Bingo!

### "Теперь у нас две проблемы" (с)

А что если представлять карточку Бинго в виде текста, а зачеркивать числа и проверять выигрыш с помощью регулярок?

Зачеркиваем число:

```perl
s/$called\b/X/;
```

Проверяем, не выиграли ли:
```perl
/^[X ]+$|X(?:.{14}X){4}/ms
```

### Без регулярок

Если представлять карточку, как двумерный массив, 
то для проверки на выигрыш напрашивается писать всякие мерзкие циклы.

А вот вам элегатный способ без этого:

```python
def win(card: List[List[int|None]], x: int, y: int) -> bool:
    return card[y].count(None) == 5 or transpose(card)[x].count(None) == 5
```

Разве что функции transpose в python нет, но в прошлом разборе упоминался простой способ сделать это:

```python
transposed_card = list(zip(*card))
```

Ещё вариант:

```ruby
def check_board(board, drawn)
  (board + board.transpose).any? { |line| line.all? { |n| drawn.include?(n) } }
end
```


Если отметки чисел хранить отдельно, то можно так:

```python
win = any(np.sum(marks, axis=0) == 5) or any(np.sum(marks, axis=1) == 5)
```


### Ковбойский ввод данных

На второй день я сделал себе в питоне мега функцию read, 
которую намерен использовать для ввода данных в каждой из следующих задач.
Она читает все строки, каждую из них разбивает на части, 
и каждую часть пытается распарсить как целое число, если не получилось, то как float,
если снова не получилось, оставляет строкой. Так на второй день, она возвращала такое:

```python
>> read()
[
    ["forward", 1],
    ["up", 5],
    ...
]
```

Сегодня это тоже помогло. Но захотелось большего! 
Написал над ней надстройку read_blocks, 
которая ещё и разбивает входные данные на блоки по пустой строке. 
В итоге весь ввод у меня выглядит так:

```python
def read_bingo() -> (List[Card], List[int]):
    draw_block, *cards = read_blocks(sep="[, ]+")
    return cards, draw_block[0]
```

Ставлю на то, что эта парочка функций сэкономит мне кучу усилий в этом году :)

### Как бы не копипастить?

Как нужно было написать part1, чтобы ее было легко использовать в part2?

Один из элегантных методов подсмотренных на реддите: 
сделать, чтобы part1 возвращал итератор 
по карточкам в порядке их выигрыша. 
Тогда part1 — это взять первый элемент из этого итератора,
а part2 — взять последний. Красиво! 

```python
def winners(cards: List[Card], draw: List[int]) -> Iterator[Card]:
    ...
```

И правки после открытия второй части требовались минимальные — всего лишь заменить return на yield.


## Краткость — сестра шифратора

Под конец, несколько удивительно коротких решений:

Python:
```python
def calculate_win_number_score(board, marks, num_list, i=0):
    num = num_list[i]
    marks[board == num] = True
    if any(np.sum(marks, axis=0) == 5) or any(np.sum(marks, axis=1) == 5):
        return [i, int(np.sum(board[marks==False]) * num)]
    return calculate_win_number_score(board, marks, num_list, i+1)

win_numbers_scores = np.array([calculate_win_number_score(board, np.zeros((5, 5)), nums) for board in boards])
print(win_numbers_scores[np.argmin(win_numbers_scores[:, 0]), 1])
print(win_numbers_scores[np.argmax(win_numbers_scores[:, 0]), 1])
```

Scala:
```scala
val scores = boardScores(nums, boards)
println(s"Part I: ${scores.head}, Part II: ${scores.last}")

def boardScores(nums: Seq[Int], boards: Seq[Seq[Seq[Int]]]): Seq[Int] = 
  nums.foldLeft(Seq.empty[Int], Set.empty[Int], boards) {
    case ((scores, marked, activeBoards), num) =>
      val newMarked     = marked + num
      val (won, active) = activeBoards.partition(b => (b ++ b.transpose).exists(_.forall(newMarked.contains)))
      val newScores     = won.map(board => num * board.flatten.filterNot(newMarked.contains).sum)
      (scores ++ newScores, newMarked, active)
  }._1
```

Perl:
```perl
local $/ = ''; my $calls = <>; my @board = <>; my $winner;
while ($calls =~ /(\d+)/g) {
  my $called = sprintf '%2d', $1;
  @board = grep {
    s/$called\b/##/;
    !/^[# ]+$|#(?:.{14}#){4}/ms
        or do { say $called * sum /(\d+)/g if !$winner++ || @board == 1; 0 }
  } @board;
}
```

Oneliner, как сказать, на Python :)
```python
print([sum([sum([col[0] if not col[1] else 0 for col in row]) for row in [b for b in winningBoards[0] if True in [all(e[i][1] is True for i in range(5)) for e in [row for row in b]] or True in [all(e[i][1] is True for i in range(5)) for e in [col for col in zip(*b)]]][0] ]) * winningBoards[1] for i in range(1) if (winningBoards := [[[boards := boards if True in [True in [all(e[i][1] is True for i in range(5)) for e in [row for row in L]] or True in [all(e[i][1] is True for i in range(5)) for e in [col for col in zip(*L)]] for L in boards[0]] else [[[[ board[i][j] if board[i][j][0] != number else [number, True] for i in range(5)] for j in range(5)] for board in boards[0]], number]] for number in [int(num) for num in [line.replace("\n", " ").strip() for line in open("input.txt", "r").readlines()][0].split(",")]][-1][0] for i in range(1) if (boards := [[[[[int(x), False] for x in e.strip().split(" ") if x != ""] for e in n.split("|")] for n in '|'.join([line.replace("\n", " ").strip() for line in open("input.txt", "r").readlines()][2:]).split("||")], 0]) is not None][0]) is not None][0])
```


До завтра!

---

Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
