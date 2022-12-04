public record Seg(int Start, int End)
{
    public bool Contains(Seg other) => Start <= other.Start && End >= other.End;
    public bool Overlaps(Seg other) => Math.Max(Start, other.Start) <= Math.Min(End, other.End);
}