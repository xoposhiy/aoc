using System.Diagnostics;
using System.Net;
using System.Reflection;

Console.WriteLine("# Advent of Code 2022");
Console.WriteLine();

await SolveDay(DateTime.Now.Day);
//foreach (var dayNumber in Enumerable.Range(1, DateTime.Now.Day)) await SolveDay(dayNumber);

async Task SolveDay(int n)
{
    Console.WriteLine("## Day " + n);

    var inputFilename = $"../../../inputs/{n:D2}.txt";
    if (!File.Exists(inputFilename))
    {
        var inputUrl = $"https://adventofcode.com/2022/day/{n}/input";
        var httpMessageHandler = new HttpClientHandler();
        var aocSession = Environment.GetEnvironmentVariable("AOC");
        if (string.IsNullOrEmpty(aocSession))
        {
            Console.WriteLine("AOC environment variable not set");
            return;
        }

        httpMessageHandler.CookieContainer.Add(new Cookie("session", aocSession, "/", "adventofcode.com"));
        var inp = await new HttpClient(httpMessageHandler).GetByteArrayAsync(inputUrl);
        File.WriteAllBytes(inputFilename, inp);
        Console.WriteLine($"Input downloaded to {Path.GetFullPath(inputFilename)}");
    }

    var assembly = Assembly.GetExecutingAssembly();
    var dayType = assembly.GetType("Day" + n) ?? assembly.GetType("Day0" + n)!;
    object res = Activator.CreateInstance(dayType) ?? throw new Exception("oops");
    var m = dayType.GetMethod("Solve")!;

    var sw = Stopwatch.StartNew();
    dayType.GetMethod("Solve")!.InvokeWithParsedArgs(res, inputFilename);
    Console.WriteLine("Total time: " + sw.Elapsed);
    Console.WriteLine();
}
