using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _06_p1_Bad
{
    class Program
    {
        static void PrintArea(Dictionary<(int x, int y), (int id, int move)> dict)
        {
            var maxX = dict.Keys.Max(p => p.x);
            var maxY = dict.Keys.Max(p => p.y);
            
            for (var j = 0; j <= maxY; j++)
            {
                for (var i = 0; i <= maxX; i++)
                {
                    if (!dict.ContainsKey((i, j)))
                        Console.Write(".");
                    else
                    {
                        var cell = dict[(i, j)];
                        if (cell.id == -1)
                            Console.Write("#");
                        else
                        {
                            var ch = (char) (cell.id + 'a');
                            if (cell.move == 0)
                                Console.Write(ch.ToString().ToUpper());
                            else
                                Console.Write(ch);
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split("\n");
            //var input = "1, 1\n1, 6\n8, 3\n3, 4\n5, 5\n8, 9".Split('\n');
            var danger = input
                .Select(s => s
                    .Split(", ")
                    .Select(int.Parse))
                .Select(lst => (x: lst.First(), y: lst.Last(), isInf: false))
                .ToArray();
            //Console.WriteLine($"Danger Count = {danger.Length}");

            var maxX = danger.Max(p => p.x);
            var maxY = danger.Max(p => p.y);
            var minX = danger.Min(p => p.x);
            var minY = danger.Min(p => p.y);

            var curMove = 0;
            var map = danger
                .Select((p, i) => (p: (x: p.x, y: p.y), id: i))
                .ToDictionary(o => o.p, o => (id: o.id, move: curMove));

            //PrintArea(map);

            var cellToAdd = 0;
            do {
                curMove++;
                //Console.WriteLine($"Current Move = {curMove}");
                var cellsForNextMove = map.Where(kvp => kvp.Value.move == curMove - 1);
                //Console.WriteLine($"Count cells may expand in that Move = {cellsForNextMove.Count()}");
                var expandedCells = cellsForNextMove.SelectMany(cell => new List<KeyValuePair<(int x, int y), (int id, int move)>>() {
                    new KeyValuePair<(int x, int y),(int id, int move)>((cell.Key.x+1, cell.Key.y), (cell.Value.id, curMove)),
                    new KeyValuePair<(int x, int y),(int id, int move)>((cell.Key.x, cell.Key.y+1), (cell.Value.id, curMove)),
                    new KeyValuePair<(int x, int y),(int id, int move)>((cell.Key.x-1, cell.Key.y), (cell.Value.id, curMove)),
                    new KeyValuePair<(int x, int y),(int id, int move)>((cell.Key.x, cell.Key.y-1), (cell.Value.id, curMove)),
                });
                //Console.WriteLine($"Count cells expanded in that move = {expandedCells.Count()}");
                var filt1 = expandedCells.Where(cell => !map.ContainsKey(cell.Key)).Distinct();
                //Console.WriteLine($"Count expanded cells that can't overrlaped with present cells = {filt1.Count()}");
                var filt2 = filt1.GroupBy(cell => cell.Key, cell => cell.Value).Where(g => g.Count() > 1);
                //Console.WriteLine($"Count expanded cells that overrlaped in that move = {filt2.Count()}");
                var filtInc = filt2.Select(g => new KeyValuePair<(int x, int y), (int id, int move)>((g.Key.x, g.Key.y), (-1, curMove)));
                var filtExc = filt2.SelectMany(g => (g.Select(kvp => new KeyValuePair<(int x, int y), (int id, int move)>((g.Key.x, g.Key.y), (kvp)))));
                var filt1Exc = filt1.Except(filtExc);
                var filt1Inc = filt1Exc.Union(filtInc);
                var filt3 = filt1Inc
                    .Where(cell => cell.Key.x > maxX || cell.Key.x < minX || cell.Key.y > maxY || cell.Key.y < minY);
                //Console.WriteLine($"Count expanded cells that went beyond area = {filt3.Count()}");
                if (filt3.Count() > 0)
                    foreach (var cell in filt3)
                        if (cell.Value.id == -1) continue;
                        else danger[cell.Value.id].isInf = true;
                var filt = (filt3.Count() > 0 ? filt1Inc.Except(filt3) : filt1Inc).ToList();
                cellToAdd = filt.Count();
                //Console.WriteLine($"Count expanded cells that to be a new area = {cellToAdd}");
                foreach (var cell in filt)
                    map.Add(cell.Key, cell.Value);
                //PrintArea(map);
            } while (cellToAdd > 0);

            var dNotInf = danger.Select((d,i) => (d.isInf,i)).Where((d,i) => !d.isInf).Select((d,i) => i);
            var dMaxCnt = dNotInf.Select(d => map.Values.Count(v => v.id == d)).Max();

            Console.WriteLine(dMaxCnt); // 6047
        }
    }
}
