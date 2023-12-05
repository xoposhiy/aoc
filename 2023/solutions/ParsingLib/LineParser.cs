using System.Reflection;
using System.Text.Json.Nodes;

public class MultilineAttribute : Attribute
{
}
public class Oneline : Attribute
{
}

public class LineParser
{
    public object Parse(Type resultType, LineReader reader, ParseSettings settings)
    {
        try
        {
            // TODO
            // var templateAttr = resultType.GetCustomAttribute<TemplateAttribute>();
            // if (templateAttr != null)
            //     return new[] { line }.ParseObject(resultType, true, settings);
            if (resultType.IsOneOf(typeof(JsonNode), typeof(JsonArray), typeof(JsonValue), typeof(JsonObject)))
                return JsonNode.Parse(reader.Line)!;
            if (resultType.IsArray)
                return ParseArray(resultType, reader, settings);
            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(List<>))
                return ParseList(resultType, reader, settings);
            var value = Parsers.TryParseToken(resultType, () => reader.ReadLineUntil(settings.FieldSeparators));
            if (value != null)
                return value;
            return ParseObject(resultType, reader, settings);
        }
        catch (Exception e)
        {
            throw new FormatException($"Can't parse type {resultType}. {reader}", e);
        }
    }

    private object ParseList(Type listType, LineReader reader, ParseSettings settings)
    {
        var list = listType.GetConstructor(Type.EmptyTypes)!.Invoke(Array.Empty<object>());
        var elementType = listType.GetGenericArguments()[0];
        var addMethod = listType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, new[] { elementType })!;
        while (!reader.IsEndOfLine && !settings.ArrayBound.Contains(reader.CurrentChar))
        {
            var value = Parse(elementType, reader, settings);
            addMethod.Invoke(list, new[] { value });
        }
        if (!reader.IsEndOfLine)
        {
            reader.MoveToNextChar();
            reader.SkipSeparators(settings.FieldSeparators);
        }

        return list;
    }

    private object ParseArray(Type arrayType, LineReader reader, ParseSettings settings)
    {
        var items = new List<object>();
        while (!reader.IsEndOfLine && !settings.ArrayBound.Contains(reader.CurrentChar))
        {
            var value = Parse(arrayType.GetElementType()!, reader, settings);
            items.Add(value);
        }

        if (!reader.IsEndOfLine)
        {
            reader.MoveToNextChar();
            reader.SkipSeparators(settings.FieldSeparators);
        }
        return items.ToArray(arrayType.GetElementType()!);
    }

    private object ParseObject(Type resultType, LineReader reader, ParseSettings settings)
    {
        //TODO templates
        var ctor = resultType.GetConstructors().MaxBy(c => c.GetParameters().Length)!;
        var parameters = ctor.GetParameters();
        var args = new List<object>();
        foreach (var param in parameters)
            if (reader.IsEndOfLine && param.IsOptional)
            {
                args.Add(param.DefaultValue!);
            }
            else
            {
                var newSettings = settings.UpdateFrom(param);
                var arg = Parse(param.ParameterType, reader, newSettings);
                args.Add(arg);
            }

        return ctor.Invoke(args.ToArray());
    }
}