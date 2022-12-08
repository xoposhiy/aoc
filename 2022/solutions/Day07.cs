/*
# [Day 7](https://adventofcode.com/2022/day/7)

Дан список команд cd / ls в командной строке, который показывает все директории и размеры файлов:

$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
...


## Part1

Найти суммарный размер всех директорий, с totalSize не превосходящим 100000.

## Part2

Найти самую маленькую директорию, после освобождения которой останется как минимум 30000000 свободного места (из 70000000 общего)
*/
public class Day07
{
    public void Solve(string[][] lines)
    {
        IEnumerable<long> DirSizes()
        {
            var stack = new Stack<long>();
            foreach (var line in lines)
            {
                if (line[0] == "$" && line[1] == "cd")
                {
                    if (line[2] == "..")
                    {
                        yield return stack.Peek();
                        stack.Push(stack.Pop() + stack.Pop());
                    }
                    else
                        stack.Push(0);
                }
                else if (long.TryParse(line[0], out var fileSize))
                    stack.Push(stack.Pop() + fileSize);
            }

            while (stack.Count > 1)
            {
                stack.Push(stack.Pop() + stack.Pop());
                yield return stack.Peek();
            }
        }

        var dirSizes = DirSizes().ToList();
        dirSizes.Where(size => size <= 100000).Sum()
            .Out("Part1: ");

        var currentFreeSpace = 70000000 - 30000000;
        var needFreeSpace = dirSizes.Last() - currentFreeSpace;
        dirSizes.Where(size => size >= needFreeSpace).Min()
            .Out("Part2: ");
    }

    record Dir
    {
        public readonly Dictionary<string, Dir> Dirs = new();
        public long TotalSize { get; set; }
        public IEnumerable<Dir> AllDirs => Dirs.Values.SelectMany(d => d.AllDirs).Prepend(this);
    }

    public void SolveWithTree(string[][] lines)
    {
        var root = new Dir();
        var stack = new Stack<Dir>();
        stack.Push(root);
        foreach (var line in lines.Skip(1))
        {
            if (line[0] == "$" && line[1] == "cd")
            {
                var dirName = line[2];
                if (dirName == "..")
                {
                    stack.Peek().TotalSize += stack.Peek().Dirs.Values.Sum(d => d.TotalSize);
                    stack.Pop();
                }
                else
                    stack.Push(stack.Peek().Dirs.GetValueOrCreate(dirName, _ => new Dir()));
            }
            else if (long.TryParse(line[0], out var fileSize)) 
                stack.Peek().TotalSize += fileSize;
        }

        while (stack.Count > 0)
        {
            stack.Peek().TotalSize += stack.Peek().Dirs.Values.Sum(d => d.TotalSize);
            stack.Pop();
        }

        root.AllDirs.Where(d => d.TotalSize <= 100000).Sum(d => d.TotalSize)
            .Out("Part1: ");

        var currentFreeSpace = 70000000 - 30000000;
        var needFreeSpace = root.TotalSize - currentFreeSpace;
        root.AllDirs.Where(d => d.TotalSize >= needFreeSpace).Min(d => d.TotalSize)
            .Out("Part2: ");
    }
}