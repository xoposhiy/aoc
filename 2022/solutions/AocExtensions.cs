using TextCopy;

public static class AocExtensions
{
    public static T Part1<T>(this T answer, string comment = "")
    {
        if (!Equals(answer, 0))
            ClipboardService.SetText(answer!.ToString()!);
        comment = comment == "" ? "" : $" ({comment})";
        return answer.Out($"Part 1{comment}: ");
    }

    public static T Part2<T>(this T answer, string comment = "")
    {
        if (!Equals(answer, 0))
            ClipboardService.SetText(answer!.ToString()!);
        comment = comment == "" ? "" : $" ({comment})";
        return answer.Out($"Part 2{comment}: ");
    }
}