public class Day10
{
    // Дана программа из команд двух видов:
    //  noop - ничего не делает и длится 1 цикл
    //  addx v - длится 2 цикла, после чего прибавляет v к регистру x

    // Part1:
    // Посчитать сумму произведений номера цикла на значение регистра x в конце этого цикла
    // для циклов с номерами 20, 60, 100, 140, 180 и 220, считая, что циклы нумеруются с 1.

    // Part2:
    // Программа пробегает по экрану 40x6 пикселей, слева на право, сверху вниз, по одному пикселю за цикл.
    // Очередной пиксель нужно рисовать, если значение регистра x
    // отличается от X-позиции текущего пикселя не более чем на 1.
    
    public void Solve(string[][] commands)
    {
        IEnumerable<(int x, int cycle)> Run()
        {
            var cycle = 0;
            var x = 1;
            foreach (var command in commands)
            {
                yield return (x, cycle++);
                if (command is ["addx", var v])
                {
                    yield return (x, cycle++);
                    x += v.ToInt();
                }
            }
        }

        var sum = Run().EveryNth(40, startFromIndex:19)
            .Sum(turn => turn.x * (turn.cycle+1));
        Console.WriteLine($"Part1: {sum}");

        
        var screen = Run()
            .GroupBy(40)
            .Select(g => g.StrJoin("", turn => Math.Abs(turn.x - turn.cycle%40) <= 1 ? "##" : "  "))
            .ToArray();
        Console.WriteLine($"Part2:\n{screen.StrJoin("\n")}");
        Console.WriteLine();

        Run()
            .Where(turn => Math.Abs(turn.x - turn.cycle % 40) <= 1)
            .Select(turn => new V(turn.cycle % 40, turn.cycle / 40))
            .CreateMap("##", "  ")
            .Out();

    }
}