using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _12_Part2
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<int> currentPlants = new HashSet<int>();
            Dictionary<int, bool> rules = new Dictionary<int, bool>();
            //var input = File.ReadAllText("input.txt").Split('\n');
            var input = "#..#.#..##......###...###\n...## => #\n..#.. => #\n.#... => #\n.#.#. => #\n.#.## => #\n.##.. => #\n.#### => #\n#.#.# => #\n#.### => #\n##.#. => #\n##.## => #\n###.. => #\n###.# => #\n####. => #".Split('\n');

            string line = input.First().Replace("initial state: ", "");
            var r1 = line.Select((x, i) => new { x, i });
            r1.Dump();
            Console.WriteLine();
            var r2 = r1
                .Where(c => c.x == '#')
                .Select(c => c.i).ToList();
            r2.Dump();
            Console.WriteLine();
            r2.ForEach(x => currentPlants.Add(x));

            var patterns = input
                .Skip(2)
                .Select(str =>
                {
                    var a = str.Split(" => ");
                    return (from: Convert.ToInt32(a[0].Replace('#', '1').Replace('.', '0'), 2),
                              to: a[1] == "#");
                })
                .ToDictionary(p => p.from, p => p.to);

            var s1 = input.Skip(2).First();
            var s2 = Convert.ToInt32(s1.Substring(0,5).Replace('#','1').Replace('.', '0'), 2);
            var s = "#..#.#..##......###...###";
            var ss = s.Replace('#', '1').Replace('.', '0');
            var isp0 = Convert.ToInt32(ss.AsSpan(0, 5).ToString(), 2);
            var b0 = patterns[isp0];
            var isp2 = Convert.ToInt32(ss.AsSpan(2, 5).ToString(), 2);
            var b2 = patterns[isp2];

            foreach (var ln in input.Skip(2))
            {
                int binary = ln.Take(5).Select((x, i) => new { x, i }).Where(c => c.x == '#').Sum(c => (int)Math.Pow(2, c.i));
                rules.Add(binary, ln[9] == '#' ? true : false);
            }


            long iterations = 50000000000;
            long totalSum = 0;
            HashSet<int> newPlants = new HashSet<int>();

            for (int iter = 1; iter <= iterations; iter++)
            {
                newPlants = new HashSet<int>();
                int min = currentPlants.Min() - 3;
                int max = currentPlants.Max() + 3;

                for (int pot = min; pot <= max; pot++)
                {
                    int sum = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (currentPlants.Contains(pot + i - 2)) sum += (int)Math.Pow(2, i);
                    }
                    if (rules[sum]) newPlants.Add(pot);
                }
                // the simulation converged to a stable point
                if (currentPlants.Select(x => x + 1).Except(newPlants).Count() == 0)
                {
                    currentPlants = newPlants;
                    totalSum = currentPlants.Sum();
                    totalSum += currentPlants.Count() * (iterations - iter);
                    break;
                }

                currentPlants = newPlants;
            }

            Console.WriteLine(totalSum);
            Console.ReadLine();
        }

    }
}