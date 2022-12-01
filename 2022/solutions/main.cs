using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

var dayNumber = DateTime.Now.Day;
Console.WriteLine("Day " + dayNumber);

var inputFilename = $"../../../inputs/{dayNumber:D2}.txt";
if (!File.Exists(inputFilename))
{
    var inputUrl = $"https://adventofcode.com/2022/day/{dayNumber}/input";
    var httpMessageHandler = new HttpClientHandler();
    httpMessageHandler.CookieContainer.Add(new Cookie("session", "53616c7465645f5f6a64de7a67f227afddfaa9ef2a5b6aec33555ede3f1f2c098ce5faf071395d1f4c141b2751f8fee74712ee06c0240cea706dd02465087dbb", "/", "adventofcode.com"));
    var inp = await new HttpClient(httpMessageHandler).GetByteArrayAsync(inputUrl);
    File.WriteAllBytes(inputFilename, inp);
    Console.WriteLine($"Input downloaded to {Path.GetFullPath(inputFilename)}");
}
var assembly = Assembly.GetExecutingAssembly();
var dayType = assembly.GetType("Day" + dayNumber) ?? assembly.GetType("Day0" + dayNumber)!;
Console.WriteLine("Solution Class: " + dayType);
dynamic res = Activator.CreateInstance(dayType) ?? throw new Exception("oops");
var m = dayType.GetMethod("Solve")!;
Console.WriteLine("Solution Method: " + m);

var sw = Stopwatch.StartNew();
if (m.GetParameters().Length == 0)
    res.Solve();
var param = m.GetParameters().Single();
var paramType = param.ParameterType;

if (paramType == typeof(string[]))
    res.Solve(File.ReadAllLines(inputFilename));
else if (paramType == typeof(string))
    res.Solve(File.ReadAllText(inputFilename));
else if (paramType == typeof(int[]))
    res.Solve(File.ReadAllText(inputFilename)
        .Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
else
    throw new Exception("No Solve method");
Console.WriteLine();
Console.WriteLine("Total time: " + sw.Elapsed);