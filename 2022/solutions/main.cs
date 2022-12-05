using System.Diagnostics;
using System.Net;
using System.Reflection;

var dayNumber = DateTime.Now.Day;
Console.WriteLine("Day " + dayNumber);

var inputFilename = $"../../../inputs/{dayNumber:D2}.txt";
if (!File.Exists(inputFilename))
{
    var inputUrl = $"https://adventofcode.com/2022/day/{dayNumber}/input";
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
var dayType = assembly.GetType("Day" + dayNumber) ?? assembly.GetType("Day0" + dayNumber)!;
Console.WriteLine("Solution Class: " + dayType);
object res = Activator.CreateInstance(dayType) ?? throw new Exception("oops");
var m = dayType.GetMethod("Solve")!;
Console.WriteLine("Solution Method: " + m);

var sw = Stopwatch.StartNew();
dayType.GetMethod("Solve")!.InvokeWithParsedArgs(res, inputFilename);
Console.WriteLine();
Console.WriteLine("Total time: " + sw.Elapsed);
