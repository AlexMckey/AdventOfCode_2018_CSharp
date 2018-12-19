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
            HashSet<int> currentPlants;
            Dictionary<int, bool> rules;
            var input = File.ReadAllText("input.txt").Split('\n');
            //var input = "#..#.#..##......###...###\n...## => #\n..#.. => #\n.#... => #\n.#.#. => #\n.#.## => #\n.##.. => #\n.#### => #\n#.#.# => #\n#.### => #\n##.#. => #\n##.## => #\n###.. => #\n###.# => #\n####. => #".Split('\n');

            string line = input.First().Replace("initial state: ", "");
            currentPlants = line.Select((x, i) => (x: x, idx: i))
                .Where(c => c.x == '#')
                .Select(c => c.idx).ToHashSet();

            rules = input
                .Skip(2)
                .Select(str =>
                {
                    var a = str.Split(" => ");
                    return (from: Convert.ToInt32(a[0].Replace('#', '1').Replace('.', '0'), 2),
                              to: a[1] == "#");
                })
                .ToDictionary(p => p.from, p => p.to);

            //var isp0 = Convert.ToInt32(ss.AsSpan(0, 5).ToString(), 2);
            //var b0 = rules[isp0];
            

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
                    if (rules.ContainsKey(sum)) newPlants.Add(pot);
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

            Console.WriteLine(totalSum); //5250000005040
        }

    }
}