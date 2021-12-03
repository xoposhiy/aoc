# Ищем интересное в AoC 2021 

## Day 3. Binary Diagnostic

Всё понятно! Просто транспонируем, цифры в каждом столбце сгруппируем, отсортируем по размеру группы, возьмем ключ первой/последней группы, заджойним обратно в строку. Го писать!

```ruby
inputs = $<.to_a.map(&:strip).map(&:chars)

# Part 1
g = inputs.transpose.map { |x| x.group_by{|y|y}.sort_by{_2.length}[-1][0] }.join.to_i(2)
e = inputs.transpose.map { |x| x.group_by{|y|y}.sort_by{_2.length}[0][0] }.join.to_i(2)
p [g,e,g*e]
```

Эм... но самый частый бит можно и попроще найти: 

```ruby
bits.count('1') >= bits.count('0') ? '1' : '0'
```

А знаете как сделать transpose на python и js? 

```python
zip(*inputs)
```

Так, отставить! Вообще-то задача про биты!

```python
gamma = 0
for i in range(bits):
    gamma_bit = 2*sum((x >> i) & 1 for x in data) >= len(data)
    gamma |= gamma_bit << i
```

А ещё эпсилон можно вычислить из gamma всего за 3 элементарных операции, применив немного [битовых хаков](https://graphics.stanford.edu/~seander/bithacks.html).

```python
epsilon = gamma ^ ((1 << bits_count) - 1)
```

С битами понятно, а как там поживают наши математизированные языки? Pandas:

```python
df = pd.DataFrame(inputs)
gamma = [df[col].value_counts().index[0] for col in df.columns]

```

Неплохо. Но numpy + scipy может и лучше:

```python
inputs = numpy.array([list(map(int, line)) for line in inp])
gamma = scipy.stats.mode(inputs)[0][0]
```

Помните такой термин из статистики _мода_ — значение, которое встречается чаще всего в выборке. Вот, это оно! 

А знаете как получить epsilon из gamma без битовых трюков?

```python
epsilon = 1 - gamma
```

Узнали векторное вычитание? :)

Для второй задачи нужно оставить только те элементы списка, в которых на i-ой позиции нужные биты. 
Это стандартная операция для numpy.

```python
inputs = inputs[inputs[:, bit] == 1]
```

А вы чего, всё циклы пишите? Ну, не расстраивайтесь, вот код второго места в глобальном лидерборде, он тоже циклы пишет:

Part 1

```python
from collections import Counter

ll = [x for x in open('input').read().strip().split('\n')]

theta = ''
epsilon = ''
for i in range(len(ll[0])):
	common = Counter([x[i] for x in ll])
	if common['0'] > common['1']:
		theta += '0'
		epsilon += '1'
	else:
		theta += '1'
		epsilon += '0'
print(int(theta,2)*int(epsilon,2))
```

Part 2

```python
from collections import Counter

ll = [x for x in open('input').read().strip().split('\n')]

theta = ''
epsilon = ''
for i in range(len(ll[0])):
	common = Counter([x[i] for x in ll])

	if common['0'] > common['1']:
		ll = [x for x in ll if x[i] == '0']
	else:
		ll = [x for x in ll if x[i] == '1']
	theta = ll[0]

ll = [x for x in open('input').read().strip().split('\n')]
for i in range(len(ll[0])):
	common = Counter([x[i] for x in ll])

	if common['0'] > common['1']:
		ll = [x for x in ll if x[i] == '1']
	else:
		ll = [x for x in ll if x[i] == '0']
	if ll:
		epsilon = ll[0]
print(int(theta,2)*int(epsilon,2))
```

На этом всё, встретимся завтра!

---

Все решения либо взяты [из реддита](https://www.reddit.com/r/adventofcode/) (и иногда доделаны),
либо мои собственные.