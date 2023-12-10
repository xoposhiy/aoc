public static class BitsExtensions
{
    public static bool HasBit(this long v, int bitIndex)
    {
        return (v & (1L << bitIndex)) != 0;
    }

    public static bool HasBit(this int v, int bitIndex)
    {
        return (v & (1 << bitIndex)) != 0;
    }

    public static long SetBit(this long v, int bitIndex)
    {
        return v | (1L << bitIndex);
    }

    public static int SetBit(this int v, int bitIndex)
    {
        return v | (1 << bitIndex);
    }

    public static long UnsetBit(this long v, int bitIndex)
    {
        return v & ~(1L << bitIndex);
    }

    public static int UnsetBit(this int v, int bitIndex)
    {
        return v & ~(1 << bitIndex);
    }

    public static bool HasBit(this int v, char letter)
    {
        return v.HasBit(char.ToLower(letter) - 'a');
    }

    public static int SetBit(this int v, char letter)
    {
        return v.SetBit(char.ToLower(letter) - 'a');
    }

    public static int UnsetBit(this int v, char letter)
    {
        return v.UnsetBit(char.ToLower(letter) - 'a');
    }
}