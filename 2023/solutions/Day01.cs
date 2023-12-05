// https://adventofcode.com/2023/day/1
// Part1
// Из каждой строки извлечь все цифры, из первой и последней составить двузначное число, сложить все такие числа.
// Part2
// Теперь подстроки one, two, ... тоже считаются цифрами и могут накладываться друг на друга: eightwo → 82.

public class Day01
{
    public void Solve(string[] lines)
    {
        List<int> ExtractDigits(string s)
        {
            var res = new List<int>();
            var ds = new[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            for (var i = 0; i < s.Length; i++)
            for (var j = 1; j < ds.Length; j++)
                if (s[i..].StartsWith(ds[j], StringComparison.Ordinal))
                {
                    res.Add(j);
                    break;
                }
                else if (char.IsDigit(s[i]))
                {
                    res.Add(s[i] - '0');
                    break;
                }

            return res;
        }

        lines.Select(s => s.Where(char.IsDigit).Select(c => c - '0').ToList())
            .Sum(s => s[0] * 10 + s.Last())
            .Part1();
        lines.Select(ExtractDigits)
            .Select(s => s[0] * 10 + s.Last())
            .Sum()
            .Part2();
    }
}