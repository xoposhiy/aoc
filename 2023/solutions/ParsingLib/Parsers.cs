using System.Globalization;

public class Parsers
{
    public static object? TryParseToken(Type resultType, Func<string> readToken)
    {
        if (resultType == typeof(int))
            return int.Parse(readToken(), CultureInfo.InvariantCulture);
        if (resultType == typeof(long))
            return long.Parse(readToken(), CultureInfo.InvariantCulture);
        if (resultType == typeof(double))
            return double.Parse(readToken(), CultureInfo.InvariantCulture);
        if (resultType == typeof(string))
            return readToken();
        if (resultType == typeof(char))
            return readToken()[0];
        if (resultType.IsEnum)
            return Enum.Parse(resultType, readToken(), true);
        return null;
    }
}