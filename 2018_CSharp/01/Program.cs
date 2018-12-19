using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _01
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var freqSeq = input
                        .Split("\n")
                        .Select(int.Parse);
            var res1 = freqSeq.Sum();

            var curFreq = 0;
            var freqs = new HashSet<int>();
            var isFind = false;
            do
                foreach(var f in freqSeq)
                {
                    curFreq += f;
                    if (freqs.Contains(curFreq))
                    {
                        isFind = true;
                        break;
                    }
                    freqs.Add(curFreq);
                }
            while (!isFind);
            var res2 = curFreq;

            Console.WriteLine($"{res1}"); // 505
            Console.WriteLine($"{res2}"); // 72330
        }
    }
}
