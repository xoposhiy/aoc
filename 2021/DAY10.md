# Ищем интересное в AoC 2021 

## Day 10. Syntax Scoring

Условие задачи: https://adventofcode.com/2021/day/10

### Everybody step back!

```python
import re

pattern = r'\{\}|\[\]|\(\)|\<\>'
while re.search(pattern, aoc_input): 
    aoc_input = re.sub(pattern, '', aoc_input)
corrupt = [line.lstrip('{[<(') for line in aoc_input.splitlines() if any(c in line for c in ')}]>')]
missing = [line for line in aoc_input.splitlines() if not any(c in line for c in ')}]>')]
```
---

Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
