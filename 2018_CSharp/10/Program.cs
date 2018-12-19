using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _10
{
    class Program
    {
        static (int x, int y, int dx, int dy) NextPoint((int x, int y, int dx, int dy) p)
        {
            return (x: p.x + p.dx, y: p.y + p.dy, p.dx, p.dy);
        }

        static (int minX, int maxX, int minY, int maxY) CalcBounds(IEnumerable<(int x, int y, int dx, int dy)> plist)
        {
            var minX = plist.Min(p => p.x);
            var maxX = plist.Max(p => p.x);
            var minY = plist.Min(p => p.y);
            var maxY = plist.Max(p => p.y);

            return (minX, maxX, minY, maxY);
        }

        static long CalcArea((int minX, int maxX, int minY, int maxY) bounds)
        {
            return (long)(bounds.maxX - bounds.minX) * (bounds.maxY - bounds.minY);
        }

        static void PrintMessage(IEnumerable<(int x, int y, int dx, int dy)> plist)
        {
            var bounds = CalcBounds(plist);
            var message = plist.Select(p => (x: p.x, y: p.y));
            var sb = new StringBuilder((int)CalcArea(bounds) + (bounds.maxX - bounds.minX) * 2);
            for (var y = bounds.minY; y <= bounds.maxY; y++)
            {
                for (var x = bounds.minX; x <= bounds.maxX; x++) 
                {
                    if (message.Contains((x, y))) sb.Append("#");
                    else sb.Append(".");
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var regexp = new Regex(@"position=<(?<X>(?:-| )\d+), (?<Y>(?:-| )?\d+)> velocity=<(?<velX>(?:-| )?\d+), +(?<velY>(?:-| )?\d+)>", RegexOptions.Compiled);

            var curMessage = regexp.Matches(input)
                .OfType<Match>()
                .Select(match => (
                    x: int.Parse(match.Groups["X"].Value), y: int.Parse(match.Groups["Y"].Value),
                    dx: int.Parse(match.Groups["velX"].Value), dy: int.Parse(match.Groups["velY"].Value)))
                .ToArray();

            var prevMessage = new(int minX, int maxX, int minY, int maxY)[curMessage.Length];
            var curArea = CalcArea(CalcBounds(curMessage));
            var deltaArea = curArea;
            var curTime = 0;
            do
            {
                curMessage.CopyTo(prevMessage, 0);
                var prevArea = curArea;
                curMessage = prevMessage.Select(p => NextPoint(p)).ToArray();
                curArea = CalcArea(CalcBounds(curMessage));
                deltaArea = prevArea - curArea;
                curTime++;
            } while (deltaArea > 0);

            PrintMessage(prevMessage); // print part1 result
            var res2 = curTime - 1;

            Console.WriteLine($"{res2}"); //10454
        }
    }
}
