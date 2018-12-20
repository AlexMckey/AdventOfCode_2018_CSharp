using System;
using System.Collections.Generic;
using System.Linq;

namespace Factorization
{
    public static class Factors
    {
        private static int[] sieveOfEratosthenes(this int n)
        {
            var result = new int[n + 1];
            result[n] = n;
            foreach (var i in Enumerable.Range(2, (int)Math.Sqrt(n)))
            {
                if (result[i] == 0)
                {
                    var shuttle = i * i;
                    while (shuttle <= n)
                    {
                        result[shuttle] = i;
                        shuttle += i;
                    }
                }
            }
            return result;
        }

        public static List<int> getMultipliers(this int number)
        {
            var result = new List<int>();
            var sieve = sieveOfEratosthenes(number);
            var curNum = number;
            while (curNum != 1)
            {
                result.Add(sieve[curNum]);
                curNum /= sieve[curNum];
            }
            return result;
        }

        public static List<int> getDividers(this int number)
        {
            var result = new List<int>();
            foreach (var probe in Enumerable.Range(2, (int)Math.Sqrt(number)))
            {
                if (number % probe == 0)
                {
                    result.Add(probe);
                    result.Add(number / probe);
                }
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var num = 10551276;
            var lst = num.getDividers();

            Console.WriteLine(string.Join(",",lst));
            Console.WriteLine(lst.Sum() + 1 + num);
        }
    }
}