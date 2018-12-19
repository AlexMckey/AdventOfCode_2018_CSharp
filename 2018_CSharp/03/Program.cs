using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _03
{
    public struct Rect
    {
        public Rect(IEnumerable<int> ie)
        {
            var e = ie.GetEnumerator();
            e.MoveNext();
            ID = e.Current;
            e.MoveNext();
            X = e.Current;
            e.MoveNext();
            Y = e.Current;
            e.MoveNext();
            H = e.Current;
            e.MoveNext();
            W = e.Current;
        }

        public int ID;
        public int X;
        public int Y;
        public int H;
        public int W;
        public int XH => X + H;
        public int YW => Y + W;
    }

    class Program
    {
        static bool CheckRect(Rect r, int[,] area)
        {
            for (var i = r.X; i < r.XH; i++)
                for (var j = r.Y; j < r.YW; j++)
                    if (area[i, j] > 1)
                    {
                        return false;
                    }
            return true;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split("\n");

            Rect f(string s) => new Rect(s.Split("# @,:x".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse));

            var s1 = "#1 @ 82,901: 26x12";
            var r1 = f(s1);

            var rects = input.Select(f);

            var maxX = rects.Max(r => r.XH);
            var maxY = rects.Max(r => r.YW);
            var area = new int[maxX, maxY];

            foreach (var r in rects)
                for (var i = r.X; i < r.XH; i++)
                    for (var j = r.Y; j < r.YW; j++)
                        area[i, j]++;

            var cnt = 0;
            foreach (var c in area)
                if (c > 1) cnt++;

            var res1 = cnt;

            Console.WriteLine($"{res1}"); // 107820

            var res2 = rects.Where(r => CheckRect(r, area)).First().ID;

            Console.WriteLine($"{res2}"); // 661
        }
    }
}