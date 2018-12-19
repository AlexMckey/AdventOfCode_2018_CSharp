using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _13
{
    using static Direction;
    using static Turn;

    public enum Direction
    {
        Up, Left, Down, Right
    }

    public enum Turn
    {
        toLeft, straight, toRight
    }

    public class Cart
    {
        public (int x, int y) pos;
        public Direction dir;
        public bool crashed;
        private int curCrossroadNum;

        public Cart(int x, int y, Direction d)
        {
            pos.x = x;
            pos.y = y;
            dir = d;
            crashed = false;
            curCrossroadNum = 0;
        }

        private void TurnOnCrossroad()
        {
            var turn = (Turn)(curCrossroadNum % 3); /* 3 == Enum.GetNames(typeof(Turn)).Length*/
            var dirDelta = 0;
            if (turn == toLeft) dirDelta = 1;
            if (turn == toRight) dirDelta = -1;
            var newDir = (((int)dir + 4 + dirDelta) % 4); /*4 == Enum.GetNames(typeof(Direction)).Length*/
            dir = (Direction)newDir;
            curCrossroadNum++;
        }

        public void Move(char curObj)
        {
            switch (curObj)
            {
                case '/' :
                    switch (dir)
                    {
                        case Up: dir = Right; break;
                        case Left: dir = Down; break;
                        case Down: dir = Left; break;
                        case Right: dir = Up; break;
                    }
                    break;
                case '\\':
                    switch (dir)
                    {
                        case Up: dir = Left; break;
                        case Left: dir = Up; break;
                        case Down: dir = Right; break;
                        case Right: dir = Down; break;
                    }
                    break;
                case '+' : TurnOnCrossroad(); break;
            }
            switch (dir)
            {
                case Up: pos.y -= 1; break;
                case Left: pos.x -= 1; break;
                case Down: pos.y += 1; break;
                case Right: pos.x += 1; break;
            }
        }
    }

    public class Track
    {
        private string[] track;
        public List<Cart> Carts { get; private set; } = new List<Cart>();
        public int Step { get; private set; } = 0;
        
        public Track(string[] track)
        {
            this.track = track;
            for (var y = 0; y < track.Length; y++)
                for (var x = 0; x < track[0].Length; x++)
                {
                    var obj = this[x, y];
                    if (!"|-\\/+ ".Contains(obj))
                    {
                        Direction cartDir = Left;
                        switch (obj)
                        {
                            case '>':
                                cartDir = Right;
                                this[x, y] = '-';
                                break;
                            case '<':
                                cartDir = Left;
                                this[x, y] = '-';
                                break;
                            case '^':
                                cartDir = Up;
                                this[x, y] = '|';
                                break;
                            case 'v':
                                cartDir = Down;
                                this[x, y] = '|';
                                break;
                        }
                        Carts.Add(new Cart(x, y, cartDir));
                    }
                }
        }

        public char this[int x, int y]
        {
            get
            {
                return track[y][x];
            }
            private set
            {
                var str = track[y];
                var leftStr = str.Substring(0, x);
                var rightStr = str.Substring(x + 1);
                track[y] = leftStr + value + rightStr;
            }
        }

        public char this[(int x, int y) pos]
        {
            get => this[pos.x, pos.y];
            private set => this[pos.x, pos.y] = value;
        }

        public void nextStep(bool labelCrash = true)
        {
            foreach (var cart in Carts)
            {
                cart.Move(this[cart.pos]);

                var crashes = Carts
                    .GroupBy(c => c.pos, c => c)
                    .Where(g => g.Count() > 1);

                if (crashes.Count() > 0)
                {
                    crashes.SelectMany(g => g.Select(c => c)).ToList().ForEach(c => c.crashed = true);
                    if (labelCrash) crashes.Select(g => g.Key).ToList().ForEach(pos => this[pos] = 'X');
                }
            }
        }

        public (int x, int y) GoRace(int part = 1, bool printFinalTrack = true, bool printStep = false)
        {
            var result = (0, 0);

            if (printStep)
            {
                Console.WriteLine($"{Step}:");
                PrintTrack();
            }

            var done = false;
            do
            {
                Carts = Carts.Where(c => !c.crashed).OrderBy(c => c.pos.y).ThenBy(c => c.pos.x).ToList();

                nextStep(part == 1 ? true : false);
                Step++;

                if (part == 1) done = Carts.Count(c => c.crashed) > 0;
                if (part == 2) done = Carts.Count(c => !c.crashed) == 1;

                if (printStep)
                {
                    Console.WriteLine($"{Step}:");
                    PrintTrack();
                }
            } while (!done);

            if (part == 1) result = Carts.First(c => c.crashed).pos;
            if (part == 2) result = Carts.First(c => !c.crashed).pos;

            if (printFinalTrack && !printStep)
            {
                Console.WriteLine($"{Step}:");
                PrintTrack();
            }
            return result;
        }

        public void PrintTrack()
        {
            for (var y = 0; y < track.Length; y++)
            {
                for (var x = 0; x < track[0].Length; x++)
                {
                    var curChar = this[x, y]; 
                    for (var i = 0; i < Carts.Count(); i++)
                        if (Carts[i].pos.x == x && Carts[i].pos.y == y && !Carts[i].crashed)
                            switch (Carts[i].dir)
                            {
                                case Left: curChar = '<'; break;
                                case Right: curChar = '>'; break;
                                case Up: curChar = '^'; break;
                                case Down: curChar = 'v'; break;
                            }
                    Console.Write(curChar);
                }
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');
//            var input = @"/->-\        
//|   |  /----\
//| /-+--+-\  |
//| | |  | v  |
//\-+-/  \-+--/
//  \------/   ".Split("\r\n");

            var race = new Track(input.ToArray());

            var res1 = race.GoRace(1, false);
            Console.WriteLine($"{res1}"); //8,3

            race = new Track(input.ToArray());

            var res2 = race.GoRace(2, false);
            Console.WriteLine($"{res2}"); //73,121
        }
    }
}
