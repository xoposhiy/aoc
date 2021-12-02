# Ищем интересное в AoC 2021 

## Day 2. Dive!

Сегодня входные данные выглядят уж больно похоже на код:

```
forward 1
down 9
down 4
forward 4
down 2
down 7
...
```

А что если выбрать язык, в котором функции вызываются без скобочек, написать свои функции forward, up и down и выполнить входные данные как код? Встречайте решение на Red!

```red
Red["Dive!"]

h: 0 d: 0 a: 0

forward: func[n][
    h: n + h
    d: a * n + d
]
up: func[n][a: a - n]
down: func[n][a: a + n]

reduce load %input.txt
print [h * a h * d]
```

А что, если как-нибудь использовать паттерн-матчинг? Привет от Erlang-а с его определением функции part1 по частям:

```erlang
part1([{"forward", N}|Tl]) -> add_tuple({N,0}, part1(Tl));
part1([{"down", N}|Tl]) -> add_tuple({0,N}, part1(Tl)); 
part1([{"up", N}|Tl]) -> add_tuple({0,-N}, part1(Tl));
part1([]) -> {0,0}.
```

А регулярки — это же тоже как бы паттерн-матчинг? Значит AWK должен подойти!

```awk
/forward/ {h+=$2;d+=a*$2};
/up/ 	  {a-=$2};
/down/ 	  {a+=$2}
END 	  {print h*a,h*d}
```

Эта задачка уже не так круто подходит математичным языкам с их векторными операциями. Хотя погодите...

```python
forward_sum = data[data[0] == 'forward'][1].astype(int).sum()
up_sum      = data[data[0] == 'up'][1].astype(int).sum()
down_sum    = data[data[0] == 'down'][1].astype(int).sum()

forward_sum * (down_sum-up_sum)
```

Ладно, ладно, эти трюки работают только для первой задачи. Для второй всё придётся переписывать.

Кстати, почему мы решая эти задачки совсем не думаем про производительность? А вы про архитектуру современных процессоров слышали? Конвейер выполнения? Брэнч-предикторы? Что некоторые неудачные if-ы могут замедлять программу в десятки раз? Так что давайте, завязывайте с этими if-ами!

```c
int main() {

  char buffer[16];
  int v = 0, h = 0, pos, a[2] = {-1, 1};
  while (fgets(buffer, 15, stdin)) {
    h += ((buffer[0] & 0x2) >> 1) * atoi(buffer + 7);
    pos = (((buffer[0] & 0x3f) >> 3) ^ 2) - 1;
    v += a[pos >> 2] * atoi(buffer + pos);
  }

  printf("v=%d h=%d v*h=%d\n", v, h, v*h);
}
```

Кого я обманываю, [всем пофиг на производительность](https://tonsky.me/blog/disenchantment/ru/), так что...

Python 10:

```python
def move(submarine, command):
    x, y, aim = submarine
    match command:
        case "forward", arg:
            return [x+arg, y+arg*aim, aim]
        case "up", arg:
            return [x, y, aim-arg]
        case "down", arg:
            return [x, y, aim+arg]
        case _:
            raise Exception(str(command))


final_x, final_y, final_aim = functools.reduce(move, inp, [0, 0, 0])
print(final_x * final_y)
print(final_x * final_aim)
```

---

Все решения либо взяты [из реддита](https://www.reddit.com/r/adventofcode/comments/r6zd93/2021_day_2_solutions/) (и иногда доделаны),
либо мои собственные.