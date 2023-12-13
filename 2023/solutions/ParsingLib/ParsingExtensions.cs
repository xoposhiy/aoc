using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace

public static class ParsingExtensions
{
    public static void InvokeWithParsedArgs(this MethodInfo method, object instance, string[] lines)
    {
        var separators = method.GetCustomAttribute<SeparatorsAttribute>()?.FieldBound ?? " \t,;:|";
        var args = lines.ParseArguments(method.GetParameters(), new ParseSettings(true, separators));
        method.Invoke(instance, args);
    }

    public static object[] ParseArguments(this string[] lines, ParameterInfo[] parameters, ParseSettings settings)
    {
        var reader = new MultilineReader(lines);
        var parser = new MultilineParser();
        var args = new List<object>();
        foreach (var parameter in parameters)
        {
            if (parameter.GetCustomAttribute<ParamArrayAttribute>() == null)
            {
                var paramSettings = settings.UpdateFrom(parameter.ParameterType).UpdateFrom(parameter);
                var arg = parameter.Name!.EndsWith("map", StringComparison.OrdinalIgnoreCase) 
                    ? parser.ParseMap(parameter.ParameterType, reader)
                    : parser.Parse(parameter.ParameterType, reader, paramSettings);
                args.Add(arg);
                if (!reader.IsEndOfLines && reader.CurrentLine == string.Empty)
                    reader.MoveToNextLine(); // skip empty line if any before next block
            }
            else
            {
                var paramsArray = new List<object>();
                var elementType = parameter.ParameterType.GetElementType()!;
                var paramSettings = settings.UpdateFrom(parameter).UpdateFrom(elementType);
                while (!reader.IsEndOfLines)
                {
                    var isMap = parameter.Name!.EndsWith("maps", StringComparison.OrdinalIgnoreCase);
                    var paramsItem = isMap 
                        ? parser.ParseMap(elementType, reader)
                        : parser.Parse(elementType, reader, paramSettings);
                    paramsArray.Add(paramsItem);
                }
                args.Add(paramsArray.ToArray(elementType));
            }
        }
        return args.ToArray();
    }

    // // // private static object[] ParseArgumentsWithTemplate(ParameterInfo[] parameters, string template, string[] parts,
    // // //     ref int startIndex, ParseSettings settings)
    // // // {
    // // //     var args = new List<object>();
    // // //     var regExes = CreateRegularExpressions(parameters, template);
    // // //     var groupsByName = MatchParts(regExes, parts, ref startIndex);
    // // //     foreach (var param in parameters)
    // // //     {
    // // //         var value = groupsByName[param.Name!];
    // // //         if (string.IsNullOrEmpty(value) && param.HasDefaultValue)
    // // //             args.Add(param.DefaultValue!);
    // // //         else
    // // //             args.Add(value.ParseLine(param.ParameterType, settings.UpdateFrom(param)));
    // // //     }
    // // //
    // // //     return args.ToArray();
    // // // }
    // //
    // // private static Dictionary<string, string> MatchParts(List<Regex> regExes, string[] parts, ref int startIndex)
    // // {
    // //     var matchesByName = new Dictionary<string, string>();
    // //     foreach (var regex in regExes)
    // //     {
    // //         var m = regex.Match(parts[startIndex++]);
    // //         //Line Card   1: 30 48 49 69  1 86 94 68 12 85 | 86 57 89  8 81 85 82 68  1 22 90  2 74 12 30 45 69 92 62  4 94 48 47 64 49 does not match template Game (?<id>-?\d+): (?<my>.+) \| (?<win>.+)
    // //         if (!m.Success)
    // //             throw new FormatException(
    // //                 $"Line {parts.StrJoin(" ")} does not match template {regex} starting from {startIndex - 1}");
    // //         foreach (var name in regex.GetGroupNames())
    // //             matchesByName[name] = m.Groups[name].Value;
    // //     }
    // //
    // //     return matchesByName;
    // // }
    //
    // private static List<Regex> CreateRegularExpressions(ParameterInfo[] parameters, string template)
    // {
    //     foreach (var p in parameters)
    //     {
    //         var typeRe = ".+";
    //         if (p.ParameterType == typeof(int) || p.ParameterType == typeof(long))
    //             typeRe = @"-?\d+";
    //         else if (p.ParameterType == typeof(char))
    //             typeRe = ".";
    //         template = template.Replace($"@{p.Name}", $"(?<{p.Name}>{typeRe})");
    //     }
    //
    //     return template.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
    //         .Select(line => new Regex(line))
    //         .ToList();
    // }
}

public record ParseSettings(bool IsMultiline, string FieldSeparators = ",; \t", string ArrayBound = "")
{
    public ParseSettings SingleLine()
    {
        return this with { IsMultiline = false };
    }
    public ParseSettings UpdateFrom(ParameterInfo parameterInfo)
    {
        var isMulti = parameterInfo.GetCustomAttribute<MultilineAttribute>() != null || IsMultiline;
        if (parameterInfo.GetCustomAttribute<Oneline>() != null)
            isMulti = false;
        var sep = parameterInfo.GetCustomAttribute<SeparatorsAttribute>()?.FieldBound ?? FieldSeparators;
        var bound = parameterInfo.GetCustomAttribute<SeparatorsAttribute>()?.ArrayBound ?? ArrayBound;
        return new ParseSettings(isMulti, sep, bound);
    }
    public ParseSettings UpdateFrom(Type type)
    {
        var isMulti = type.GetCustomAttribute<MultilineAttribute>() != null || IsMultiline;
        if (type.GetCustomAttribute<Oneline>() != null)
            isMulti = false;
        var sep = type.GetCustomAttribute<SeparatorsAttribute>()?.FieldBound ?? FieldSeparators;
        var bound = type.GetCustomAttribute<SeparatorsAttribute>()?.ArrayBound ?? ArrayBound;
        return new ParseSettings(isMulti, sep, bound);
    }
}

public class TemplateAttribute : Attribute
{
    public TemplateAttribute(string template)
    {
        Template = template;
    }

    public TemplateAttribute(string template,
        params Expression<Func<string, (string separator, string template)>>[] arrayTemplates)
    {
        Template = template;
    }

    public string Template { get; }
}

public class SeparatorsAttribute : Attribute
{
    public SeparatorsAttribute(string? fieldBound = null, string? arrayBound = null)
    {
        FieldBound = fieldBound;
        ArrayBound = arrayBound;
    }

    public string? FieldBound { get; }
    public string? ArrayBound { get; }
}