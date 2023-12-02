using TextCopy;

public static class AocExtensions
{
    public static T Part1<T>(this T answer, string comment = "")
    {
        if (!Equals(answer, 0))
            ClipboardService.SetText(answer!.ToString()!);
        comment = comment == "" ? "" : $" ({comment})";
        answer.Out($"Part 1{comment}: ");
        CheckAnswer(answer, 1);
        return answer;
    }

    private static void CheckAnswer<T>(T answer, int partNumber)
    {
        if (answer == null) throw new ArgumentNullException(nameof(answer));
        var answerFilename = $"../../../inputs/{CurrentDay.Day:D2}.ans.txt";
        if (!File.Exists(answerFilename) || new FileInfo(answerFilename).Length == 0 || Equals(answer, 0))
        {
            File.WriteAllText(answerFilename, answer.Format());
            return;
        }
        var answers = File.ReadLines(answerFilename).ToList();
        if (answers.Count < partNumber)
        {
            answers.Add(answer.Format());
            File.WriteAllLines(answerFilename, answers);
            return;
        }

        if (answers[partNumber - 1] != answer.Format())
        {
            Console.WriteLine($"========================================================================");
            Console.WriteLine($"WRONG ANSWER! Expected: {answers[partNumber-1]} Actual: {answer.Format()}");
            Console.WriteLine($"========================================================================");
        }
    }

    public static T Part2<T>(this T answer, string comment = "")
    {
        if (!Equals(answer, 0))
            ClipboardService.SetText(answer!.ToString()!);
        comment = comment == "" ? "" : $" ({comment})";
        answer.Out($"Part 2{comment}: ");
        CheckAnswer(answer, 2);
        return answer;
    }
}