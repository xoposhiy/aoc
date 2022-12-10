public class Day10
{
    // Дана программа из команд двух видов:
    // * noop - ничего не делает и длится 1 цикл;
    // * addx V - длится 2 цикла, после чего прибавляет V к регистру X.

    // Part 1:
    // Для циклов с номерами 20, 60, 100, 140, 180 и 220 (нумерация с 1)
    // нужно посчитать сумму произведений номера цикла на X в конце этого цикла.
    
    public record Command(string OpName, int Arg = 0);
    public record VmState(int X, int Cycle);

    IEnumerable<VmState> RunVm(Command[] commands)
    {
        var cycle = 0;
        var x = 1;
        foreach (var command in commands)
            switch (command)
            {
                case ("addx", var v):
                    yield return new(x, cycle++);
                    yield return new(x, cycle++);
                    x += v;
                    break;
                case ("noop", _):
                    yield return new(x, cycle++);
                    break;
                default:
                    throw new NotSupportedException(command.ToString());
            }
    }
    
    public void Solve(Command[] commands)
    {
        RunVm(commands)
            .EveryNth(40, startFromIndex:19)
            .Sum(vm => vm.X * (vm.Cycle+1))
            .Out("Part 1: ");
        
        // Part 2:
        // Программу управляет лучом, который пробегает по экрану 40x6 пикселей,
        // построчно слева на право, сверху вниз, по одному пикселю за цикл.
        // Очередной пиксель нужно рисовать, только если значение регистра x на этом цикле
        // отличается от X-позиции луча не более чем на 1.
        

        RunVm(commands)
            .GroupBy(40)
            .Select(screenRow =>
                screenRow.StrJoin("", vm => Math.Abs(vm.X - vm.Cycle % 40) <= 1 ? "##" : "  "))
            .Out("Part 2:\n");

        RunVm(commands)
            .Where(vm => Math.Abs(vm.X - vm.Cycle % 40) <= 1)
            .Select(vm => new V(vm.Cycle % 40, vm.Cycle / 40))
            .CreateMap("##", "  ")
            .Out("\nPart 2 (version 2):\n");
    }
}

// Полное решение ищите тут: https://github.com/xoposhiy/aoc

