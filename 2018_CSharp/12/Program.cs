using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _12
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split('\n');
            //const long generationCount = 20;
            //const long generationCount = 50000000000;
            const long generationCount = 100;
            var testRules = "\n\n...## => #\n..#.. => #\n.#... => #\n.#.#. => #\n.#.## => #\n.##.. => #\n.#### => #\n#.#.# => #\n#.### => #\n##.#. => #\n##.## => #\n###.. => #\n###.# => #\n####. => #";
            var testInit = "#..#.#..##......###...###";

            //var initialState = input.First().Replace("initial state: ", "");
            var initialState = testInit;
            //var rules = input
            var rules = testRules.Split('\n')
                .Skip(2)
                .Select(s =>
                    {
                        var a = s.Split(" => ");
                        return (from: a[0], to: a[1]);
                    })
                .ToDictionary(p => p.from, p => p.to);

            //var plantsCountInGen = new List<int>();
            //var potsWithDeltaByGen = new List<(int gen, int dleft, int dright, string pots)>();
            var dl = 0;
            var dr = 0;
            var curPots = initialState;
            //plantsCountInGen.Add(curPots.Count(c => c == '#'));
            //potsWithDeltaByGen.Add((0, 0, 0, curPots));

            //Console.WriteLine("  0: " + curPots);

            for (var gen = 1L; gen <= generationCount; gen++)
            {
                var sb = new StringBuilder();
                curPots = "...." + curPots + "....";
                for (var pot = 0; pot <= curPots.Length - 5; pot++)
                {
                    var source = curPots.Substring(pot, 5);
                    if (rules.ContainsKey(source))
                        sb.Append(rules[source]);
                    else sb.Append(".");
                }
                var i = 0;
                while (sb[0] == '.' && i <= 2)
                {
                    sb.Remove(0, 1);
                    i++;
                }
                dl += 2 - i;
                i = 0;
                while (sb[sb.Length-1] == '.' && i <= 2)
                {
                    sb.Remove(sb.Length - 1, 1);
                    i++;
                }
                dr += 2 - i;
                curPots = sb.ToString();
                //plantsCountInGen.Add(curPots.Count(c => c == '#'));
                //potsWithDeltaByGen.Add((gen, dl, dr, curPots));
                //Console.WriteLine((gen < 10 ? "  " : gen < 100 ? " " : "") + $"{gen}: " + curPots);
            }

            //var res1 = plantsCountInGen.Sum();
            //long tl = initialState.Length;
            //var sl = potsWithDeltaByGen.Select(p => p.dleft).Sum();
            //var sr = potsWithDeltaByGen.Select(p => p.dright).Sum();
            //var r = Enumerable.Range(-dl, dl + tl + dr);
            var res1 = 0L;
            var cur = -dl;
            foreach (var c in curPots)
            {
                res1 += (c == '#' ? cur : 0L);
                cur++;
            }
            //var pi = curPots.Select(c => c == '#' ? 1 : 0).Zip(r, (k, i) => k*i).Sum();

                //Console.WriteLine($"{res1}");
            Console.WriteLine($"{res1}"); //1374
        }
    }
}