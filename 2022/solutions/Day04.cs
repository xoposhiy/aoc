using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Day04
{
    // input:
    // 35-73,35-82
    // ...
    public void Solve((R, R)[] pairs)
    {
        //In how many assignment pairs does one range fully contain the other?
        pairs.Count(pair => pair.Item1.Contains(pair.Item2) || pair.Item2.Contains(pair.Item1))
            .Out("Part 1: ").ShouldBe(507);

        //In how many assignment pairs do the ranges overlap?
        pairs.Count(pair => pair.Item1.Overlaps(pair.Item2))
            .Out("Part 2: ").ShouldBe(897);
    }
}
