public record Move(string MoveToken, int Count, string FromToken, int From, string ToToken, int To);

public class Day05
{
    // Дан список стеков и список перемещений элементов из одного стека в другой.
    public void Solve(string[] stacksView, Move[] moves)
    {
        var stacks = ParseStacks(stacksView);
        //stacks.Out();

        // Найти верхушки стеков после применения всех перемещений, если объекты перемещаются по одному
        ExecuteWithPart1Rules(moves, stacks);
        stacks.Select(s => s.Peek()).StrJoin().Out("Part1: ");

        // Найти верхушки стеков после применения всех перемещений, если объекты перемещаются целой стопкой
        stacks = ParseStacks(stacksView);
        ExecuteWithPart2Rules(moves, stacks);
        stacks.Select(s => s.Peek()).StrJoin().Out("Part2: ");
    }

    private static Stack<char>[] ParseStacks(string[] stacksView)
    {
        return stacksView
            .Reversed()
            .Columns()
            .EveryNth(4, startFrom:1)
            .Select(c => new Stack<char>(c.Skip(1).Except(" ")))
            .ToArray();
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