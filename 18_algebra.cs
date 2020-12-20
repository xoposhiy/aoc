using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Day18
{
    public void Solve()
    {
        var inp = File.ReadAllLines("18.txt");
        Console.WriteLine($"Part One (RPN): {inp.Select(e => Eval(e, op => op switch { "+" => 0, "*" => 0, _ => 2 })).Sum()}");
        Console.WriteLine($"Part Two (RPN): {inp.Select(e => Eval(e, op => op switch { "+" => 0, "*" => 1, _ => 2 })).Sum()}");
        Console.WriteLine($"Part One (RegExes): {inp.Select(e => EvalWithRegex(e, @"\+|\*")).Sum()}");
        Console.WriteLine($"Part Two (RegExes): {inp.Select(e => EvalWithRegex(e, @"\+", @"\*")).Sum()}");
    }

    // Solution with Reverse Polish Notation:
    
    private long Eval(string expr, Func<string, int> operationPriority)
    {
        var tokens = expr.Replace("(", "( ").Replace(")", " )").Split(" ");
        return EvalPostfix(ToPostfix(tokens, operationPriority));
    }

    private long EvalPostfix(IEnumerable<string> expr)
    {
        var stack = new Stack<long>();
        foreach (var token in expr)
        {
            if (token == "+")
                stack.Push(stack.Pop() + stack.Pop());
            else if (token == "*")
                stack.Push(stack.Pop() * stack.Pop());
            else
                stack.Push(long.Parse(token));
        }
        return stack.Pop();
    }

    private IEnumerable<string> ToPostfix(string[] tokens, Func<string, int> operationPriority)
    {
        var stack = new Stack<string>();
        foreach (var token in tokens)
        {
            if (token == "(") stack.Push(token);
            else if (token is "*" or "+")
            {
                while (stack.Any() && operationPriority(stack.Peek()) <= operationPriority(token))
                    yield return stack.Pop();
                stack.Push(token);
            }
            else if (token == ")")
            {
                while (stack.Peek() != "(")
                    yield return stack.Pop();
                stack.Pop();
            }
            else yield return token;
        }
        while (stack.Any())
            yield return stack.Pop();
    }

    // Alternative solution with regular expressions

    private long EvalWithRegex(string expr, params string[] operationsRegexes)
    {
        var regexes = operationsRegexes.Select(op => new Regex($@"(\d+|\(\d+\)) ({op}) (\d+|\(\d+\))")).ToList();
        while (true)
        {
            var replaced = regexes.Select(re => re.Replace(expr, EvalMatch, 1)).FirstOrDefault(res => res != expr);
            if (replaced == null) return long.Parse(expr);
            expr = replaced;
        }
    }

    private string EvalMatch(Match match)
    {
        var a = long.Parse(match.Groups[1].Value.Trim('(', ')'));
        var b = long.Parse(match.Groups[3].Value.Trim('(', ')'));
        return match.Groups[2].Value switch
        {
            "*" => $"{a * b}",
            "+" => $"{a + b}",
            var x => throw new Exception(x)
        };
    }
}