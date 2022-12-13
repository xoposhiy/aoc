record ExprBracket(List<ExprBracket> Items, int? Value = null)
{
    public override string ToString() => Value.HasValue ? Value.ToString()! : $"[{string.Join(",", Items)}]";
}

public class Day13
{
    /*
    Даны пары скобочных выражений, типа
    [[5,[],4],[[[8],2,5,9],[[0,6],9,8],[[0,7,9],7,[10],5,[5,6,10]],[0]],[1]]
    [[[[2,9,10,0,4],2,10],4]]

    Пары можно сравнивать поэлементно до первого неравенства. Если левый список кончился раньше, то левое выражение меньше.
    Если нужно сравнить скаляр со списком, то скаляр преобразуется в одноэлементный список.

    Part1
    Найти и сложить индексы (с единицы!) все пары, в которых первое выражение не больше второго.

    Part2
    Ко всем выражениям из входного файла добавить два разделителя [[2]] и [[6]], отсортировать по возрастанию, 
    и найти произведение индексов разделителей в отсортированном массиве выражений.   
     */

    public void Solve(params (string a, string b)[] pairs)
    {
        var rightIndicesSum = 0;
        var divider1 = ParseSquareBracketsExpr("[[2]]");
        var divider2 = ParseSquareBracketsExpr("[[6]]");
        var all = new List<ExprBracket>() { divider1, divider2 };
        for (int i = 0; i < pairs.Length; i++)
        {
            var pair = pairs[i];
            var a = ParseSquareBracketsExpr(pair.a);
            var b = ParseSquareBracketsExpr(pair.b);
            all.Add(a);
            all.Add(b);
            if (Compare(a, b) <= 0)
                rightIndicesSum += i+1;
        }
        rightIndicesSum.Out("Part 1: ");

        all.Sort(Compare);
        var index1 = (all.IndexOf(divider1) + 1);
        var index2 = all.IndexOf(divider2) + 1;
        (index1 * index2).Out("Part 2: ");
    }

    private int Compare(ExprBracket a, ExprBracket b)
    {
        if (a.Value.HasValue && b.Value.HasValue)
            return a.Value.Value.CompareTo(b.Value.Value);
        if (a.Value.HasValue)
            return Compare(new(new() { a }), b);
        if (b.Value.HasValue)
            return Compare(a, new(new() { b }));
        var res = a.Items.Zip(b.Items, Compare).FirstOrDefault(o => o != 0);
        return res != 0 ? res : a.Items.Count.CompareTo(b.Items.Count);
    }

    private ExprBracket ParseSquareBracketsExpr(string expr)
    {
        var i = 0;
        return ParseSquareBracketsExpr(expr, ref i);
    }

    // Partially credits to copilot
    private ExprBracket ParseSquareBracketsExpr(string expr, ref int startIndex)
    {
        if (expr[startIndex] != '[')
            return new(new(), ParseInt(expr, ref startIndex));
        startIndex++;
        var items = new List<ExprBracket>();
        while (startIndex < expr.Length)
        {
            var c = expr[startIndex];
            switch (c)
            {
                case '[':
                    items.Add(ParseSquareBracketsExpr(expr, ref startIndex));
                    break;
                case ']':
                    startIndex++;
                    return new(items);
                case ',':
                    startIndex++;
                    break;
                default:
                    items.Add(new(new(), ParseInt(expr, ref startIndex)));
                    break;
            }
        }
        throw new("Invalid expression " + expr);

    }

    // Credits to copilot
    private int ParseInt(string expr, ref int startIndex)
    {
        var value = 0;
        while (startIndex < expr.Length)
        {
            var c = expr[startIndex];
            if (c is >= '0' and <= '9')
            {
                value = value * 10 + (c - '0');
                startIndex++;
            }
            else
                break;
        }
        return value;
    }
}