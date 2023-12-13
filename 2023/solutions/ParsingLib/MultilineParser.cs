using System.Reflection;
using System.Text.Json.Nodes;

public class MultilineParser
{
    public object Parse(Type resultType, MultilineReader reader, ParseSettings settings)
    {
        try
        {
            if (!settings.IsMultiline)
            {
                var result = new LineParser().Parse(resultType, reader.GetLineReader(), settings);
                reader.MoveToNextLine();
                return result;
            }
            // TODO
            // var templateAttr = resultType.GetCustomAttribute<TemplateAttribute>();
            // if (templateAttr != null)
            //     return new[] { line }.ParseObject(resultType, true, settings);
            if (resultType.IsOneOf(typeof(JsonNode), typeof(JsonArray), typeof(JsonValue), typeof(JsonObject)))
                return JsonNode.Parse(reader.ReadUntilEmptyLine())!;
            if (resultType.IsArray)
                return ParseArray(resultType, reader, settings);
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(List<>))
                return ParseList(resultType, reader, settings);
            var value = Parsers.TryParseToken(resultType, reader.ReadLine);
            if (value != null)
                return value;
            return ParseObject(resultType, reader, settings);
        }
        catch (Exception e)
        {
            throw new FormatException($"Can't parse type {resultType}. {reader}", e);
        }
    }
    
    private object ParseList(Type listType, MultilineReader reader, ParseSettings settings)
    {
        var list = listType.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
        var elementType = listType.GetGenericArguments()[0];
        var addMethod = listType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, new[] { elementType })!;
        var itemSettings = settings.SingleLine().UpdateFrom(elementType);
        while (!reader.IsEndOfLines && reader.CurrentLine != string.Empty)
        {
            var value = Parse(elementType, reader, itemSettings);
            addMethod.Invoke(list, new[] { value });
        }
        if (!reader.IsEndOfLines)
            reader.MoveToNextLine();
        return list;
    }

    public object ParseArray(Type arrayType, MultilineReader reader, ParseSettings settings)
    {
        var items = new List<object>();
        var elementType = arrayType.GetElementType()!;
        var itemSettings = settings.SingleLine().UpdateFrom(elementType);
        while (!reader.IsEndOfLines && reader.CurrentLine != string.Empty)
        {
            var value = Parse(elementType, reader, itemSettings);
            items.Add(value);
        }
        if (!reader.IsEndOfLines)
            reader.MoveToNextLine();
        return items.ToArray(elementType!);
    }

    private object ParseObject(Type resultType, MultilineReader reader, ParseSettings settings)
    {
        //TODO templates
        var ctor = resultType.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var parameters = ctor.GetParameters();
        var args = new List<object>();
        foreach (var param in parameters)
            if (reader.IsEndOfLines && param.IsOptional)
            {
                args.Add(param.DefaultValue!);
            }
            else
            {
                var propSettings = settings.SingleLine().UpdateFrom(param.ParameterType).UpdateFrom(param);
                var arg = Parse(param.ParameterType, reader, propSettings);
                args.Add(arg);
            }
        if (!reader.IsEndOfLines && reader.CurrentLine == string.Empty)
            reader.MoveToNextLine(); // skip empty line;

        return ctor.Invoke(args.ToArray());
    }

    public object ParseMap(Type mapType, MultilineReader reader)
    {
        if (!mapType.IsArray && mapType.GetElementType()!.IsArray)
            throw new Exception("Parameter 'map' must be an array of arrays");
        var cellType = mapType.GetElementType()!.GetElementType()!;
        var res = new List<object>();
        while (!reader.IsEndOfLines && reader.CurrentLine != string.Empty)
        {
            var line = reader.ReadLine();
            res.Add(line.Select(c => ParseMapChar(c, cellType)).ToArray(cellType));
        }
        if (!reader.IsEndOfLines && reader.CurrentLine == string.Empty)
            reader.MoveToNextLine(); // skip empty line;

        return res.ToArray(mapType.GetElementType()!);
    }
    
    private static object ParseMapChar(char c, Type cellType)
    {
        if (cellType == typeof(char)) return c;
        if (cellType == typeof(int)) return c - '0';
        throw new NotSupportedException($"Cannot parse map cell of type {cellType}");
    }

}