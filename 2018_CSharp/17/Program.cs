using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _17
{
    using static Direction;

    public enum Direction { Down, Left, Right, Up }

    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Pos AtLeft => new Pos(x - 1, y);
        public Pos AtRight => new Pos(x + 1, y);
        public Pos AtDown => new Pos(x, y + 1);
        public Pos AtUp => new Pos(x, y - 1);

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }

    public class State
    {
        public Pos startPos;
        public Pos leftPos;
        public Pos rightPos;
        public Direction dir = Down;
        public bool leftClosed = false;
        public char ch = '.';
        public bool isVertical = false;
        public bool isClosed = false;
    }

    public struct FillingPos
    {
        public bool horizontal;
        public int by;
        public int from;
        public int to;

        public FillingPos(bool horizontal, int by, int from, int to)
        {
            this.horizontal = horizontal;
            this.by = by;
            this.from = from;
            this.to = to;
        }

        public IEnumerable<Pos> Positions()
        {
            for (var i = from; i <= to; i++)
                yield return (horizontal ? new Pos(by, i) : new Pos(i, by));
        }
    }

    public static class PosFactory
    {
        public static IEnumerable<Pos> ToPositions(this string str)
        {
            var isFirstX = str[0] == 'x';
            var coords = str.Replace("x=", "").Replace("y=", "").Split(", ");
            var firstCoord = int.Parse(coords[0]);
            var range = coords[1].Split("..").Select(s => int.Parse(s));
            var startRange = range.First();
            var endRange = range.Last();
            return new FillingPos(isFirstX, firstCoord, startRange, endRange).Positions();
        }
    }

    public class Subsoil
    {
        public Dictionary<Pos, char> map = new Dictionary<Pos, char>();
        private Pos startPos;
        public int MaxDepth;
        public int MinDepth;
        private Queue<Pos> q = new Queue<Pos>();

        public Subsoil(IEnumerable<string> definiton)
        {
            FillMapByChar(definiton
                .SelectMany(str => str.ToPositions())
                .Distinct(), '#');
            MaxDepth = map.Keys.Max(pos => pos.y);
            MinDepth = map.Keys.Min(pos => pos.y);
        }

        private void FillMapByChar(IEnumerable<Pos> poss, char ch)
        {
            foreach (var pos in poss)
                if (map.ContainsKey(pos))
                    map[pos] = ch;
                else map.Add(pos, ch);
        }

        public void FillWithWater(Pos waterSpring, bool printStep = false, bool each = false)
        {
            startPos = waterSpring;
            q.Enqueue(startPos);
            var done = false;
            var step = 0;
            if (printStep && each) PrettyPrintMap();
            do
            {
                //if (step == 10)
                //{
                //    Console.WriteLine(q.Peek());
                //}
                DoOneStep();
                done = !q.TryPeek(out _);
                //if (step == 1000)
                //{
                //    Console.WriteLine(q.Peek());
                //    done = true;
                //}
                if (printStep && each) PrettyPrintMap();
                step++;
            } while (!done);
            //this[startPos] = '+';
            if (printStep) PrettyPrintMap();
        }

        private void DoOneStep()
        {
            var curPos = q.Dequeue();
            if (curPos.y >= MaxDepth) return;
            var dir = (this[curPos.AtDown] == '#' || this[curPos.AtDown] == '~') ? Left : Down;
            var state = new State() { startPos = curPos, leftPos = curPos, rightPos = curPos, dir = dir};
            var result = (complite: false, state: state);
            while (!result.complite)
                result = NextStep(result.state);
            if (result.state.isVertical)
            {
                var fp = new FillingPos(true, result.state.startPos.x, result.state.startPos.y, result.state.leftPos.y);
                FillMapByChar(fp.Positions(), result.state.ch);
            }
            else
            {
                var fp = new FillingPos(false, result.state.startPos.y, result.state.leftPos.x, result.state.rightPos.x);
                FillMapByChar(fp.Positions(), result.state.ch);
            }
        }
        
        private (bool complite, State state) NextStep(State state)
        {
            if (state.dir == Down)
            {
                if (this[state.leftPos.AtDown] == '.')
                {
                    if (state.leftPos.y > MaxDepth)
                    {
                        state.ch = '|';
                        state.isVertical = true;
                        return (true, state);
                    }
                    else
                    {
                        state.leftPos = state.leftPos.AtDown;
                        return (false, state);
                    }
                }
                else if (this[state.leftPos.AtDown] == '|')
                {
                    state.ch = '|';
                    state.isVertical = true;
                    return (true, state);
                }
                else // '#' или '~'
                {
                    q.Enqueue(state.leftPos);
                    state.ch = '|';
                    state.isVertical = true;
                    return (true, state);
                }
            }
            else if (state.dir == Left)
            {
                if (this[state.leftPos.AtDown] == '#' || this[state.leftPos.AtDown] == '~')
                {
                    if (this[state.leftPos.AtLeft] == '#')
                    {
                        state.dir = Right;
                        state.leftClosed = true;
                        return (false, state);
                    }
                    else if (this[state.leftPos.AtLeft] == '.')
                    {
                        state.leftPos = state.leftPos.AtLeft;
                        return (false, state);
                    }
                    else // '|'
                    {
                        if (this[state.leftPos.AtLeft.AtDown] == '.')
                        {
                            q.Enqueue(state.leftPos.AtLeft);
                            state.dir = Right;
                            return (false, state);
                        }
                        else
                        {
                            state.leftPos = state.leftPos.AtLeft;
                            return (false, state);
                        }
                    }
                }
                else // снизу не '#' или '~' - значит возможно '.' или '|'
                {
                    if (this[state.leftPos.AtDown] == '.')
                    {
                        q.Enqueue(state.leftPos);
                        state.dir = Right;
                        state.leftClosed = false;
                        return (false, state);
                    }
                    else // '|'
                    {
                        if (this[state.leftPos.AtLeft] == '|')
                        {
                            state.dir = Right;
                            state.leftClosed = false;
                            return (false, state);
                        }
                        else
                        {
                            state.ch = '|';
                            state.isClosed = false;
                            return (true, state);
                        }
                    }
                }
            }
            else // Right
            {
                if (this[state.rightPos.AtDown] == '#' || this[state.rightPos.AtDown] == '~')
                {
                    if (this[state.rightPos.AtRight] == '#')
                    {
                        if (state.leftClosed)
                        {
                            q.Enqueue(state.startPos.AtUp);
                            state.isClosed = true;
                            state.ch = '~';
                            return (true, state);
                        }
                        else
                        {
                            state.isClosed = false;
                            state.ch = '|';
                            return (true, state);
                        }
                    }
                    else if (this[state.rightPos.AtRight] == '.')
                    {
                        state.rightPos = state.rightPos.AtRight;
                        return (false, state);
                    }
                    else // '|'
                    {
                        if (this[state.rightPos.AtRight.AtDown] == '.')
                        {
                            q.Enqueue(state.rightPos.AtRight);
                            state.isClosed = false;
                            state.ch = '|';
                            return (true, state);
                        }
                        else
                        {
                            state.rightPos = state.rightPos.AtRight;
                            return (false, state);
                        }
                    }
                }
                else // снизу не '#' или '~' - значит возможно '.' или '|'
                {
                    if (this[state.rightPos.AtDown] == '.')
                    {
                        q.Enqueue(state.rightPos);
                        state.ch = '|';
                        state.isClosed = false;
                        return (true, state);
                    }
                    else // '|'
                    {
                        if (this[state.rightPos.AtRight] == '|')
                        {
                            state.ch = '|';
                            state.isClosed = false;
                            return (true, state);
                        }
                        else
                        {
                            state.ch = '|';
                            state.isClosed = false;
                            return (true, state);
                        }
                    }
                }
            }
        }

        public char this[Pos pos]
        {
            get => map.ContainsKey(pos) ? map[pos] : '.';
            internal set
            {
                if (value == '.') return;
                if (map.ContainsKey(pos))
                    map[pos] = value;
                else map.Add(pos, value);
            }
        }

        public int WaterArea => map.Where(kvp => kvp.Key.y <= MaxDepth && kvp.Key.y >= MinDepth).Count(kvp => kvp.Value == '~' || kvp.Value == '|');
        public int WA => map.Count(kvp => kvp.Value == '~' || kvp.Value == '|');

        public int ClosedWaterArea => map.Where(kvp => kvp.Key.y <= MaxDepth).Count(kvp => kvp.Value == '~');
        
        public void PrettyPrintMap()
        {
            var minX = map.Keys.Min(pos => pos.x);
            var maxX = map.Keys.Max(pos => pos.x);
            var WaterSum = 0;
            for (var y = 0; y <= MaxDepth; y++)
            {
                var yStr = ((y < 10) ? "000" : (y < 100) ? "00" : (y < 1000) ? "0" : "") + y.ToString() + ": ";
                Console.Write(yStr);
                var waterCount = 0;
                for (var x = minX; x <= maxX; x++)
                {
                    var pos = new Pos(x, y);
                    Console.Write(this[pos]);
                    if (this[pos] == '~' || this[pos] == '|') waterCount++;
                }
                Console.Write($" :{yStr}: wa = ");

                var waterCountStr = ((waterCount < 10) ? "000" : (waterCount < 100) ? "00" : (waterCount < 1000) ? "0" : "") + waterCount.ToString();
                Console.WriteLine(waterCountStr);
                WaterSum += waterCount;
            }
            Console.WriteLine($"wa: {WaterArea}");
            Console.WriteLine($"wa: {WaterSum}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');
            //var inputStr = "x=494, y=2..10\ny=10, x=494..503\nx=503, y=2..10\nx=497, y=4..7\ny=7, x=497..500\nx=500, y=4..7";
            //var inputStr = "x=495, y=2..7\ny=7, x=495..501\nx=501, y=3..7\nx=498, y=2..4\nx=506, y=1..2\nx=498, y=10..13\nx=504, y=10..13\ny=13, x=498..504";
            //var input = inputStr.Split('\n');


            //FileStream filestream = new FileStream("out.txt", FileMode.Create);
            //var streamwriter = new StreamWriter(filestream);
            //streamwriter.AutoFlush = true;
            //Console.SetOut(streamwriter);
            //Console.SetError(streamwriter);

            var waterSource = new Pos(500, 0);
            var ground = new Subsoil(input);
            //ground.FillWithWater(waterSource, true, true);
            //ground.FillWithWater(waterSource,true);
            ground.FillWithWater(waterSource);

            var res1 = ground.WaterArea;

            Console.WriteLine($"{res1}"); //42389
            //Console.WriteLine($"{ground.WA}");

            var res2 = ground.ClosedWaterArea;

            Console.WriteLine($"{res2}"); //34497

            //var day17 = new Day17();
            //day17.Go();

            //var minX = ground.map.Keys.Min(pos => pos.x);
            //var maxX = ground.map.Keys.Max(pos => pos.x);
            //day17.PrettyPrintGrid(minX, maxX, ground.MaxDepth);
        }
    }

    public class Day17
    {
        char[,] grid;
        int maxY = 0;
        int minY = int.MaxValue;


        public void PrettyPrintGrid(int minx, int maxx, int maxdepth)
        {
            var minX = minx;
            var maxX = maxx;
            var WaterSum = 0;
            for (var y = 0; y <= maxdepth; y++)
            {
                var yStr = ((y < 10) ? "000" : (y < 100) ? "00" : (y < 1000) ? "0" : "") + y.ToString() + ": ";
                var waterCount = 0;
                Console.Write(yStr);
                for (var x = minX; x <= maxX; x++)
                {
                    var pos = new Pos(x, y);
                    if (grid[pos.x, pos.y] == '\0') Console.Write('.');
                    else Console.Write(grid[pos.x, pos.y]);
                    if (grid[pos.x, pos.y] == 'W' || grid[pos.x, pos.y] == '|') waterCount++;
                }
                Console.Write($" :{yStr}: wa = ");

                var waterCountStr = ((waterCount < 10) ? "000" : (waterCount < 100) ? "00" : (waterCount < 1000) ? "0" : "") + waterCount.ToString();
                Console.WriteLine(waterCountStr);
                WaterSum += waterCount;
            }
            Console.WriteLine(WaterSum);
        }
        
        public void Go()
        {
            var input = File.ReadAllLines("input.txt");
            var x = 2000;
            var y = 2000;

            grid = new char[x, y];

            foreach (var line in input)
            {
                var l = line.Split(new[] { '=', ',', '.' });

                if (l[0] == "x")
                {
                    x = int.Parse(l[1]);
                    y = int.Parse(l[3]);
                    var len = int.Parse(l[5]);
                    for (var a = y; a <= len; a++)
                    {
                        grid[x, a] = '#';
                    }
                }
                else
                {
                    y = int.Parse(l[1]);
                    x = int.Parse(l[3]);
                    var len = int.Parse(l[5]);
                    for (var a = x; a <= len; a++)
                    {
                        grid[a, y] = '#';
                    }
                }

                if (y > maxY)
                {
                    maxY = y;
                }

                if (y < minY)
                {
                    minY = y;
                }
            }

            var springX = 500;
            var springY = 0;

            // fill with water
            GoDown(springX, springY);

            // count spaces with water
            var t = 0;
            for (y = minY; y < grid.GetLength(1); y++)
            {
                for (x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y] == 'W' || grid[x, y] == '|') // Part 1
                    // if (grid[x,y] == 'W') // Part 2
                    {
                        t++;
                    }
                }
            }

            Console.WriteLine(t);
            Console.WriteLine(minY);
            Console.WriteLine(maxY);


        }

        private bool SpaceTaken(int x, int y)
        {
            return grid[x, y] == '#' || grid[x, y] == 'W';
        }

        public void GoDown(int x, int y)
        {
            grid[x, y] = '|';
            while (grid[x, y + 1] != '#' && grid[x, y + 1] != 'W')
            {

                y++;
                if (y > maxY)
                {
                    return;
                }
                grid[x, y] = '|';
            };

            do
            {
                bool goDownLeft = false;
                bool goDownRight = false;

                // find boundaries
                int minX;
                for (minX = x; minX >= 0; minX--)
                {
                    if (SpaceTaken(minX, y + 1) == false)
                    {
                        goDownLeft = true;
                        break;
                    }

                    grid[minX, y] = '|';

                    if (SpaceTaken(minX - 1, y))
                    {
                        break;
                    }

                }

                int maxX;
                for (maxX = x; maxX < grid.GetLength(0); maxX++)
                {
                    if (SpaceTaken(maxX, y + 1) == false)
                    {
                        goDownRight = true;

                        break;
                    }

                    grid[maxX, y] = '|';

                    if (SpaceTaken(maxX + 1, y))
                    {
                        break;
                    }

                }

                // handle water falling
                if (goDownLeft)
                {
                    if (grid[minX, y] != '|')
                        GoDown(minX, y);
                }

                if (goDownRight)
                {
                    if (grid[maxX, y] != '|')
                        GoDown(maxX, y);
                }

                if (goDownLeft || goDownRight)
                {
                    return;
                }

                // fill row
                for (int a = minX; a < maxX + 1; a++)
                {
                    grid[a, y] = 'W';
                }

                y--;
            }
            while (true);
        }
    }
}
