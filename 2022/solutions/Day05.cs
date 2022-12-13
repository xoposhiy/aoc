
[Template("""move @Count from @From to @To""")]
public record Move(int Count, int From, int To);

public class Day05
{
    // Дан список стеков и список перемещений элементов из одного стека в другой.
    public void Solve(string[] stacksView, Move[] moves)
    {
        var initialStacks = 
            stacksView
                .Reversed()
                .Skip(1)
                .Columns()
                .EveryNth(4, startFromIndex: 1)
                .Select(col => col.Trim())
                .ToArray();

        var stacks = initialStacks.Select(c => c.ToStack()).ToArray();
        stacks.Out();

        // Найти верхушки стеков после применения всех перемещений, если объекты перемещаются по одному
        ExecuteWithPart1Rules(moves, stacks);
        stacks.Select(s => s.Peek()).StrJoin().Out("Part1: ");

        // Найти верхушки стеков после применения всех перемещений, если объекты перемещаются целой стопкой
        stacks = initialStacks.Select(c => c.ToStack()).ToArray();
        ExecuteWithPart2Rules(moves, stacks);
        stacks.Select(s => s.Peek()).StrJoin().Out("Part2: ");
    }

    private static void ExecuteWithPart1Rules(Move[] moves, Stack<char>[] stacks)
    {
        foreach (var move in moves)
        {
            for (int i = 0; i < move.Count; i++)
            {
                var c = stacks[move.From - 1].Pop();
                stacks[move.To - 1].Push(c);
            }
        }
    }

    private static void ExecuteWithPart2Rules(Move[] moves, Stack<char>[] stacks)
    {
        foreach (var move in moves)
        {
            var tmp = new Stack<char>();
            for (int i = 0; i < move.Count; i++) 
                tmp.Push(stacks[move.From - 1].Pop());
            for (int i = 0; i < move.Count; i++)
                stacks[move.To - 1].Push(tmp.Pop());
        }
    }
}