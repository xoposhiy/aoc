using System.Text.Json.Nodes;
using Shouldly;

public class Day13
{
    /*
    Даны пары скобочных выражений, типа
    [[5,[],4],[[[8],2,5,9],[[0,6],9,8],[[0,7,9],7,[10],5,[5,6,10]],[0]],[1]]
    [[[[2,9,10,0,4],2,10],4]]

    Пары можно сравнивать поэлементно до первого неравенства. Если левый список кончился раньше, то левое выражение меньше.
    Если нужно сравнить скаляр со списком, то скаляр преобразуется в одноэлементный список.
     */

    public void Solve(params (JsonNode a, JsonNode b)[] pairs)
    {
        /*
        Part 1
        Найти и сложить индексы (с единицы!) всех пар, в которых первое выражение не больше второго.
         */
        pairs.Indices()
            .Where(i => Compare(pairs[i].a, pairs[i].b) <= 0)
            .Sum(i => i + 1)
            .Out("Part1: ").ShouldBe(5013);
        var divider1 = JsonNode.Parse("[[2]]")!;
        var divider2 = JsonNode.Parse("[[6]]")!;
        
        /*
        Part 2
        Ко всем выражениям из входного файла добавить два разделителя [[2]] и [[6]], отсортировать по возрастанию, 
        и найти произведение индексов разделителей в отсортированном массиве выражений.   
         */
        var all = pairs
            .SelectMany(p => new[] { p.a, p.b })
            .Concat(new[] { divider1, divider2 })
            .ToList();
        all.Sort(Compare);
        ((all.IndexOf(divider1) + 1) * (all.IndexOf(divider2) + 1))
            .Out("Part 2: ").ShouldBe(25038);
    }

    private int Compare(JsonNode a, JsonNode b)
    {
        if (a is JsonValue && b is JsonValue)
            return a.GetValue<int>().CompareTo(b.GetValue<int>());
        if (a is JsonValue)
            a = new JsonArray(a.GetValue<int>());
        if (b is JsonValue)
            b = new JsonArray(b.GetValue<int>());
        var aa = a.AsArray();
        var bb = b.AsArray();
        var res = aa.Zip(bb, Compare!).FirstOrDefault(o => o != 0);
        return res != 0 ? res : aa.Count.CompareTo(bb.Count);
    }
}