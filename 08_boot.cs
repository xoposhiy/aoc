using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Day8
{
    public void Solve()
    {
        var program =
            File.ReadAllLines("08.txt")
                .Select(s => s.Split(" "))
                .Select(a => (op: a[0], arg: int.Parse(a[1])))
                .ToList();

        Console.WriteLine($"Part One: {Run(program).acc}");
        //Console.WriteLine($"Part Two: {GetAccOfFixedProgram(program)}");
    }

    (bool looped, int acc) Run(List<(string op, int arg)> program)
    {
        var ip = 0;
        var acc = 0;
        var visited = new HashSet<int>();
        while (ip < program.Count)
        {
            var inst = program[ip];
            if (!visited.Add(ip)) return (true, acc);
            ip = inst.op switch {
                "acc" => ip + 1 + (acc+=inst.arg) - acc, // Mua-ha-haha!!! >:D
                "jmp" => ip + inst.arg,
                _ => ip + 1,
            };
        }
        return (false, acc);
    }

    int GetAccOfFixedProgram(List<(string op, int arg)> program)
    {
        return
            Mutate(program)
            .Select(Run)
            .Where(res => !res.looped)
            .First()
            .acc;
    }

    IEnumerable<List<(string op, int arg)>> Mutate(List<(string op, int arg)> program)
    {
        for (var index = 0; index < program.Count; index++)
        {
            var inst = program[index];
            var newOp = inst.op switch
            {
                "nop" => "jmp",
                "jmp" => "nop",
                var op => op
            };
            program[index] = (newOp, inst.arg);
            yield return program;
            program[index] = inst;
        }
    }
}
