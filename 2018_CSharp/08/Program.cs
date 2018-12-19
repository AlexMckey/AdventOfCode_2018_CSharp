using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _08
{
    class Program
    {
        static IEnumerable<int> CollectMetaData(IEnumerable<int> source, Stack<int> childStack, Stack<int> metaStack, IEnumerable<int> acc)
        {
            if (source.Count() == 0) return acc;
            var childCnt = childStack.Pop();
            if (childCnt == 0)
            {
                var cntMeta = metaStack.Pop();
                var meta = source.Take(cntMeta);
                return CollectMetaData(source.Skip(cntMeta), childStack, metaStack, acc.Concat(meta));
            }
            else
            {
                childStack.Push(childCnt - 1);
                var cntChild = source.First();
                var cntMeta = source.Skip(1).First();
                childStack.Push(cntChild);
                metaStack.Push(cntMeta);
                return CollectMetaData(source.Skip(2), childStack, metaStack, acc);
            }
        }

        static int CollectRootData(IEnumerable<int> source, Stack<int> childStack, Stack<int> metaStack, Stack<IEnumerable<int>> acc)
        {
            if (source.Count() == 0) return acc.Pop().FirstOrDefault();
            var childCnt = childStack.Pop();
            if (childCnt == 0)
            {
                var cntMeta = metaStack.Pop();
                var metaIdxs = source.Take(cntMeta);
                var metaData = acc.Pop();
                var meta = metaIdxs.Select(idx => metaData.ElementAtOrDefault(idx - 1)).Sum();
                var newMeta = acc.Pop().Append(meta);
                acc.Push(newMeta);
                return CollectRootData(source.Skip(cntMeta), childStack, metaStack, acc);
            }
            else
            {
                childStack.Push(childCnt - 1);
                var cntChild = source.First();
                var cntMeta = source.Skip(1).First();
                if (cntChild == 0)
                {
                    var meta = source.Skip(2).Take(cntMeta).Sum();
                    var metaData = acc.Pop();
                    acc.Push(metaData.Append(meta));
                    return CollectRootData(source.Skip(2 + cntMeta), childStack, metaStack, acc);
                }
                else
                {
                    childStack.Push(cntChild);
                    metaStack.Push(cntMeta);
                    acc.Push(Enumerable.Empty<int>());
                    return CollectRootData(source.Skip(2), childStack, metaStack, acc);
                }
            }
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");

            var testInput = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2";

            var source = input.Split(' ').Select(i => int.Parse(i)).ToList();

            var metaStack = new Stack<int>();
            var childStack = new Stack<int>();
            childStack.Push(1);

            var meta = CollectMetaData(source, childStack, metaStack, Enumerable.Empty<int>());

            var res1 = meta.Sum();

            Console.WriteLine($"{res1}"); //40984

            metaStack.Clear();
            childStack.Clear();
            childStack.Push(1);
            var acc = new Stack<IEnumerable<int>>();
            acc.Push(Enumerable.Empty<int>());

            var res2 = CollectRootData(source, childStack, metaStack, acc);

            Console.WriteLine($"{res2}"); //37067
        }
    }
}