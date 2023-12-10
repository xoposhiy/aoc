using System.Numerics;

public static class MathExtensions
{
    public static T GreatestCommonDivisor<T>(this T a, T b)
        where T : INumber<T>
    {
        var bigA = BigInteger.CreateChecked(a);
        var bigB = BigInteger.CreateChecked(b);
        var gcd = BigInteger.GreatestCommonDivisor(bigA, bigB);
        return T.CreateChecked(gcd);
    }

    public static BigInteger LeastCommonMultiple<T>(this T a, T b)
        where T : INumber<T>
    {
        var bigA = BigInteger.CreateChecked(a);
        var bigB = BigInteger.CreateChecked(b);
        var gcd = BigInteger.GreatestCommonDivisor(bigA, bigB);
        return bigA * bigB / gcd;
    }

    public static BigInteger LeastCommonMultiple<T>(this IEnumerable<T> values)
        where T : INumber<T>
    {
        var bigValues = values.Select(BigInteger.CreateChecked).ToList();
        var gcd = bigValues.Aggregate(BigInteger.GreatestCommonDivisor);
        return bigValues.Aggregate((a, b) => a * b / gcd);
    }

    public static T GreatestCommonDivisor<T>(this IEnumerable<T> values)
        where T : INumber<T>
    {
        return T.CreateChecked(
            values
                .Select(BigInteger.CreateChecked)
                .Aggregate(BigInteger.GreatestCommonDivisor));
    }


    public static T ModPositive<T>(this T dividend, T divisor)
        where T : IAdditionOperators<T, T, T>, IModulusOperators<T, T, T>
    {
        return (dividend % divisor + divisor) % divisor;
    }
}