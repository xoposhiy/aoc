global using static System.Math; 
global using static System.Linq.Enumerable; 
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;

var year = 2023;

Console.WriteLine($"# Advent of Code {year}");
Console.WriteLine();

await SolveDay(14);
//foreach (var dayNumber in Enumerable.Range(1, DateTime.Now.Day)) await SolveDay(dayNumber);
//foreach (var dayNumber in Enumerable.Range(1, 25)) await SolveDay(dayNumber);

async Task SolveDay(int? optionalDay = null)
{
    var day = optionalDay ?? (DateTime.Now.TimeOfDay.Hours < 3 ? DateTime.Now.Day - 1 : DateTime.Now.Day);
    if (day > 25) day = 25;
    Console.WriteLine("## Day " + day);
    CurrentDay.Day = day;
    var daySolution = CreateInstanceForDay(day);

    var inputFilename = $"../../../inputs/{day:D2}.txt";
    await DownloadInputIfNeeded(inputFilename, day);

    var sw = Stopwatch.StartNew();
    var singleSolve = daySolution.GetType().GetMethod("Solve");
    var lines = File.ReadAllLines(inputFilename);
    if (singleSolve != null)
    {
        singleSolve.InvokeWithParsedArgs(daySolution, lines);
    }
    else
    {
        daySolution.GetType().GetMethod("Part1")!.InvokeWithParsedArgs(daySolution, lines);
        daySolution.GetType().GetMethod("Part2")!.InvokeWithParsedArgs(daySolution, lines);
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
        var inputUrl = $"https://adventofcode.com/{year}/day/{i}/input";
        var httpMessageHandler = new HttpClientHandler();
        var args = Environment.GetCommandLineArgs();
        var aocSession = args.Length > 1 ? args[1] : Environment.GetEnvironmentVariable("AOC");
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
    var dayType = assembly.GetType("Day" + dayNumber) ?? assembly.GetType("Day0" + dayNumber);
    if (dayType == null)
    {
        var dayN = FileHelper.PatchFilename("DayN.cs");
        var content = File.ReadAllText(dayN)
            .Replace("class DayN", $"class Day{dayNumber:D2}")
            .Replace("LinkToDayN", $"https://adventofcode.com/{year}/day/{dayNumber}");
        var daySourceFile = Path.Combine(Path.GetDirectoryName(dayN)!, $"Day{dayNumber:D2}.cs");
        File.WriteAllText(daySourceFile, content, Encoding.UTF8);
        throw new NotImplementedException("No Day" + dayNumber + " class found. Created!");
    }

    return Activator.CreateInstance(dayType) ?? throw new Exception("oops");
}

public class CurrentDay
{
    public static int Day;
}