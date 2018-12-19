using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _04
{
    static class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            
            var regexp = new Regex(@"\[(?<date>[\d-: ]+)] (?:(?<awake>wakes up)|(?<asleep>falls asleep)|(?:Guard #(?<id>\d+) begins shift))", RegexOptions.Compiled);

            var events = regexp.Matches(input)
                .OfType<Match>()
                .Select(match =>
                {
                    var dt = DateTime.Parse(match.Groups["date"].Value);
                    var id = match.Groups["id"].Success ? int.Parse(match.Groups["id"].Value) : 0;
                    var sleep = match.Groups["asleep"].Success;
                    return (dt, id, sleep);
                })
                .OrderBy(e => e.dt)
                .ToArray();
            var gi = new List<(int id, IEnumerable<int> interval)>();
            var asleepev = (dt: default(DateTime), id: 0, sleep: false);
            var curid = 0;
            //foreach (var i in events.Take(10))
            //    Console.WriteLine($"{i.dt.ToString("dd.yy.yyyy HH:mm")} => {i.id} : {(i.sleep ? "sleep" : i.id == 0 ? "awake" : "shift")}");
            foreach (var ev in events)
            {
                if (ev.id != 0)
                    curid = ev.id;
                else if (ev.sleep)
                {
                    asleepev = ev;
                }
                else
                {
                    var awakeev = ev;
                    var diff = (awakeev.dt - asleepev.dt).Minutes;
                    var startmin = asleepev.dt.Minute;
                    gi.Add((curid, Enumerable.Range(startmin, diff)));
                }
            }

            var dict = gi.GroupBy(pair => pair.id).ToDictionary(p => p.Key, i => i.SelectMany(p => p.interval));
            var gcnt = dict.Select(kvp => (kvp.Key, kvp.Value.Count()));
            var mgcnt = gcnt.OrderByDescending(p => p.Item2).First();
            var minint = dict[mgcnt.Key].GroupBy(x => x).Select(g => (g.Key, g.Count())).OrderByDescending(kvp => kvp.Item2).First();
            var res1 = minint.Key * mgcnt.Key;

            Console.WriteLine(res1);

            var gcntm = dict.Select(kvp => (kvp.Key, kvp.Value.GroupBy(id => id).Select(g => (g.Key, g.Count())).OrderByDescending(g => g.Item2).First()));
            var maxmin = gcntm.OrderByDescending(p => p.Item2.Item2).First();
            var res2 = maxmin.Key * maxmin.Item2.Key;

            Console.WriteLine(res2);
        }
    }
}
