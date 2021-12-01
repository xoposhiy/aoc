using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Day19
{
    public void Solve()
    {
        var inp = File.ReadAllLines("19.txt");

        var rules = inp.TakeWhile(s => s != "")
            .Select(s => s.Split(": "))
            .OrderBy(p => int.Parse(p[0]))
            .ToList();
        Console.WriteLine(rules.StrJoin("\n", s => s.StrJoin(",")));
        var messages = inp.Skip(rules.Count + 1).ToList();

        var cachePart1 = CompileAll(rules);
        var re0 = "^" + cachePart1[rules[0][1]] + "$";
        Console.WriteLine(re0);
        Console.WriteLine($"Part One: {messages.Count(s => Regex.IsMatch(s, re0))}");

        rules[8] = new[] { "8", "42 | 42 8" };
        rules[11] = new[] { "11", "42 31 | 42 11 31" };

        var cachePart2 = CompileAll(rules);
        var re0_2 = "^" + cachePart2[rules[0][1]] + "$";
        Console.WriteLine(re0_2);
        Console.WriteLine($"Part Two: {messages.Count(s => Regex.IsMatch(s, re0_2))}");
    }

    private static Dictionary<string, string> CompileAll(List<string[]> rules)
    {
        var cache = new Dictionary<string, string>();
        foreach (var rule in rules)
        {
            var ruleBody = rule[1];
            cache[ruleBody] = Compile(ruleBody, cache, rules);
        }

        return cache;
    }

    private static string Compile(string ruleBody, IReadOnlyDictionary<string, string> cache, List<string[]> rules)
    {
        if (cache.TryGetValue(ruleBody, out var res)) return res;
        if (ruleBody == "42 | 42 8")
            return Compile(rules[42][1], cache, rules) + "+";
        if (ruleBody == "42 31 | 42 11 31")
        {
            var rule42 = Compile(rules[42][1], cache, rules);
            var rule31 = Compile(rules[31][1], cache, rules);
            return "(?'o'" + rule42 + ")+(?'c-o'" + rule31 + ")+";
        }
        if (Regex.IsMatch(ruleBody, @"^""a|b""$")) return ruleBody.Substring(1, 1);
        var combination = Regex.Match(ruleBody, @"^(.*) \| (.*)$");
        if (combination.Success)
            return $"{Compile(combination.Groups[1].Value, cache, rules)}|{Compile(combination.Groups[2].Value, cache, rules)}";
        return ruleBody.Split(" ").Select(int.Parse).StrJoin("", i => "(" + Compile(rules[i][1], cache, rules) + ")");
    }
}