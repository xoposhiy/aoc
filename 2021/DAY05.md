# Ищем интересное в AoC 2021 

## Day 5. Hydrothermal Vents

### Ковбойский ввод данных

...работает как часы

```python
inp = read(sep=",| -> ")
```

### numpy — новый jQuery

А знаете что, оказывается, работает так же хорошо? 

```perl
while (<>) {
	my ($x1, $y1, $x2, $y2) = /\d+/g;
    ...
```

Это был Perl :)

Но то же самое умеет и numpy.fromregex:

```python
data = np.fromregex(open(0), '\d+', [('',int)]*4)
```

Да и продлжение тоже красивое:

```python
grid = np.zeros((2, 1000, 1000))
for (x, y, X, Y) in data:
    dx, dy = np.sign([X-x, Y-y])                 
    while (x,y) != (X+dx, Y+dy):
        grid[dx * dy, x, y] += 1
        x+=dx; y+=dy

print((grid[0]>1).sum(), (grid.sum(0)>1).sum())
```
В последней строчке опять векторные операции.
dx*dy == 0 только для горизонтальных или вертикальных линий. 
Поэтому работает первая часть. 
sum(0) — суммирование по всем значениям dx * dy, поэтому работет вторая часть.

### В Python нет функции sign

Но оригинальный способ ее написать:

```python
sign = lambda n: (n>0) - (n<0)
```

В некоторых языках вместо отсутствующего sign можно использовать оператор сравнения:

Perl:
```perl
my ($dx, $dy) = ($x2 <=> $x1, $y2 <=> $y1);
```

### Математическое решение

Если всё предыдущее вам кажется недостаточно математичным, держите комплексные числа:

```python
c, r, v = defaultdict(int), re.compile(r"\d+"), []

for l in fileinput.input():
    x1, y1, x2, y2 = map(int, r.findall(l))
    v.append((complex(x1, y1), complex(x2, y2)))

for diagonal in (False, True):
    for a, b in v:
        if diagonal ^ (a.real == b.real or a.imag == b.imag):
            i = b - a
            i /= max(abs(i.real), abs(i.imag))
            while a != b + i:
                c[a] += 1
                a += i

    print(sum(v > 1 for v in c.values()))
```

Этот способ получения всех точек отрезка, можно выделить,
и переписать с классом вектора V вместо комплексных чисел, 
чтобы было не так страшно:

```c#
    IEnumerable<V> Range(V a, V b)
    {
        var ba = (b - a);
        return Enumerable.Range(0, ba.CLen + 1).Select(i => a + ba / ba.CLen * i);
    }
```

CLen — чебышевская длина вектора.

## Решение в один expression

```c#
    int Part2(string[] input) =>
        input
            .Select(s => s.Split(" -> ").Select(ParseV).ToArray())
            .SelectMany(p => Range(p[0], p[1]))
            .ToLookup(p => p)
            .Count(g => g.Count() > 1);
```
## Красивое решение на Perl целиком

Ввод в одну строку. Подсчет всех точек отрезка в одну строку. Вывод в одну строку. Красота же!

```perl
my %vents1;
my %vents2;
while (<>) {
    my ($x1, $y1, $x2, $y2) = /[0-9]+/g;
    my ($dx, $dy) = ($x2 <=> $x1, $y2 <=> $y1);
    my $dist = abs ($x1 - $x2) || abs ($y1 - $y2);
    unless ($dx * $dy) {
        $vents1{$x1 + $_ * $dx, $y1 + $_ * $dy}++ for 0 .. $dist;
    }
    $vents2{$x1 + $_ * $dx, $y1 + $_ * $dy}++ for 0 .. $dist;
}
say "Solution 1: ", scalar grep {$_ > 1} values %vents1;
say "Solution 2: ", scalar grep {$_ > 1} values %vents2;
```

Если вам сложно читать Perl, то вот краткая справка:
* 
* $x — переменная.
* %x — словарь.
* <> — ввод из стандартного потока в дефолтную переменную (которую можно не указывать для краткости).
* /[0-9]+/g — матчинг дефолтной переменной по регулярке.
* $dict{$key}++ — увеличение значения в словаре по ключу.
* grep — ищет в массиве по условию.
* scalar grep ... - возвращает "скалярный контекст", то есть количество найденного.


### SQL?!

Почему бы и нет!

```sql
WITH parsed AS (
    SELECT regexp_match(str, '^(\d+),(\d+) -> (\d+),(\d+)') AS coord FROM day5
), coords AS (
    SELECT coord[1]::INT x1, coord[2]::INT y1, coord[3]::INT x2, coord[4]::INT y2 FROM parsed
), vectors AS (
    SELECT x1, y1, sign(x2-x1) AS dx, sign(y2-y1) AS dy, GREATEST(ABS(x1-x2), ABS(y1-y2)) AS length FROM coords
), points AS (
    SELECT x1 + i * dx AS x, y1 + i * dy AS y, dx * dy != 0 AS is_diag FROM vectors, generate_series(0, length) AS i
), multiples_part1 AS (
    SELECT x, y FROM points WHERE NOT is_diag GROUP BY x, y HAVING COUNT(*) > 1
), multiples_part2 AS (
    SELECT x, y FROM points GROUP BY x, y HAVING COUNT(*) > 1
)
SELECT (SELECT COUNT(*) FROM multiples_part1) AS part1_answer, (SELECT COUNT(*) FROM multiples_part2) AS part2_answer;
```

### Лайв от leijurv (8 место в глобальном лидерборде)

https://www.youtube.com/watch?v=WgpwKtt2R4M&ab_channel=LurfJurv

---

Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
