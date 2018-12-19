using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace _02
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split("\n");
            
            var twoTimes = input
                .Select(s => s
                    .GroupBy(c => c)
                    .Where(g => g.Count() == 2)
                    .Select(g => (g.Key, g.Count())))
                .Where(i => i.Count() > 0)
                .Count();
            var threeTimes = input
                .Select(s => s
                    .GroupBy(c => c)
                    .Where(g => g.Count() == 3)
                    .Select(g => (g.Key, g.Count())))
                .Where(i => i.Count() > 0)
                .Count();
            var res1 = twoTimes * threeTimes;

            Console.WriteLine($"{res1}"); //5976

            var l3 = input
                .Select(st1 => input
                    .Where(st2 => st2 != st1)
                    .Select(st2 => {
                        var zc = st1.Zip(st2, (c1, c2) => c1 == c2);
                        return (st2, zc.Count(b => b == false));
                    })
                    .OrderBy(p => p.Item2)
                    .First())
                .OrderBy(p => p.Item2);
            var r1 = l3.First().Item1;
            var r2 = l3.Skip(1).First().Item1;
            var zi = r1.Zip(r2, (c1, c2) => (ch: c1, p: c1 == c2))
                .Where(p => p.p == true)
                .Select(p => p.ch);
            var res2 = string.Join("", zi);

            Console.WriteLine($"{res2}"); //xretqmmonskvzupalfiwhcfdb
        }
    }
}
