using System.Globalization;
using System.Reflection;

// ReSharper disable once CheckNamespace

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

            if (param.Name == "blocks")
            {
                if (!param.ParameterType.IsArray && param.ParameterType.GetElementType()!.IsArray)
                    throw new Exception("Parameter 'blocks' must be an array of line-arrays");
                var lineType = param.ParameterType.GetElementType()!.GetElementType()!;
                var fieldSeparators = param.GetCustomAttribute<SeparatorAttribute>()?.SeparatorChars ?? ",; \t";
                var blocks = File.ReadAllLines(inputFilename)
                    .SplitBy(string.IsNullOrEmpty)
                    .Select(block => CreateArrayFrom(block, lineType, fieldSeparators))
                    .ToList();
                var blocksArray = Array.CreateInstance(param.ParameterType.GetElementType()!, blocks.Count);
                for (var i = 0; i < blocks.Count; i++)
                    blocksArray.SetValue(blocks[i], i);
                method.Invoke(instance, new object[] { blocksArray });
                return;
            }

            if (param.Name == "map")
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
        var fieldSeparators = param.GetCustomAttribute<SeparatorAttribute>()?.SeparatorChars ?? ",; \t";
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
            return CreateArrayFrom(lines, elementType, fieldSeparators + "|-.");
        }
    }

    public static object ParseLine(this string line, Type resultType, params char[] fieldSeparators)
    {
        if (resultType == typeof(string))
            return line;
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

        var ctor = type.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var args = new List<object>();
        foreach (var param in ctor.GetParameters()) args.Add(Parse(ps, param.ParameterType, ref startIndex));
        return ctor.Invoke(args.ToArray());
    }
}