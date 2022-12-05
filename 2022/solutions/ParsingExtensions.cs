using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace

public class SeparatorAttribute : Attribute
{
    public string SeparatorChars { get; }

    public SeparatorAttribute(string separatorChars)
    {
        SeparatorChars = separatorChars;
    }
}
public static class ParsingExtensions
{
    public static void InvokeWithParsedArgs(this MethodInfo method, object instance, string inputFilename)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 1)
        {
            var param = parameters[0];
            var paramType = param.ParameterType;
            if (paramType == typeof(string))
                method.Invoke(instance, new object []{File.ReadAllText(inputFilename)});
            else if (paramType == typeof(string[]))
                method.Invoke(instance, new object[] { File.ReadAllLines(inputFilename) });
            else
            {
                var lines = File.ReadAllLines(inputFilename);
                var paramLines = CreateParamLines(param, lines);
                method.Invoke(instance, new[] { paramLines });
            }
        }
        else
        {
            var lines = File.ReadAllLines(inputFilename);
            var args = lines.SplitBy(string.IsNullOrEmpty)
                .Select((block, i) => CreateParamLines(parameters[i], block))
                .ToArray();
            method.Invoke(instance, args);
        }
    }

    private static object CreateParamLines(ParameterInfo param, string[] lines)
    {
        var paramType = param.ParameterType;
        var fieldSeparators = param.GetCustomAttribute<SeparatorAttribute>()?.SeparatorChars ?? ",; \t";
        if (paramType.IsArray)
        {
            var lineType = paramType.GetElementType()!;
            var paramLines = Array.CreateInstance(lineType, lines.Length);
            var parsedLines = lines.Select(line => line.ParseLine(lineType, fieldSeparators.ToCharArray()));
            var i = 0;
            foreach (var parsedLine in parsedLines)
                paramLines.SetValue(parsedLine, i++);
            return paramLines;
        }
        else
        {
            throw new NotSupportedException(paramType.ToString());
        }
    }


    public static T[] ParseLines<T>(this string[] lines, params char[] fieldSeparators)
    {
        var seps = fieldSeparators.Any() ? fieldSeparators : new[] { ' ', '\t', ',' };
        return lines
            .Select(line => line.Split(seps, StringSplitOptions.RemoveEmptyEntries))
            .Select(Parse<T>)
            .ToArray();
    }

    public static TLine[][] ParseBlocks<TLine>(this string[] lines, char[]? fieldSeparators = null)
    {
        var seps = fieldSeparators ?? new[] { ' ', '\t', ',' };
        var block = new List<TLine>();
        var blocks = new List<TLine[]>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                blocks.Add(block.ToArray());
                block = new List<TLine>();
            }
            else
            {
                block.Add(line.ParseLine<TLine>(seps));
            }
        }
        if (block.Any())
            blocks.Add(block.ToArray());
        return blocks.ToArray();
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

    public static object ParseLine(this string line, Type resultType, params char[] fieldSeparators)
    {
        if (resultType == typeof(string))
            return line;
        var ps = line.Split(fieldSeparators, StringSplitOptions.RemoveEmptyEntries);
        return Parse(ps, resultType);
    }

    public static T Parse<T>(this string[] ps)
    {
        int start = 0;
        return (T)Parse(ps, typeof(T), ref start);
    }

    private static object Parse(this string[] ps, Type type)
    {
        int start = 0;
        return Parse(ps, type, ref start);

    }

    private static object Parse(this string[] ps, Type type, ref int startIndex)
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
            return ps[startIndex++][0];
        if (type.IsArray)
        {
            Array array = Array.CreateInstance(type.GetElementType()!, ps.Length - startIndex);
            int index = 0;
            while (startIndex < ps.Length)
            {
                var value = Parse(ps, type.GetElementType()!, ref startIndex);
                array.SetValue(value, index++);
            }
            return array;
        }
        var ctor = type.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var args = new List<object>();
        foreach (var param in ctor.GetParameters())
        {
            args.Add(Parse(ps, param.ParameterType, ref startIndex));
        }
        return ctor.Invoke(args.ToArray());
    }
}