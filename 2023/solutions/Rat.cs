public readonly record struct Rat
{
    public Rat(long numerator, long denominator)
    {
        var gcd = Gcd(numerator, denominator);
        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
        if (Denominator >= 0) return;
        Numerator = -Numerator;
        Denominator = -Denominator;
    }

    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    public static Rat operator +(Rat a, Rat b)
    {
        var gcd = Gcd(a.Denominator, b.Denominator);
        var lcm = a.Denominator / gcd * b.Denominator;
        return new Rat(a.Numerator * (lcm / a.Denominator) + b.Numerator * (lcm / b.Denominator), lcm);
    }

    public static Rat operator -(Rat a, Rat b)
    {
        var gcd = Gcd(a.Denominator, b.Denominator);
        var lcm = a.Denominator / gcd * b.Denominator;
        return new Rat(a.Numerator * (lcm / a.Denominator) - b.Numerator * (lcm / b.Denominator), lcm);
    }

    public static Rat operator *(Rat a, Rat b) => new(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
    public static Rat operator /(Rat a, Rat b) => new(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
    public static implicit operator Rat(long n) => new(n, 1);
    public static implicit operator Rat((long n, long d) t) => new(t.n, t.d);
    public static implicit operator (long n, long d)(Rat r) => (r.Numerator, r.Denominator);
    public long Numerator { get; init; }
    public long Denominator { get; init; }
    public override string ToString() => $"{Numerator}/{Denominator}";

    public void Deconstruct(out long N, out long D)
    {
        N = this.Numerator;
        D = this.Denominator;
    }
}