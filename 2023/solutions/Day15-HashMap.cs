// https://adventofcode.com/2023/day/15

public class Day15
{
    private static int Hash(string s) => s.Aggregate(0, (h, c) => (h + c) * 17 % 256);

    // rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
    public void Solve([Separators(",")][Oneline]string[] lines)
    {
        lines.Sum(Hash).Part1();
        
        var boxes = Range(0, 256)
            .Select(_ => new List<(string Label, int Lense)>())
            .ToArray();
        foreach (var line in lines)
        {
            var parts = line.Split('-', '=');
            var label = parts[0];
            var op = line[label.Length];
            var box = boxes[Hash(label)];
            var slotIndex = box.IndexOf(x => x.Label == label);
            if (op is '=')
            {
                var slotValue = (label, int.Parse(parts[1]));
                if (slotIndex >= 0)
                    box[slotIndex] = slotValue;
                else
                    box.Add(slotValue);
            }
            else if (op == '-')
            {
                if (slotIndex >= 0) box.RemoveAt(slotIndex);
                // else do nothing
            }
        }

        var res = 0L;
        for (int iBox = 0; iBox < boxes.Length; iBox++)
        for (int iSlot = 0; iSlot < boxes[iBox].Count; iSlot++) 
                res += (iBox + 1) * (iSlot + 1) * boxes[iBox][iSlot].Item2;
        res.Part2();
    }
}