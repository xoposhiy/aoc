using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Channels;

var sw = Stopwatch.StartNew();
var assembly = Assembly.GetExecutingAssembly();
var dayNumber = assembly.GetTypes()
    .Where(t => t.Name.StartsWith("Day"))
    .Select(t => t.Name.Substring(3))
    .Select(int.Parse)
    .OrderBy(n => n)
    .Last();
dynamic res = Activator.CreateInstance(assembly.GetType("Day" + dayNumber));
Console.WriteLine("Day " + dayNumber);
res!.Solve();
Console.WriteLine(sw.Elapsed);

/*
T[] ParseMany<T>(string input, string pattern, Func<string, string, T> parse){
    var res = Regex.Matches(input, pattern);
    return res.Select(m => parse(m.Groups[1].Value, m.Groups[2].Value)).ToArray();
}

*/