using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _05
{
    class Program
    {
        static int ConstDiff = 'a' - 'A';

        static string ReducePolymer(string str, char toRemove = '\0')
        {
            var sb = new StringBuilder(str);
            if (toRemove != '\0')
            {
                sb.Replace(toRemove.ToString().ToLower(), "");
                sb.Replace(toRemove.ToString().ToUpper(), "");
            }
            var cnt = 0;
            var idx = 0;
            var isComplite = false;

            while (!isComplite)
            { 
                while (idx < sb.Length - 1)
                {
                    if (Math.Abs(sb[idx] - sb[idx + 1]) == ConstDiff)
                    {
                        sb.Remove(idx, 2);
                        cnt++;
                    }
                    else
                        idx++;
                }
                isComplite = cnt == 0;
                cnt = 0;
                idx = 0;
            }
            
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var res1 = ReducePolymer(input).Length;

            Console.WriteLine(res1); // 10762
                         
            var chars = "abcdefghijklmnopqrstuvwxvz";
            var res2 = chars.Select(ch => ReducePolymer(input, ch).Length).Min();
            
            Console.WriteLine(res2); // 6946
        }
    }
}
