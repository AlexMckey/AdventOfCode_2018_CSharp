using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _09
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var test0 = "9 players; last marble is worth 25 points: high score is 32";
            var test1 = "10 players; last marble is worth 1618 points: high score is 8317";
            var test2 = "13 players; last marble is worth 7999 points: high score is 146373";
            var test3 = "17 players; last marble is worth 1104 points: high score is 2764";
            var test4 = "21 players; last marble is worth 6111 points: high score is 54718";
            var test5 = "30 players; last marble is worth 5807 points: high score is 37305";

            var r = new Regex(@"(?<pcnt>\d+) players;.+ (?<lastp>\d+) points(?:\:.* is (?<maxscore>\d+))?", RegexOptions.Compiled);

            var mtchs = r.Matches(input).First();
            var cntPlayer = int.Parse(mtchs.Groups["pcnt"].Value);
            var lastPoints = int.Parse(mtchs.Groups["lastp"].Value);
            var testScore = 0;
            if (mtchs.Groups["maxscore"].Success)
                testScore = int.Parse(mtchs.Groups["maxscore"].Value);

            var game = new MarbleGameCircle(cntPlayer);
            game.Go(lastPoints);

            var res1 = game.MaxScore;

            Console.WriteLine($"{res1}"); // 424639
            //Console.WriteLine(res1 == testScore);
            //Console.WriteLine($"{game.WinnerScore}");

            game = new MarbleGameCircle(cntPlayer);
            game.Go(lastPoints*100);

            var res2 = game.MaxScore;

            Console.WriteLine($"{res2}"); // 3516007333
        }
    }
}