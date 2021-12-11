# Ищем интересное в AoC 2021 

## Day 9. Smoke Basin

Условие задачи: https://adventofcode.com/2021/day/9

Просто короткое решение!

Фишки: 

1. Хранить поле в виде словаря точка → высота.
2. Удалять из этого словаря точку при посещении, чтобы не заводить отдельный visited.
3. Рекурсивный DFS в count_basin
4. `neighbours(*p)` вместо `neighbours(p[0], p[1])` 
5. `math.prod(sorted(basins)[-3:])` для получения ответа во второй задаче.

```python
from math import prod

height = {(x,y):int(h) for y,l in enumerate(open(0))
                       for x,h in enumerate(l.strip())}

def neighbours(x, y):
  return [n for n in [(x,y-1),(x,y+1),(x-1,y),(x+1,y)] if n in height]

def is_low(p):
  return all(height[p] < height[n] for n in neighbours(*p))

low_points = list(filter(is_low, height))
print(sum(height[p]+1 for p in low_points))

def count_basin(p):
  if height[p] == 9: 
    return 0 
  del height[p]
  return 1+sum(map(count_basin, neighbours(*p)))

basins = [count_basin(p) for p in low_points]
print(prod(sorted(basins)[-3:]))
```


### Image processing toolbox time!

Всегда найдется язык программирования, в котором задача уже решена в одной из стандартных библиотек!

Matlab:
```matlab
input = replace(readmatrix("input.txt",'OutputType','string'),'',' ');
input = cell2mat(arrayfun(@str2num,input,'UniformOutput',false));

mins = imregionalmin(input,4);
part1 = sum(input(mins)+1)

connectedAreas = bwconncomp(input<9,4).PixelIdxList;
sizes = cellfun(@numel,connectedAreas);
part2 = prod(maxk(sizes,3))
```

Python:
```python
#part 2
from collections import Counter
from skimage import measure

mask = (inputday9 != 9).astype(int).values

labeled_basins = measure.label(mask, background=0, connectivity=1)

c = Counter(labeled_basins.flatten())

top_sizes = [x[1] for x in c.most_common()[1:4]]

top_sizes[0] * top_sizes[1] * top_sizes[2]

```

### np.pad

Все становится проще, если вокруг карты добавить рамочку из девяток. 

![pad](https://i.redd.it/67nu5wfl6i481.png)

Но если вам всегда лень возиться с этой рамочкой, то встречайте numpy.pad!

```python
height_map = np.pad(height_map, pad_width=1, mode="constant", constant_values=9)
```

### 1D

Если карту хранить в одномерном массиве, то шаги вверх и вниз — это шаги на width влево и вправо.

Perl:

```perl
sub size($i, $map, $width) {
  return if $map->[$i] == 9;
  $map->[$i] = 9;
  1 + sum0 map { size($_, $map, $width) } $i-$width, $i-1, $i+1, $i+$width;
}
```


---
Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
