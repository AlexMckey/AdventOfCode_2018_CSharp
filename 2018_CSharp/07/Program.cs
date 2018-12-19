using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _07
{
    class Worker
    {
        private int timeToComplite;

        public Worker()
        {
            Step = '.';
            timeToComplite = 0;
        }

        public void TryInit()
        {
            if (IsEmpty)
            {
                if (Step != '.') resultStore.Append(Step);
                var goNodes = workflowDB.Where(kvp => kvp.Value.inbox.Count() == 0 && !kvp.Value.islock);
                if (goNodes.Count() != 0)
                {
                    Step = goNodes.Min(kvp => kvp.Key);
                    workflowDB[Step] = (inbox: workflowDB[Step].inbox,
                                       outbox: workflowDB[Step].outbox,
                                       islock: true);
                    timeToComplite = StepCount(Step);
                }
                else Step = '.';
            }
        }

        public void TryComplite()
        {
            if (IsEmpty) return;
            timeToComplite--;
            if (IsEmpty && Step != '.')
            {
                var nodeToVisit = workflowDB[Step].outbox.OrderBy(v => v);
                workflowDB.Remove(Step);
                foreach (var n in nodeToVisit)
                    workflowDB[n].inbox.Remove(Step);
            }
        }

        public bool IsEmpty => timeToComplite == 0;

        public char Step { get; private set; }

        const int BaseStepTime = 60;

        public static Dictionary<char, (HashSet<char> inbox,HashSet<char> outbox, bool islock)> workflowDB;
        public static StringBuilder resultStore;

        static public int StepCount(char node)
        {
            return BaseStepTime + node - 'A' + 1;
        }

    }

    class Program
    {
        static void PrintOneStepOfLoop(int curStep, Worker[] workers, string result, bool printHeader = false)
        {
            var sb = new StringBuilder();
            if (printHeader)
            {
                sb.Append("Second\t");
                var i = 1;
                foreach (var w in workers)
                    sb.Append($"Work_{i++}\t");
                sb.Append("Result");
                Console.WriteLine(sb.ToString());
            }
            else
            {
                if (curStep < 10) sb.Append($"    {curStep}");
                else if (curStep >= 100) sb.Append($"  {curStep}");
                else sb.Append($"   {curStep}");
                sb.Append('\t');
                foreach (var w in workers)
                    sb.Append($"   {w.Step}\t");
                sb.Append(result);
                Console.WriteLine(sb.ToString());
            }
        }

        static void Main(string[] args)
        {
            var inputStr = @"Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.";//.Split("\r\n");

            var input = File.ReadAllText("input.txt");

            var regexp = new Regex(@"Step (?<from>[A-Z]).+(?<to>[A-Z]).+", RegexOptions.Compiled);

            var edges = regexp.Matches(input)
                .OfType<Match>()
                .Select(match => (from: match.Groups["from"].Value[0],
                                    to: match.Groups["to"].Value[0]))
                .ToList();
            var nodes = edges
                .SelectMany(e => new List<char>() { e.from, e.to })
                .Distinct()
                .ToList();

            var dict = nodes
                .ToDictionary(n => n,
                    n => (inbox: edges.Where(e => e.to == n).Select(e => e.from).ToHashSet(),
                         outbox: edges.Where(e => e.from == n).Select(e => e.to).ToHashSet()));

            var sb = new StringBuilder();
            var isComplite = false;
            
            // loop
            do {
                var startNodes = dict.Where(kvp => kvp.Value.inbox.Count() == 0);
                if (startNodes.Count() == 0)
                    isComplite = true;
                else
                {
                    var goNode = startNodes.Min(kvp => kvp.Key);
                    sb.Append(goNode);
                    var nodeToVisit = dict[goNode].outbox.OrderBy(v => v);
                    dict.Remove(goNode);
                    foreach (var n in nodeToVisit)
                        dict[n].inbox.Remove(goNode);
                }
            } while (!isComplite);

            var res1 = sb.ToString();

            Console.WriteLine($"{res1}"); // IBJTUWGFKDNVEYAHOMPCQRLSZX

            var dict2 = nodes
                .ToDictionary(n => n,
                    n => (inbox: edges.Where(e => e.to == n).Select(e => e.from).ToHashSet(),
                         outbox: edges.Where(e => e.from == n).Select(e => e.to).ToHashSet(),
                         islock: false));

            
            var workerCount = 5;
            var workers = new Worker[workerCount];
            for (var i = 0; i < workerCount; i++)
                workers[i] = new Worker();
            sb.Clear();
            isComplite = false;
            var curStep = 0;
            Worker.workflowDB = dict2;
            Worker.resultStore = sb;

            //PrintOneStepOfLoop(curStep, workers, sb.ToString(), true);
            // loop
            do
            {
                foreach (var w in workers)
                    w.TryInit();
                var emptyWorkersCnt = workers.Count(w => w.IsEmpty);
                if (emptyWorkersCnt == workerCount)
                    isComplite = true;
                else
                {
                    foreach (var w in workers)
                        w.TryComplite();
                }
                //PrintOneStepOfLoop(curStep, workers, sb.ToString());
                curStep++;
            } while (!isComplite);

            var resStr = sb.ToString();
            var res2 = curStep - 1;

            Console.WriteLine($"{res2} : {resStr}"); // 1118 : ITUWBJGFKDNVYAEHQOMPCRLSZX
        }
    }
}