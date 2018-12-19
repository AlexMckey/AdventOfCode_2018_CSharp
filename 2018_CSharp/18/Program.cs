using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _18
{
    using System.Collections;
    using static Contents;

    public enum Contents { Open, Tree, Lumber }

    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Pos operator +(Pos pos, (int x, int y) pair)
        {
            return new Pos { x = pos.x + pair.x, y = pos.y + pair.y };
        }

        public IEnumerable<Pos> Adjacent()
        {
            yield return this + (1, 1);
            yield return this + (1, 0);
            yield return this + (1, -1);
            yield return this + (0, 1);
            yield return this + (0, -1);
            yield return this + (-1, 1);
            yield return this + (-1, 0);
            yield return this + (-1, -1);
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }

    public static class ContentHelper
    {
        public static Contents ToContent(this char ch)
        {
            switch (ch)
            {
                case '.': return Open;
                case '|': return Tree;
                case '#': return Lumber;
                default: throw new ArgumentException("Неверный объект на игровом поле");
            }
        }

        public static char ToChar(this Contents c)
        {
            switch (c)
            {
                case Open: return '.';
                case Tree: return '|';
                case Lumber: return '#';
                default: return ' ';
            }
        }
    }

    public class Game
    {
        private Contents[][] board;
        private Contents[][] newboard;
        private int step = 0;
        public int maxY;
        public int maxX;

        public Game(IEnumerable<string> input)
        {
            board = input
                .Select(str => str
                    .ToCharArray()
                    .Select(ch => ch
                        .ToContent())
                    .ToArray())
                .ToArray();
            maxY = board.Length;
            maxX = board[0].Length;
            newboard = new Contents[maxY][];
            for (var i = 0; i < maxY; i++)
                newboard[i] = new Contents[maxX];
        }

        private void NewTransform()
        {
            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    var p = new Pos(x, y);
                    var ac = p.Adjacent();
                    var ca = ac.Where(pos =>
                        pos.x >= 0 &&
                        pos.x < maxX &&
                        pos.y >= 0 &&
                        pos.y < maxY);
                    var cons = ca.Select(pos => this[pos]);
                    var cnts = cons
                        .GroupBy(con => con)
                        .ToDictionary(item => item.Key, item => item.Count());
                    var ocnt = cnts.ContainsKey(Open) ? cnts[Open] : 0;
                    var tcnt = cnts.ContainsKey(Tree) ? cnts[Tree] : 0;
                    var lcnt = cnts.ContainsKey(Lumber) ? cnts[Lumber] : 0;
                    //Console.Write($"{p} = {this[p]} -> [.({ocnt}) |({tcnt}) #({lcnt})] => ");
                    switch (this[p])
                    {
                        case Open:
                            if (tcnt >= 3) newboard[p.y][p.x] = Tree;
                            else newboard[p.y][p.x] = this[p];
                            break;
                        case Tree:
                            if (lcnt >= 3) newboard[p.y][p.x] = Lumber;
                            else newboard[p.y][p.x] = this[p];
                            break;
                        case Lumber:
                            if (lcnt >= 1 && tcnt >= 1) newboard[p.y][p.x] = Lumber;
                            else newboard[p.y][p.x] = Open;
                            break;
                    }
                    //Console.WriteLine(newboard[p.y][p.x]);
                }
            }
            for (var y = 0; y < maxY; y++)
                for (var x = 0; x < maxX; x++)
                    board[y][x] = newboard[y][x];
        }

        private void Step()
        {
            NewTransform();
            step++;
        }

        public int Go(long countStep, bool printStep = false)
        {
            if (countStep < 500)
            {
                if (printStep) PrettyPrintBoard();
                while (step < countStep)
                {
                    Step();
                    if (printStep) PrettyPrintBoard();
                }
                return Score;
            }
            else
            {
                return FindCycle(countStep);
            }
        }

        public int FindCycle(long countStep)
        {
            var hs = new HashSet<(int step, int stamp)>();
            hs.Add((step, this.GetHashCode()));
            var done = false;
            do
            {
                Step();
                done = hs.Any(pair => pair.stamp == this.GetHashCode());
                if (!done) hs.Add((step, this.GetHashCode()));
            } while (!done);
            var fromStep = hs.First(pair => pair.stamp == this.GetHashCode()).step;
            var diff = countStep - fromStep;
            var nstep = diff % (step - fromStep);
            return Go(step + nstep);
        }

        private Contents this[Pos pos]
        {
            get => board[pos.y][pos.x];
            set => board[pos.y][pos.x] = value;
        }

        //public int OpenGroundCount => board.Sum(str => str.Count(o => o == Open));
        public int TreesCount => board.Sum(str => str.Count(o => o == Tree));
        public int LumberyardCount => board.Sum(str => str.Count(o => o == Lumber));
        public int Score => TreesCount * LumberyardCount;

        public override string ToString()
        {
            var sb = new StringBuilder(maxX * maxY + 2 * maxY);
            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    sb.Append(board[y][x].ToChar());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            var outer = 0;
            for (var y = 0; y < maxY; y++)
            {
                var inner = 0;
                for (var x = 0; x < maxX; x++)
                    inner += board[y][x].ToChar() * x;
                outer += inner * y;
            }
            return outer;
        }

        public void PrettyPrintBoard()
        {
            Console.WriteLine($"Step: {step}");
            Console.WriteLine(this.ToString());
            Console.WriteLine($"Tree: {TreesCount}, Lumber: {LumberyardCount}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var board = File.ReadAllText("input.txt").Split('\n');
            //var inputstr = @".#.#...|#.;.....#|##|;.|..|...#.;..|#.....#;#.#|||#|#|;...#.||...;.|....|...;||...#|.#|;|.||||..|.;...#.|..|.";
            //var board = inputstr.Split(";");
            var newGame = new Game(board);

            var part1 = 10;
            var res1 = newGame.Go(part1);

            Console.WriteLine($"{res1}"); //603098

            var part2 = 1000000000L;

            newGame = new Game(board);

            var res2 = newGame.Go(part2);
            Console.WriteLine($"{res2}"); //210000
            Console.WriteLine(newGame.ToString());
        }
    }
}
