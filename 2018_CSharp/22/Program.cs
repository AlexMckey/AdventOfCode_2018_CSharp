using System;
using System.Security.Cryptography.X509Certificates;

namespace _22
{
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using System.IO;
    using PriorityQueueLib;
    using GraphLib;

    using static Regions;
    using static Tools;
    using static Console;

    public enum Regions
    {
        Rocky, Wet, Narrow
    }

    public enum Tools
    {
        Torch, ClimbGear, Neither
    }

    public static class Helper
    {
        public static int ToInt(this Regions rt) => (int) rt;
        public static int ToInt(this Tools tl) => (int)tl;
        public static Regions ToRT(this int num) => (Regions)num;
        public static char ToChar(this Regions rt)
        {
            switch (rt)
            {
                case Rocky: return '.';
                case Wet: return '=';
                default: return '|';
            }
        }
    }

    public class Cave
    {
        private const int Modulo = 20183;
        private const int XTimes = 16807;
        private const int YTimes = 48271;
        private readonly int depth;
        private readonly (int x, int y) target;
        public readonly int[,] EL;
        public readonly int[,] RT;

        private int GItoEL(int val) => (val + depth) % Modulo;

        public Regions this[int x, int y] => RT[x,y].ToRT();

        public int RiskLevel
        {
            get
            {
                var risk = 0;
                for (var y = 0; y <= target.y; y++)
                for (var x = 0; x <= target.x; x++)
                    risk += RT[x, y];
                return risk;
            }

        }

        public Cave(int d, (int x, int y) t, int reserve = 100)
        {
            var maxX = t.x + reserve;
            var maxY = t.y + reserve;
            var toolCnt = Enum.GetNames(typeof(Tools)).Length;
            depth = d;
            target = t;
            EL = new int[maxX + 1, maxY + 1];
            RT = new int[maxX + 1, maxY + 1];
            var graph = new Graph(maxX * maxY * toolCnt);
            var weights = new Dictionary<Edge, double>();

            for (var y = 0; y <= maxY; y++)
            for (var x = 0; x <= maxX; x++)
            {
                if (x == 0 && y == 0)
                    EL[x, y] = GItoEL(0);
                else if (x == target.x && y == target.y)
                    EL[x, y] = GItoEL(0);
                else if (x == 0)
                    EL[x, y] = GItoEL(y * YTimes);
                else if (y == 0)
                    EL[x, y] = GItoEL(x * XTimes);
                else
                    EL[x, y] = GItoEL(EL[x - 1, y] * EL[x, y - 1]);
            }

            for (var y = 0; y <= maxY; y++)
            for (var x = 0; x <= maxX; x++)
            {
                RT[x, y] = EL[x, y] % 3;
                var cur = (y * maxX + x);
                if (x != maxX - 1)
                {
                    foreach (Tools toolFrom in Enum.GetValues(typeof(Tools)))
                    {
                        foreach (Tools toolTo in Enum.GetValues(typeof(Tools)))
                        {
                            var nFrom = cur + toolFrom.ToInt();
                            var nTo = cur + toolCnt + toolTo.ToInt();
                            var e = graph.Connect(nFrom, nTo);
                        }
                    }
                }

            }

            Graph MakeGraphFromArray(int length, int width)
            {
                var cur = 0;
                while (cur < length * width - 1)
                {
                    
                    //if (toRight % width != 0)
                    //    var edge1 = graph.Connect(cur, toRight);
                    //var toDown = cur + width;
                    //if (toDown < length * width)
                    //    var edge2 = graph.Connect(cur, toDown);
                    //cur++;
                }
                return graph;
            }

            var start = graph[0];
            var end = graph[(target.y - 1) * (target.x + reserve) + target.x];
            

            bool RegionAcceptTools(Regions rt, Tools tool)
            {
                switch (rt)
                {
                    case Rocky: return tool == ClimbGear || tool == Torch;
                    case Wet: return tool == ClimbGear || tool == Neither;
                    case Narrow: return tool == Torch || tool == Neither;
                    default: return false;
                }
            }



        }

        public override string ToString()
        {
            var sb = new StringBuilder(target.x * target.y + 2 * target.y);
            for (var y = 0; y <= target.y; y++)
            {
                for (var x = 0; x <= target.x; x++)
                {
                    if (x == 0 && y == 0)
                        sb.Append('M');
                    else if (x == target.x && y == target.y)
                        sb.Append('T');
                    else
                        sb.Append(this[x,y].ToChar());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("input.txt").Split('\n');
            var arr = input.Last()
                .Replace("target: ", "")
                .Split(",").Select(int.Parse).ToList();
            var depth = int.Parse(input.First().Replace("depth: ", ""));
            var target = (arr.First(), arr.Last());
            //var depth = 510;
            //var target = new Pos(10,10);

            var cave = new Cave(depth, target);

            WriteLine(cave.ToString());

            WriteLine($"{cave.RiskLevel}");
        }
    }
}