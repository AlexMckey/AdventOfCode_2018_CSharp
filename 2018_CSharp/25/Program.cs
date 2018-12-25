using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using static System.Math;

namespace _25
{
    using static Pos4D;

    public struct Pos4D
    {
        private int[] arr;

        public int a => arr[0];
        public int b => arr[1];
        public int c => arr[2];
        public int d => arr[3];

        public Pos4D(int[] points)
        {
            arr = new int[4];
            Array.Copy(points, arr, 4);
        }

        public override bool Equals(object obj)
        {
            if (obj is Pos4D)
            {
                var p = (Pos4D)obj;
                return a == p.a && b == p.b && c == p.c && d == p.d;
            }
            else throw new ArgumentException("Argument not a Pos4D type {obj}");
        }

        public override string ToString()
        {
            return $"({a},{b},{c},{d})";
        }

        public static Pos4D operator -(Pos4D first, Pos4D second)
        {
            return new Pos4D(first.arr.Zip(second.arr, (i1, i2) => i1 - i2).ToArray());
        }

        public static double Distance(Pos4D first, Pos4D second)
        {
            return (first - second).arr.Select(Abs).Sum();
            //return Sqrt((first - second).arr.Select(a => a^2).Sum());
        }
    }

    class Program
    {
        public static IEnumerable<IEnumerable<Pos4D>> FindConstellations(List<(Pos4D pos, IEnumerable<Pos4D> pnears)> lst, IEnumerable<IEnumerable<Pos4D>> acc)
        {
            if (lst.Count() == 0) return acc;
            //var pf = lst.First().pos;
            var pnears = lst.First().pnears;
            //if (pnears.Count() == 0)
            //{
            //    var pfrest = lst.Where(p => !lst.Select(pt => p.pos).Contains(pf));
            //    return FindConstellations(pfrest, acc.Append(new List<Pos4D>() { pf }));
            //}
            var ps = lst.Where(p => pnears.Contains(p.pos)).SelectMany(p => p.pnears);
            var upnears = pnears.Union(ps);
            var psrest = lst.Where(p => !upnears.Contains(p.pos));
            return FindConstellations(psrest.ToList(), acc.Append(upnears));
        }

        public static IEnumerable<(Pos4D pos, IEnumerable<Pos4D> pnears)> FindNears(IEnumerable<Pos4D> lst, int distance = 3)
        {
            return lst.Select(p1 => (pos: p1, pnears: lst.Where(p2 => /*!p1.Equals(p2) &&*/ Distance(p1, p2) <= distance)))
                .OrderByDescending(pair => pair.pnears.Count()); ;
        }

        public static IEnumerable<Pos4D> FindNear(IEnumerable<Pos4D> lst, Pos4D pos, int distance)
        {
            var res = lst.Where(p => Distance(pos, p) <= distance);
            return res;
        }

        public static IEnumerable<IEnumerable<Pos4D>> FindClusters(IEnumerable<Pos4D> lst, int distance = 3)
        {
            var result = Enumerable.Empty<IEnumerable<Pos4D>>();
            var visited = new List<Pos4D>();
            var qToVisit = new Queue<Pos4D>();
            var unvisited = new List<Pos4D>(lst);
            while (unvisited.Count > 0)
            {
                var firstPos = unvisited.First();
                var curClaster = new List<Pos4D>();
                qToVisit.Enqueue(firstPos);
                while (qToVisit.Count > 0)
                {
                    var curPos = qToVisit.Dequeue();
                    curClaster.Add(curPos);
                    unvisited.Remove(curPos);
                    var candidates = FindNear(unvisited, curPos, distance);
                    foreach (var candidate in candidates)
                        qToVisit.Enqueue(candidate);
                }
                result = result.Append(curClaster);
            }
            return result;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');
            //var input = "0,0,0,0;3,0,0,0;0,3,0,0;0,0,3,0;0,0,0,3;0,0,0,6;9,0,0,0;12,0,0,0".Split(';'); //2
            //var input = "-1,2,2,0;0,0,2,-2;0,0,0,-2;-1,2,0,0;-2,-2,-2,2;3,0,2,-1;-1,3,2,2;-1,0,-1,0;0,2,1,-2;3,0,0,0".Split(';');
            //var input = "1,-1,0,1;2,0,-1,0;3,2,-1,0;0,0,3,1;0,0,-1,-1;2,3,-2,0;-2,2,0,0;2,-2,0,-1;1,-1,0,-1;3,2,0,2".Split(';');
            //var input = "1,-1,-1,-2;-2,-2,0,1;0,2,1,3;-2,3,-2,1;0,2,3,-2;-1,-1,1,-2;0,-2,-1,0;-2,2,3,-1;1,2,2,0;-1,-2,0,-2".Split(';');

            var pts = input.Select(s => new Pos4D(s.Split(',').Select(int.Parse).ToArray())).ToList();

            var nres = FindClusters(pts, 3);
            Console.WriteLine(nres.Count());
        }
    }
}
