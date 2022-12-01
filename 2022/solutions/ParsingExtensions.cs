using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace

public static class ParsingExtensions
{
    public static T[] ParseLines<T>(this string[] lines, char[]? fieldSeparators = null)
    {
        var seps = fieldSeparators ?? new[] { ' ', '\t', ',' };
        return lines
            .Select(line => line.Split(seps, StringSplitOptions.RemoveEmptyEntries))
            .Select(Parse<T>)
            .ToArray();
    }

    public static IList<IList<TLine>> ParseBlocks<TLine>(this string[] lines, char[]? fieldSeparators = null)
    {
        var seps = fieldSeparators ?? new[] { ' ', '\t', ',' };
        var block = new List<TLine>();
        var blocks = new List<IList<TLine>>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                blocks.Add(block);
                block = new List<TLine>();
            }
            else
            {
                block.Add(line.ParseLine<TLine>(seps));
            }
        }
        if (block.Any())
            blocks.Add(block);
        return blocks;
    }

    public static IList<T> ParseBlockOf<T>(this string[] lines, ref int startLine, params char[] fieldSeparators)
    {
        var block = new List<T>();
        while (startLine < lines.Length)
        {
            var line = lines[startLine++];
            block.Add(line.ParseLine<T>(fieldSeparators));
        }
        return block;
    }
    public static T ParseLine<T>(this string line, params char[] fieldSeparators)
    {
        var ps = line.Split(fieldSeparators, StringSplitOptions.RemoveEmptyEntries);
        return Parse<T>(ps);
    }

    public static T Parse<T>(this string[] ps)
    {
        int start = 0;
        return (T)Parse(typeof(T), ps, ref start);
    }

    private static object Parse(Type type, string[] ps, ref int startIndex)
    {
        if (startIndex >= ps.Length)
            throw new FormatException($"Line {ps.StrJoin(" ")} has not enough parts :(");
        if (type == typeof(int))
            return int.Parse(ps[startIndex++], CultureInfo.InvariantCulture);
        if (type == typeof(long))
            return long.Parse(ps[startIndex++], CultureInfo.InvariantCulture);
        if (type == typeof(double))
            return double.Parse(ps[startIndex++], CultureInfo.InvariantCulture);
        if (type == typeof(string))
            return ps[startIndex++];
        if (type == typeof(char))
            return ps[startIndex++];
        if (type.IsArray)
        {
            var res = new List<object>();
            while (startIndex < ps.Length)
            {
                res.Add(Parse(type.GetElementType()!, ps, ref startIndex));
                startIndex++;
            }
            return res.ToArray();
        }
        var ctor = type.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var args = new List<object>();
        foreach (var param in ctor.GetParameters())
        {
            args.Add(Parse(param.ParameterType, ps, ref startIndex));
        }
        return ctor.Invoke(args.ToArray());
    }
}