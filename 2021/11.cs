using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

int Flash(IDictionary<(int x, int y), char> energy, (int x, int y) pos)
{
    if (energy[pos] <= '9') return 0;
    energy[pos] = '0';
    var count = 1;
    var (x, y) = pos;
    for (var nx = x - 1; nx <= x + 1; nx++)
    for (var ny = y - 1; ny <= y + 1; ny++)
    {
        var next = (nx, ny);
        if (next != pos && energy.TryGetValue(next, out var value) && value > '0')
        {
            energy[next]++;
            count += Flash(energy, next);
        }
    }
    return count;
}

int Step(Dictionary<(int x, int y), char> energy)
{
    foreach (var pos in energy.Keys)
        energy[pos]++;
    return energy.Keys.Sum(pos => Flash(energy, pos));
}

var lines = File.ReadAllLines("11.txt");
var energy = lines.SelectMany((line, y) => line.Select((v, x) => (pos: (x, y), v)))
    .ToDictionary(t => t.pos, t => t.v);

var total = 0;
var time = 1;
while (true)
{
    var flashesCount = Step(energy);
    total += flashesCount;
    Console.WriteLine(flashesCount);
    if (time == 100)
        Console.WriteLine($"Part One: {total}");
    if (flashesCount == 100)
    {
        Console.WriteLine($"Part Two: {time}");
        break;
    }
    time++;
}