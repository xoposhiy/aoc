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
                .Select(Inst.Parse)
                .ToList();

        Console.WriteLine($"Part One: {Run(program).Acc}");
        Console.WriteLine($"Part Two: {RunFixedProgram(program).Acc}");
    }

    public record Vm(int Acc = 0, int Ip = 0, bool Halted = false)
    {
        public Vm Execute(Inst instruction) =>
            instruction switch
            {
                Acc(var x) => this with { Ip = Ip + 1, Acc = Acc + x },
                Jmp(var x) => this with { Ip = Ip + x },
                Nop(_) => this with { Ip = Ip + 1 },
                var op => throw new Exception($"{op}")
            };
    }

    public abstract record Inst {
        public static Inst Parse(string line) {
            var ps = line.Split(" ");
            var op = ps[0];
            var arg = int.Parse(ps[1]);
            return op switch
            {
                "nop" => new Nop(arg),
                "jmp" => new Jmp(arg),
                "acc" => new Acc(arg),
                _ => throw new Exception(op)
            };
        }
    }
    public sealed record Nop(int Arg) : Inst;
    public sealed record Jmp(int Arg) : Inst;
    public sealed record Acc(int Arg) : Inst;

    Vm Run(IReadOnlyList<Inst> program)
    {
        var vm = new Vm();
        var visited = new HashSet<int>();
        while (vm.Ip < program.Count && visited.Add(vm.Ip))
            vm = vm.Execute(program[vm.Ip]);
        return vm with { Halted = vm.Ip >= program.Count };
    }

    Vm RunFixedProgram(List<Inst> program)
    {
        return
            EnumerateMutations(program)
            .Select(Run)
            .First(res => res.Halted);
    }

    IEnumerable<List<Inst>> EnumerateMutations(List<Inst> program)
    {
        for (var index = 0; index < program.Count; index++)
        {
            var inst = program[index];
            program[index] = inst switch
            {
                Nop(var x) => new Jmp(x),
                Jmp(var x) => new Nop(x),
                var op => op
            };
            yield return program;
            program[index] = inst;
        }
    }
}
