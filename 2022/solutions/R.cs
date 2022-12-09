﻿public record R(int Start, int End)
{
    public bool Contains(R other) => Start <= other.Start && End >= other.End;
    public bool Overlaps(R other) => Math.Max(Start, other.Start) <= Math.Min(End, other.End);
}