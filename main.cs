using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

//new Day8().Solve();
//new Day9().Solve();
//new Day10().Solve();
//new Day11().Solve();
//new Day12().Solve();
//new Day13().Solve();
new Day14().Solve();

/*
T[] ParseMany<T>(string input, string pattern, Func<string, string, T> parse){
    var res = Regex.Matches(input, pattern);
    return res.Select(m => parse(m.Groups[1].Value, m.Groups[2].Value)).ToArray();
}

*/