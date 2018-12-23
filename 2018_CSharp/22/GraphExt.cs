using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace GraphLib
{
    public static class NodeExtensions
    {
        public static IEnumerable<Node> DepthSearch(Node startNode)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            visited.Add(startNode);
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                var node = stack.Pop();
                yield return node;
                foreach (var nextNode in node.IncidentNodes.Where(n => !visited.Contains(n)))
                {
                    visited.Add(nextNode);
                    stack.Push(nextNode);
                }
            }
        }

        public static IEnumerable<Node> BreadthSearch(Node startNode)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            visited.Add(startNode);
            queue.Enqueue(startNode);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                yield return node;
                foreach (var nextNode in node.IncidentNodes.Where(n => !visited.Contains(n)))
                {
                    visited.Add(nextNode);
                    queue.Enqueue(nextNode);
                }

            }
        }
    }
}