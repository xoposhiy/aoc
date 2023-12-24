// https://adventofcode.com/2023/day/24

using Microsoft.Z3;

public class Day24
{
    public record Hail(long X, long Y, long Z, long Vx, long Vy, long Vz);

    public void Solve([Separators(" @,")] Hail[] hs)
    {
        var low = 200000000000000;
        var high = 400000000000000;
        //var low = 7;
        //var high = 27;
        var count = 0;
        for (int i = 0; i < hs.Length; i++)
        for (int j = i + 1; j < hs.Length; j++)
        {
            if (IsIntersected(hs[i], hs[j], low, high))
                count++;
        }

        count.Part1();
        SolvePart2WithZ3(hs).Part2();
    }

    private bool IsIntersectedZ3(Hail a, Hail b, long low, long high)
    {
        // Too slow, but correct
        using var ctx = new Context();
        var solver = ctx.MkSolver();
        var ax = ctx.MkReal(a.X);
        var ay = ctx.MkReal(a.Y);
        var bx = ctx.MkReal(b.X);
        var by = ctx.MkReal(b.Y);
        var avx = ctx.MkReal(a.Vx);
        var avy = ctx.MkReal(a.Vy);
        var bvx = ctx.MkReal(b.Vx);
        var bvy = ctx.MkReal(b.Vy);
        var t1 = ctx.MkRealConst("t1");
        var t2 = ctx.MkRealConst("t2");
        var x = ctx.MkRealConst("x");
        var y = ctx.MkRealConst("y");
        solver.Add(ctx.MkEq(x, ax + avx*t1));
        solver.Add(ctx.MkEq(x, bx + bvx*t2));
        solver.Add(ctx.MkEq(y, by + bvy*t2));
        solver.Add(ctx.MkEq(y, ay + avy*t1));
        solver.Add(t1 >= 0);
        solver.Add(t2 >= 0);
        solver.Add(x >= low);
        solver.Add(x <= high);
        solver.Add(y >= low);
        solver.Add(y <= high);
        return solver.Check() == Status.SATISFIABLE;
    }

    private bool IsIntersected(Hail a, Hail b, long low, long high)
    {
        // intersection in future in XY axis
        var (x1, y1, _ , v1x, v1y, _) = a;
        var (x2, y2, _ , v2x, v2y, _) = b;
        // x1 + v1.X * t1 = x2 + v2.X * t2
        // y1 + v1.Y * t1 = y2 + v2.Y * t2
        // solve for t1, t2. Eliminate t1:
        // x1*v1.Y - y1*v1.X = x2*v1.Y - y2*v1.X + v2.X * t2 * v1.Y - v2.Y * t2 * v1.X
        // t2 = (x1*v1.Y - y1*v1.X - x2*v1.Y + y2*v1.X) / (v2.X * v1.Y - v2.Y * v1.X)
        // t1 = (x2 + v2.X * t2 - x1) / v1.X
        var vp = (double)(v2x * v1y - v2y * v1x);
        if (vp == 0)
        {
            var vp2 = x1 * v1y - y1 * v1x - x2 * v1y + y2 * v1x;
            if (vp2 == 0) throw new Exception("Shouldn't be");
            return false;
        }

        var t2 = (x1 * v1y - y1 * v1x - x2 * v1y + y2 * v1x) / (double)(v2x * v1y - v2y * v1x);
        var t1 = (x2 + v2x * t2 - x1) / (double)v1x;
        if (t1 < 0 || t2 < 0)
            return false;

        var x = x1 + v1x * t1;
        var y = y1 + v1y * t1;
        return x >= low && x <= high && y >= low && y <= high;
    }

    private double SolvePart2WithZ3(Hail[] hailstones)
    {
        using var ctx = new Context();
        var solver = ctx.MkSolver();
        var sx = ctx.MkRealConst("sx");
        var sy = ctx.MkRealConst("sy");
        var sz = ctx.MkRealConst("sz");
        var svx = ctx.MkRealConst("vx");
        var svy = ctx.MkRealConst("vy");
        var svz = ctx.MkRealConst("vz");
        for (var i = 0; i < 3; i++)
        {
            var h = hailstones[i];
            var ti = ctx.MkRealConst("t" + i);
            solver.Add(ti > 0);
            solver.Add(ctx.MkEq(sx + svx * ti, h.X + h.Vx * ti));
            solver.Add(ctx.MkEq(sy + svy * ti, h.Y + h.Vy * ti));
            solver.Add(ctx.MkEq(sz + svz * ti, h.Z + h.Vz * ti));
        }

        if (solver.Check() != Status.SATISFIABLE) throw new Exception("Unsatisfiable");
        return new[] { sx, sy, sz }.Sum(v => ((RatNum)solver.Model.Eval(v)).Double);
    }
}