using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _20
{
    public struct Pos
    {
        public int x;
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Pos operator + (Pos pos, (int x, int y) pair)
        {
            return new Pos { x = pos.x + pair.x, y = pos.y + pair.y };
        }

        public Pos GoDir(char dir)
        {
            switch (dir)
            {
                case 'W': return this + (-1, 0);
                case 'N': return this + (0, 1);
                case 'E': return this + (1, 0);
                case 'S': return this + (0, -1);
                default: return this;
            }
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }

    public class Path
    {
        public Dictionary<Pos, int> Rooms = new Dictionary<Pos, int>();

        public Path(string regex)
        {
            var cur = new Pos(0, 0);
            var stack = new Stack<Pos>();
            Rooms.Add(cur, 0);
            foreach (var c in regex)
            {
                switch(c)
                {
                    case '(':
                        stack.Push(cur);
                        break;
                    case ')':
                        cur = stack.Pop();
                        break;
                    case '|':
                        cur = stack.Peek();
                        break;
                    case '^': break;
                    case '$': break;
                    default:
                        var next = cur.GoDir(c);
                        if (Rooms.ContainsKey(next))
                            Rooms[next] = Math.Min(Rooms[next], Rooms[cur] + 1);
                        else Rooms.Add(next, Rooms[cur] + 1);
                        cur = next;
                        break;
                }
            }
        }

        public int MaxPath => Rooms.Max(kvp => kvp.Value);
        public int MaxPathGreater1000 => Rooms.Where(kvp => kvp.Value >= 1000).Count();
    }

    public static class Program
    {
        
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            //var input = "^WNE$"; //3
            //var input = "^ENWWW(NEEE|SSE(EE|N))$"; // 10
            //var input = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$"; // 18
            //var input = "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$"; // 23
            //var input = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$"; // 31

            var path = new Path(input);

            var res1 = path.MaxPath;

            Console.WriteLine($"{res1}");

            var res2 = path.MaxPathGreater1000;

            Console.WriteLine($"{res2}");
        }
    }
}
