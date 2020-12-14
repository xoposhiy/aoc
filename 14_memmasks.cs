using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day14
{
    public void Solve()
    {
        var commands = File.ReadAllLines("14.txt")
            .Select(s => s.Split(new[] { '[', ']', ' ', '=' }, StringSplitOptions.RemoveEmptyEntries))
            .ToArray();
        Console.WriteLine($"Part One: {Solve(commands, oldProtocolVersion: true)}");
        Console.WriteLine($"Part Two: {Solve(commands, oldProtocolVersion: false)}");
    }

    private long Solve(string[][] commands, bool oldProtocolVersion)
    {
        var mem = new Dictionary<long, long>();
        var mask = "";
        foreach (var cmd in commands)
        {
            if (cmd[0] == "mask")
                mask = cmd[1];
            else
            {
                var value = long.Parse(cmd[2]);
                var address = long.Parse(cmd[1]);
                if (oldProtocolVersion)
                {
                    var valueString = Convert.ToString(value, 2).PadLeft(mask.Length, '0');
                    var valueChars = valueString.Zip(mask, (v, m) => m == 'X' ? v : m).ToArray();
                    mem[address] = Convert.ToInt64(new string(valueChars), 2);
                }
                else
                {
                    foreach (var realAddress in GetAddresses(address, mask))
                        mem[realAddress] = value;
                }
            }
        }

        return mem.Values.Sum();
    }

    private IEnumerable<long> GetAddresses(long address, string mask)
    {
        var addressStr = Convert.ToString(address, 2).PadLeft(mask.Length, '0');
        var res = new List<char[]> { addressStr.ToCharArray() };
        var i = 0;
        foreach (var maskBit in mask)
        {
            if (maskBit == '1')
                foreach (var item in res)
                    item[i] = '1';
            else if (maskBit == 'X')
            {
                var nextRes = new List<char[]>();
                foreach (var item in res)
                {
                    item[i] = '1';
                    nextRes.Add(item.ToArray());
                    item[i] = '0';
                    nextRes.Add(item.ToArray());
                }
                res = nextRes;
            }
            i++;
        }

        return res.Select(item => Convert.ToInt64(new string(item), 2));
    }
}