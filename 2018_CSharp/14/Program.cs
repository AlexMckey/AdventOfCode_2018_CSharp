using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _14
{
    public class Recipes
    {
        int i;
        int j;
        List<int> seq;
        
        public Recipes()
        {
            seq = new List<int>() { 3, 7 };
            i = 0;
            j = 1;
        }

        public void Next()
        {
            var fst = seq[i];
            var snd = seq[j];
            var sum = fst + snd;
            if (sum > 9) seq.Add(sum / 10);
            seq.Add(sum % 10);
            var len = seq.Count;
            i = (i + 1 + fst) % len;
            j = (j + 1 + snd) % len;
        }

        public string CurrentScoreBoard => string.Join("", seq);

        public string ScoreAfterNRecipes(int n)
        {
            while (seq.Count < n + 10)
            {
                Next();
            }
            return string.Join("", seq.Skip(n).Take(10));
        }

        public int CountRecipesBeforeScore(string score)
        {
            var idx = -1;
            do
            {
                Next();
                var s = string.Join("", seq.Skip(seq.Count - score.Length - 1));
                if (s.Contains(score))
                    idx = string.Join("", seq).IndexOf(score);
            } while (idx < 0);
            return idx;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var rb = new Recipes();

            //var rest1p1 = rb.ScoreAfterNRecipes(5);
            //Console.WriteLine($"{rest1p1} == 0124515891: {rest1p1 == "0124515891"}");

            //var rest2p1 = rb.ScoreAfterNRecipes(9);
            //Console.WriteLine($"{rest2p1} == 5158916779: {rest2p1 == "5158916779"}");

            //var rest3p1 = rb.ScoreAfterNRecipes(18);
            //Console.WriteLine($"{rest3p1} == 9251071085: {rest3p1 == "9251071085"}");

            //var rest4p1 = rb.ScoreAfterNRecipes(2018);
            //Console.WriteLine($"{rest4p1} == 5941429882: {rest4p1 == "5941429882"}");

            var res1 = rb.ScoreAfterNRecipes(190221);
            Console.WriteLine($"{res1}"); //1191216109

            rb = new Recipes();

            //var rest1p2 = rb.CountRecipesBeforeScore("01245");
            //Console.WriteLine($"{rest1p2}: {rest1p2 == 5}");

            //var rest2p2 = rb.CountRecipesBeforeScore("51589");
            //Console.WriteLine($"{rest2p2}: {rest2p2 == 9}");

            //var rest3p2 = rb.CountRecipesBeforeScore("92510");
            //Console.WriteLine($"{rest3p2}: {rest3p2 == 18}");

            //var rest4p2 = rb.CountRecipesBeforeScore("59414");
            //Console.WriteLine($"{rest4p2}: {rest4p2 == 2018}");

            var res2 = rb.CountRecipesBeforeScore("190221");
            Console.WriteLine($"{res2}"); //20268576
        }
    }
}
