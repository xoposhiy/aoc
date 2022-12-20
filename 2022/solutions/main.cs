using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;

var year = 2022;

Console.WriteLine($"# Advent of Code {year}");
Console.WriteLine();

await SolveDay();
//foreach (var dayNumber in Enumerable.Range(1, DateTime.Now.Day)) await SolveDay(dayNumber);

async Task SolveDay(int? optionalDay = null)
{
    var day = optionalDay ?? DateTime.Now.Day;
    if (DateTime.Now.TimeOfDay.Hours < 3) day--;
    Console.WriteLine("## Day " + day);
    var daySolution = CreateInstanceForDay(day);

    var inputFilename = $"../../../inputs/{day:D2}.txt";
    await DownloadInputIfNeeded(inputFilename, day);

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
        var inputUrl = $"https://adventofcode.com/{year}/day/{i}/input";
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
