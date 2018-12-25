using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _23
{
    using static Math;

    public struct Pos3DR
    {
        public double x;
        public double y;
        public double z;
        public double r;

        public Pos3DR(double x, double y, double z, double r)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }

        public double Distance(Pos3DR other)
        {
            return Abs(x - other.x) + Abs(y - other.y) + Abs(z - other.z);
        }

        public bool InRadius(Pos3DR other)
        {
            return Distance(other) <= r;
        }

        public override string ToString()
        {
            return $"(x={x},y={y},z={z},r=[{r}])";
        }
    }

    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var nanobots = input
                .Replace("pos=<", "")
                .Replace(">, r=", ",")
                .Split("\n")
                .Select(str => str
                    .Split(",")
                    .Select(int.Parse))
                .Select(arr =>
                {
                    var enumerable = arr as int[] ?? arr.ToArray();
                    return new Pos3DR(enumerable.First(),
                        enumerable.Skip(1).First(),
                        enumerable.Skip(2).First(),
                        enumerable.Last());
                })
                .ToList();
            var lsrn /*largestSignalRadiusNanobot*/ = nanobots
                .OrderByDescending(nb => nb.r).First();
            var botsInRadius = nanobots
                .Count(nb => lsrn.InRadius(nb));
            var res1 = botsInRadius;

            Console.WriteLine($"{res1}");

            var point = nanobots.Aggregate((n1, n2) =>
                new Pos3DR((n1.x + n2.x) / 2, (n1.y + n2.y) / 2, (n1.z + n2.z) / 2, (n1.r + n2.r) / 2));
            var botsCnt = nanobots
                .Count(nb => point.InRadius(nb));
            var dist = point.Distance(new Pos3DR(0, 0, 0, 0));
            var minX = nanobots.Min(p => p.x - p.r);
            var maxX = nanobots.Max(p => p.x + p.r);
            var minY = nanobots.Min(p => p.y - p.r);
            var maxY = nanobots.Max(p => p.y + p.r);
            var minZ = nanobots.Min(p => p.z - p.r);
            var maxZ = nanobots.Max(p => p.z + p.r);
            var p1 = nanobots.First();
            var p2 = nanobots.Skip(1).First();
            var x12l = Max(p1.x - p1.r, p2.x - p2.r);
            var x12r = Min(p1.x + p1.r, p2.x + p2.r);
            var (x, y, z) = nanobots.Aggregate(
                (x: (lx: int.MinValue, rx: int.MaxValue), y: (ly: int.MinValue, ry: int.MaxValue), z: (lz: int.MinValue, rz: int.MaxValue)),
                (acc, n) => (((int, int), (int, int), (int, int)))
                    ((Max(acc.x.lx, n.x - n.r), Min(acc.x.rx, n.x + n.r)),
                        (Max(acc.y.ly, n.y - n.r), Min(acc.y.ry, n.y + n.r)),
                        (Max(acc.z.lz, n.z - n.r), Min(acc.z.rz, n.z + n.r))));

            Console.WriteLine($"{point}");
        }
    }
}
