using System.Globalization;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

// ReSharper disable once CheckNamespace

public class TemplateAttribute : Attribute
{
    public string Template { get; }

    public TemplateAttribute(string template)
    {
        Template = template;
    }
}

public class SeparatorAttribute : Attribute
{
    public SeparatorAttribute(string separatorChars)
    {
        SeparatorChars = separatorChars;
    }

    public string SeparatorChars { get; }
}

public static class ParsingExtensions
{
    private const string DefaultFieldSeparators = ",; \t";
    private const string ExtendedFieldSeparators = "|-.";

    public static void InvokeWithParsedArgs(this MethodInfo method, object instance, string inputFilename)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 1)
        {
            var param = parameters[0];
            var paramType = param.ParameterType;
            if (paramType == typeof(string))
            {
                method.Invoke(instance, new object[] { File.ReadAllText(inputFilename) });
                return;
            }

            if (param.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                if (!param.ParameterType.IsArray)
                    throw new("Parameter 'blocks' must be an array");
                var fieldSeparators = param.GetCustomAttribute<SeparatorAttribute>()?.SeparatorChars ?? DefaultFieldSeparators;
                var blocksOfLines = File.ReadAllLines(inputFilename)
                    .SplitBy(string.IsNullOrEmpty)
                    .ToList();
                
                if (param.ParameterType.GetElementType()!.IsArray) // Solve(BlockLine[][] blocks)
                {
                    var lineType = param.ParameterType.GetElementType()!.GetElementType()!;
                    var blocks = blocksOfLines
                        .Select(block => CreateArrayFrom(block, lineType, fieldSeparators))
                        .ToList();
                    var blocksArray = Array.CreateInstance(param.ParameterType.GetElementType()!, blocks.Count);
                    for (var i = 0; i < blocks.Count; i++)
                        blocksArray.SetValue(blocks[i], i);
                    method.Invoke(instance, new object[] { blocksArray });
                    return;
                }
                else // Solve(MySuperType[] blocks)
                {
                    var blocks = blocksOfLines
                        .Select(block => block.ParseObject(param.ParameterType.GetElementType()!))
                        .ToList();
                    var blocksArray = Array.CreateInstance(param.ParameterType.GetElementType()!, blocks.Count);
                    for (var i = 0; i < blocks.Count; i++)
                        blocksArray.SetValue(blocks[i], i);
                    method.Invoke(instance, new object[] { blocksArray });
                    return;
                }
            }

            if (param.Name == "map") // Solve(T[][] map)
            {
                if (!param.ParameterType.IsArray && param.ParameterType.GetElementType()!.IsArray)
                    throw new Exception("Parameter 'map' must be an array of line-arrays");
                var cellType = param.ParameterType.GetElementType()!.GetElementType()!;
                var map = File.ReadAllLines(inputFilename)
                    .Select(line => line.Select(c => ParseMapChar(c, cellType)).ToArray(cellType))
                    .ToList();
                var mapArray = Array.CreateInstance(param.ParameterType.GetElementType()!, map.Count);
                for (var i = 0; i < map.Count; i++)
                    mapArray.SetValue(map[i], i);
                method.Invoke(instance, new object[] { mapArray });
                return;
            }
        }
        //Solve(Line1[] block1, Line2[] block2, ...)

        var lines = File.ReadAllLines(inputFilename);
        var args = lines.SplitBy(string.IsNullOrEmpty)
            .Select((block, i) => CreateParamLines(parameters[i], block))
            .ToArray();
        method.Invoke(instance, args);
    }

    public static Array ToArray<T>(this IEnumerable<T> items, Type elementType)
    {
        var itemsList = items.ToList();
        var array = Array.CreateInstance(elementType, itemsList.Count);
        var i = 0;
        foreach (var item in itemsList)
            array.SetValue(item, i++);
        return array;
    }
    

    private static object ParseMapChar(char c, Type cellType)
    {
        if (cellType == typeof(char)) return c;
        if (cellType == typeof(int)) return c - '0';
        throw new NotSupportedException($"Cannot parse map cell of type {cellType}");
    }

    private static object CreateParamLines(ParameterInfo param, string[] lines)
    {
        var fieldSeparators = param.GetCustomAttribute<SeparatorAttribute>()?.SeparatorChars ?? DefaultFieldSeparators;
        var paramType = param.ParameterType;
        if (!paramType.IsArray)
            throw new NotSupportedException(paramType.ToString());
        return CreateArrayFrom(lines, paramType.GetElementType()!, fieldSeparators);
    }

    private static object CreateArrayFrom(string[] lines, Type elementType, string fieldSeparators)
    {
        try
        {
            var paramLines = Array.CreateInstance(elementType, lines.Length);
            var parsedLines = lines.Select(line => line.ParseLine(elementType, fieldSeparators.ToCharArray()));
            var i = 0;
            foreach (var parsedLine in parsedLines)
                paramLines.SetValue(parsedLine, i++);
            return paramLines;
        }
        catch (FormatException)
        {
            if (fieldSeparators.EndsWith(ExtendedFieldSeparators))
                throw;
            return CreateArrayFrom(lines, elementType, fieldSeparators + ExtendedFieldSeparators);
        }
    }

    public static object ParseLine(this string line, Type resultType)
    {
        try
        {
            return ParseLine(line, resultType, DefaultFieldSeparators.ToCharArray());
        }
        catch
        {
            return ParseLine(line, resultType, (DefaultFieldSeparators + ExtendedFieldSeparators).ToCharArray());
        }
    }

    public static object ParseLine(this string line, Type resultType, params char[] fieldSeparators)
    {
        if (resultType == typeof(string))
            return line;
        var templateAttr = resultType.GetCustomAttribute<TemplateAttribute>();
        if (templateAttr != null)
            return new[]{line}.ParseObject(resultType);
        var ps = line.Split(fieldSeparators, StringSplitOptions.RemoveEmptyEntries);
        return Parse(ps, resultType);
    }

    private static object Parse(this string[] ps, Type type)
    {
        var start = 0;
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
        if (type.IsOneOf(typeof(JsonNode), typeof(JsonArray), typeof(JsonValue), typeof(JsonObject)))
            return JsonNode.Parse(ps[startIndex++])!;
        if (type.IsArray)
        {
            var array = Array.CreateInstance(type.GetElementType()!, ps.Length - startIndex);
            var index = 0;
            while (startIndex < ps.Length)
            {
                var value = Parse(ps, type.GetElementType()!, ref startIndex);
                array.SetValue(value, index++);
            }

            return array;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = type.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
            var elementType = type.GetGenericArguments()[0];
            var addMethod = type.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, new[] { elementType })!;
            while (startIndex < ps.Length)
            {
                var value = Parse(ps, elementType, ref startIndex);
                addMethod.Invoke(list, new[] { value });
            }
            return list;
        }

        return ParseObject(ps, type, ref startIndex);
    }

    private static object ParseObject(this string[] parts, Type objectType)
    {
        var startIndex = 0;
        return ParseObject(parts, objectType, ref startIndex);
    }

    public static object ParseObject(this string[] parts, Type objectType, ref int startIndex)
    {
        var ctor = objectType.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var args = new List<object>();
        var templateAttribute = objectType.GetCustomAttribute<TemplateAttribute>();
        if (templateAttribute == null)
        {
            foreach (var param in ctor.GetParameters())
                if (startIndex >= parts.Length && param.IsOptional)
                    args.Add(param.DefaultValue!);
                else
                    args.Add(Parse(parts, param.ParameterType, ref startIndex));
        }
        else
        {
            var parameters = ctor.GetParameters();
            var template = templateAttribute.Template;
            foreach (var p in parameters)
            {
                var typeRe = ".+";
                if (p.ParameterType == typeof(int) || p.ParameterType == typeof(long))
                    typeRe = "\\d+";
                else if (p.ParameterType == typeof(char))
                    typeRe = ".";
                template = template.Replace($"@{p.Name}", $"(?<{p.Name}>{typeRe})");
            }
            var templateLines = 
                template.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var matchesByName = new Dictionary<string, string>();
            foreach (var templateLine in templateLines)
            {
                var regex = new Regex(templateLine);
                var m = regex.Match(parts[startIndex++]);
                if (!m.Success)
                    throw new FormatException($"Line {parts.StrJoin(" ")} does not match template {template}");
                foreach (var name in regex.GetGroupNames())
                    matchesByName[name] = m.Groups[name].Value;
            }

            foreach (var param in parameters)
            {
                var value = matchesByName[param.Name!];
                if (string.IsNullOrEmpty(value) && param.HasDefaultValue)
                    args.Add(param.DefaultValue!);
                else
                    args.Add(value.ParseLine(param.ParameterType));
            }
        }
        return ctor.Invoke(args.ToArray());
    }
}