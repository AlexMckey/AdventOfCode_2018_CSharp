using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _15
{
    using static BoardObjects;

    enum BoardObjects { Empty, Wall, Elves, Goblin, Unknown }
    enum Directions { Up, Right, Down, Left }

    struct Pos
    {
        public int x;
        public int y;
    }

    class Board
    {
        BoardObjects[][] board;

        public Board(IEnumerable<string> input)
        {
            BoardObjects ParseObjects(char ch)
            {
                switch (ch)
                {
                    case '#': return Wall;
                    case '.': return Empty;
                    case 'G': return Goblin;
                    case 'E': return Elves;
                    default: return Unknown;

                }
            }

            board = input.Select(str => str.Select(ParseObjects).ToArray()).ToArray();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
