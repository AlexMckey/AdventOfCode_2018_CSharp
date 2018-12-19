using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _06
{
    class Program
    {
        static void PrintArea((int id, int dist)[,] area)
        {
            for (var y = 0; y < area.GetLength(1); y++)
            {
                for (var x = 0; x < area.GetLength(0); x++)
                {
                    if (area[x,y].id == 0)
                        Console.Write(".");
                    else
                    {
                        var cell = area[x,y];
                        if (cell.id == -1)
                            Console.Write("#");
                        else
                        {
                            var ch = (char)(cell.id + 'a' - 1);
                            if (cell.dist == 0)
                                Console.Write(ch.ToString().ToUpper());
                            else
                                Console.Write(ch);
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void PrintAreaS((int dist, bool isSafe)[,] area, (int id, Point p, bool inf)[] d, int minX, int minY)
        {
            for (var y = 0; y < area.GetLength(1); y++)
            {
                for (var x = 0; x < area.GetLength(0); x++)
                {
                    if (area[x, y].isSafe)
                        Console.Write("#");
                    else
                        Console.Write(".");
                    var pt = new Point(x + minX, y + minY);
                    var ds = d.FirstOrDefault(i => i.p == pt).id;
                    if (ds != 0)
                        Console.Write(((char)(ds + 'a' - 1)).ToString().ToUpper());
                }
                Console.WriteLine();
            }
        }

        static int CalcManhDist(Point p1, Point p2)
        {
            return Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y);
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split("\n");
            //var input = "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9".Split('\n');
            var danger = input
                .Select(s => s
                    .Split(", ")
                    .Select(int.Parse))
                .Select(lst => (p: new Point(lst.First(), lst.Last()), isInf: false))
                .Select((d, i) => (id: i + 1, p: d.p, isInf: d.isInf))
                .ToArray();

            var maxX = danger.Max(d => d.p.X);
            var maxY = danger.Max(d => d.p.Y);
            var minX = danger.Min(d => d.p.X);
            var minY = danger.Min(d => d.p.Y);

            var area = new (int id, int dist)[maxX - minX + 1, maxY - minY + 1];

            for (var y = minY; y <= maxY; y++)
                for (var x = minX; x <= maxX; x++)
                {
                    var dd = danger.Select(d => (id: d.id, dist: CalcManhDist(new Point(x, y), d.p)));
                    var mind = dd.Min(d => d.dist);
                    var mindd = dd.Where(d => d.dist == mind);
                    var mindcnt = mindd.Count();
                    var idd = mindcnt == 1 ? mindd.First().id : -1;
                    area[x - minX, y - minY] = (idd, mind);
                }

            for (var i = 0; i < maxX - minX; i++)
            {
                if (area[i, 0].id >= 0)
                    danger[area[i, 0].id-1].isInf = true;
                if (area[i, maxY-minY].id >= 0)
                    danger[area[i, maxY-minY].id-1].isInf = true;
            }

            for (var i = 0; i < maxY - minY; i++)
            {
                if (area[0,i].id != -1)
                    danger[area[0,i].id-1].isInf = true;
                if (area[maxX - minX, i].id != -1)
                    danger[area[maxX - minX, i].id-1].isInf = true;
            }
            
            //PrintArea(area);

            var darea = danger
                .Where(d => d.isInf)
                .Select(d => d.id);
            var cnt = area
                .OfType<(int id, int dist)>()
                .Where(p => !darea.Contains(p.id) && p.id != -1)
                .GroupBy(g => g.id, g => g.id)
                .Select(kvp => (kvp.Key, kvp.Count()))
                .Max(d => d.Item2);

            Console.WriteLine(cnt);

            var areaS = new(int dist, bool safe)[maxX - minX + 1, maxY - minY + 1];
            var maxD = 10000;
            //var maxD = 32;

            for (var y = minY; y <= maxY; y++)
                for (var x = minX; x <= maxX; x++)
                {
                    var dd = danger.Select(d => CalcManhDist(new Point(x, y), d.p)).Sum();
                    areaS[x - minX, y - minY] = (dd, dd < maxD);
                }

            //PrintAreaS(areaS,danger,minX,minY);

            var res2 = areaS
               .OfType<(int dist, bool isSafe)>()
               .Where(v => v.isSafe)
               .Count();

            Console.WriteLine(res2); // 46320
        }
    }
}
