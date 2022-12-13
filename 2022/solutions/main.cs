using System.Diagnostics;
using System.Net;
using System.Reflection;

Console.WriteLine("# Advent of Code 2022");

Console.WriteLine();

await SolveDay(DateTime.Now.Day);
//await SolveDay(12);
//foreach (var dayNumber in Enumerable.Range(1, DateTime.Now.Day)) await SolveDay(dayNumber);

async Task SolveDay(int day)
{
    Console.WriteLine("## Day " + day);

    var inputFilename = $"../../../inputs/{day:D2}.txt";
    await DownloadInputIfNeeded(inputFilename, day);

    var daySolution = CreateInstanceForDay(day);
    var sw = Stopwatch.StartNew();
    var singleSolve = daySolution.GetType().GetMethod("Solve");
    if (singleSolve != null)
        singleSolve.InvokeWithParsedArgs(daySolution, inputFilename);
    else
    {
        daySolution.GetType().GetMethod("Part1")!.InvokeWithParsedArgs(daySolution, inputFilename);
        daySolution.GetType().GetMethod("Part2")!.InvokeWithParsedArgs(daySolution, inputFilename);
    }
    Console.WriteLine("Total time: " + sw.Elapsed);
    Console.WriteLine();
}

async Task DownloadInputIfNeeded(string filename, int i)
{
    // if no file filename or it has zero size
    if (!File.Exists(filename) || new FileInfo(filename).Length == 0)
    {
        File.WriteAllBytes(filename, Array.Empty<byte>());
        var inputUrl = $"https://adventofcode.com/2022/day/{i}/input";
        var httpMessageHandler = new HttpClientHandler();
        var aocSession = Environment.GetEnvironmentVariable("AOC");
        if (string.IsNullOrEmpty(aocSession))
            throw new Exception("AOC environment variable not set");

        httpMessageHandler.CookieContainer.Add(new Cookie("session", aocSession, "/", "adventofcode.com"));
        var inp = await new HttpClient(httpMessageHandler).GetByteArrayAsync(inputUrl);
        File.WriteAllBytes(filename, inp);
        Console.WriteLine($"Input downloaded to {Path.GetFullPath(filename)}");
    }
}

object CreateInstanceForDay(int dayNumber)
{
    var assembly = Assembly.GetExecutingAssembly();
    var dayType = assembly.GetType("Day" + dayNumber) ?? assembly.GetType("Day0" + dayNumber)!;
    return Activator.CreateInstance(dayType) ?? throw new Exception("oops");
}
