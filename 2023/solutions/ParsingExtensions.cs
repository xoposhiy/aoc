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
        var separators = method.GetCustomAttribute<SeparatorsAttribute>()?.SeparatorChars ?? " \t,;:|";
        var args = lines.ParseArguments(method.GetParameters(), new ParseSettings(separators));
        method.Invoke(instance, args);
    }

    public static object[] ParseArguments(this string[] lines, ParameterInfo[] parameters, ParseSettings settings)
    {
        if (parameters.Length == 1)
        {
            var param = parameters[0];
            if (param.GetCustomAttribute<ParamArrayAttribute>() != null)
                return new object[] { lines.ParseBlocks(param, settings) };
            return new[] { lines.ParseBlock(param, settings) };
        }

        return lines.SplitBy(string.IsNullOrEmpty)
            .Select((block, i) => block.ParseBlock(parameters[i], settings))
            .ToArray();
    }

    private static Array ParseBlocks(this string[] lines, ParameterInfo blocksParameter, ParseSettings settings)
    {
        if (!blocksParameter.ParameterType.IsArray)
            throw new Exception("Parameter 'blocks' must be an array");
        var blockType = blocksParameter.ParameterType.GetElementType()!;

        return lines.SplitBy(string.IsNullOrEmpty)
            .Select(blockLines => blockLines.ParseBlock(blockType, settings.UpdateFrom(blocksParameter)))
            .ToArray(blockType);
    }

    private static object ParseBlock(this string[] lines, ParameterInfo parameter, ParseSettings settings)
    {
        if (parameter.Name == "map") // T[][] map
            return lines.ParseMap(parameter);
        return lines.ParseBlock(parameter.ParameterType, settings.UpdateFrom(parameter));
    }

    private static Array ParseMap(this string[] lines, ParameterInfo mapParameter)
    {
        if (!mapParameter.ParameterType.IsArray && mapParameter.ParameterType.GetElementType()!.IsArray)
            throw new Exception("Parameter 'map' must be an array of arrays");
        var cellType = mapParameter.ParameterType.GetElementType()!.GetElementType()!;
        return lines
            .Select(line => line.Select(c => ParseMapChar(c, cellType)).ToArray(cellType))
            .ToArray(mapParameter.ParameterType.GetElementType()!);
    }


    private static object ParseMapChar(char c, Type cellType)
    {
        if (cellType == typeof(char)) return c;
        if (cellType == typeof(int)) return c - '0';
        throw new NotSupportedException($"Cannot parse map cell of type {cellType}");
    }


    private static object ParseBlock(this string[] lines, Type blockType, ParseSettings settings)
    {
        if (blockType == typeof(string))
            return lines.StrJoin("\n");
        if (blockType == typeof(int))
            return int.Parse(lines.Single());
        if (blockType == typeof(long))
            return long.Parse(lines.Single());
        if (blockType == typeof(JsonNode))
            return JsonNode.Parse(lines.StrJoin("\n"))!;
        if (blockType == typeof(JsonObject))
            return (JsonObject)JsonNode.Parse(lines.StrJoin("\n"))!;
        if (blockType.IsArray) // line → element
        {
            var elementType = blockType.GetElementType()!;
            return lines.Select(line => ParseLine(line, elementType, settings)).ToArray(elementType);
        }

        return ParseObject(lines, blockType, true, settings);
    }

    public static object ParseLine(this string line, Type resultType, ParseSettings settings)
    {
        try
        {
            if (resultType == typeof(string))
                return line;
            var templateAttr = resultType.GetCustomAttribute<TemplateAttribute>();
            if (templateAttr != null)
                return new[] { line }.ParseObject(resultType, true, settings);
            if (resultType.IsOneOf(typeof(JsonNode), typeof(JsonArray), typeof(JsonValue), typeof(JsonObject)))
                return JsonNode.Parse(line)!;
            var fields = line.Split(settings.FieldSeparators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return Parse(fields, resultType, false, settings);
        }
        catch (Exception e)
        {
            throw new FormatException($"Can't parse type {resultType} from line '{line}'", e);
        }
    }

    private static object Parse(this string[] ps, Type type, bool allowSplitToFields, ParseSettings settings)
    {
        var start = 0;
        return Parse(ps, type, ref start, allowSplitToFields, settings);
    }

    private static object Parse(this string[] ps, Type type, ref int startIndex, bool allowSplitToFields,
        ParseSettings settings)
    {
        var initialStartIndex = startIndex;
        try
        {
            if (type.IsArray)
                return ParseArray(type, ps, ref startIndex, allowSplitToFields, settings);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return ParseList(type, ps, ref startIndex);
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
            if (type.IsEnum)
                return Enum.Parse(type, ps[startIndex++], true);
            return ParseObject(ps, type, ref startIndex, allowSplitToFields, settings);
        }
        catch (Exception e)
        {
            throw new FormatException($"Can't parse type {type} from line '{ps[initialStartIndex..].StrJoin(" ")}'", e);
        }
    }

    private static object ParseList(Type listType, string[] ps, ref int startIndex)
    {
        var list = listType.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
        var elementType = listType.GetGenericArguments()[0];
        var addMethod = listType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, new[] { elementType })!;
        while (startIndex < ps.Length)
        {
            var value = Parse(ps, elementType, ref startIndex, false, null!);
            addMethod.Invoke(list, new[] { value });
        }

        return list;
    }

    private static object ParseArray(Type arrayType, string[] ps, ref int startIndex, bool allowSplitToFields,
        ParseSettings settings)
    {
        var items = new List<object>();
        while (startIndex < ps.Length)
        {
            var value = Parse(ps, arrayType.GetElementType()!, ref startIndex, allowSplitToFields, settings);
            items.Add(value);
        }

        return items.ToArray(arrayType.GetElementType()!);
    }

    private static object ParseObject(this string[] fields, Type objectType, bool allowSplitToFields,
        ParseSettings settings)
    {
        var start = 0;
        return ParseObject(fields, objectType, ref start, allowSplitToFields, settings);
    }

    public static object ParseObject(this string[] fields, Type objectType, ref int startIndex, bool allowSplitToFields,
        ParseSettings settings)
    {
        var ctor = objectType.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var parameters = ctor.GetParameters();
        var templateAttribute = objectType.GetCustomAttribute<TemplateAttribute>();
        if (templateAttribute == null)
        {
            var args = new List<object>();
            foreach (var param in parameters)
                if (startIndex >= fields.Length && param.IsOptional)
                {
                    args.Add(param.DefaultValue!);
                }
                else
                {
                    var newSettings = settings.UpdateFrom(param);
                    var arg = allowSplitToFields
                        ? ParseLine(fields[startIndex++], param.ParameterType, newSettings)
                        : Parse(fields, param.ParameterType, ref startIndex, false, newSettings);
                    args.Add(arg);
                }

            return ctor.Invoke(args.ToArray());
        }
        else
        {
            var args = ParseArgumentsWithTemplate(parameters, templateAttribute.Template, fields, ref startIndex,
                settings);
            return ctor.Invoke(args);
        }
    }

    private static object[] ParseArgumentsWithTemplate(ParameterInfo[] parameters, string template, string[] parts,
        ref int startIndex, ParseSettings settings)
    {
        var args = new List<object>();
        var regExes = CreateRegularExpressions(parameters, template);
        var groupsByName = MatchParts(regExes, parts, ref startIndex);
        foreach (var param in parameters)
        {
            var value = groupsByName[param.Name!];
            if (string.IsNullOrEmpty(value) && param.HasDefaultValue)
                args.Add(param.DefaultValue!);
            else
                args.Add(value.ParseLine(param.ParameterType, settings.UpdateFrom(param)));
        }

        return args.ToArray();
    }

    private static Dictionary<string, string> MatchParts(List<Regex> regExes, string[] parts, ref int startIndex)
    {
        var matchesByName = new Dictionary<string, string>();
        foreach (var regex in regExes)
        {
            var m = regex.Match(parts[startIndex++]);
            //Line Card   1: 30 48 49 69  1 86 94 68 12 85 | 86 57 89  8 81 85 82 68  1 22 90  2 74 12 30 45 69 92 62  4 94 48 47 64 49 does not match template Game (?<id>-?\d+): (?<my>.+) \| (?<win>.+)
            if (!m.Success)
                throw new FormatException(
                    $"Line {parts.StrJoin(" ")} does not match template {regex} starting from {startIndex - 1}");
            foreach (var name in regex.GetGroupNames())
                matchesByName[name] = m.Groups[name].Value;
        }

        return matchesByName;
    }

    private static List<Regex> CreateRegularExpressions(ParameterInfo[] parameters, string template)
    {
        foreach (var p in parameters)
        {
            var typeRe = ".+";
            if (p.ParameterType == typeof(int) || p.ParameterType == typeof(long))
                typeRe = @"-?\d+";
            else if (p.ParameterType == typeof(char))
                typeRe = ".";
            template = template.Replace($"@{p.Name}", $"(?<{p.Name}>{typeRe})");
        }

        return template.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => new Regex(line))
            .ToList();
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
}

public record ParseSettings(string FieldSeparators = ",; \t")
{
    public ParseSettings UpdateFrom(ParameterInfo parameterInfo)
    {
        var attr = parameterInfo.GetCustomAttribute<SeparatorsAttribute>();
        if (attr != null)
            return this with { FieldSeparators = attr.SeparatorChars };
        return this;
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
    public SeparatorsAttribute(string separatorChars)
    {
        SeparatorChars = separatorChars;
    }

    public string SeparatorChars { get; }
}