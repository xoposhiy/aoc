# Ищем интересное в AoC 2021 

## Day 6. Lanternfish

```ruby
(0...days).each do |d|
    newFish = groups.shift
    groups[6] += newFish
    groups.push(newFish)
end

groups.reduce(:+)
```

```c#
for (var t = 0; t < days; t++) {
    fishCountByInternalTimer[(t + 7) % 9] += fishCountByInternalTimer[t % 9];
}
```

---

Автор большинства сниппетов выше не я — они взяты [из реддита](https://www.reddit.com/r/adventofcode/) твиттера или нашего [телеграм чата](https://t.me/konturAoC2021_chat).
