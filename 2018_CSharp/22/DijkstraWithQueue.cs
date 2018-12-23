using System;
using System.Collections.Generic;
using System.Linq;
using PriorityQueueLib;

namespace GraphLib
{
    public static class DijkstraQueueExt
    {
        public static List<Node> DijkstraWithQueue(this Graph graph,
            Dictionary<Edge, double> weights,
            Node start,
            Node end,
            IPriorityQueue<Node> queue)
        {
            var track = new Dictionary<Node, Node> {[start] = null};
            queue.Add(start, 0);

            while(true)
            {
                var (toOpen, bestPrice) = queue.ExtractMin();
                if (toOpen == end) break;

                foreach(var edge in toOpen.IncidentEdges.Where(z => z.From == toOpen))
                {
                    var currentPrice = bestPrice + weights[edge];
                    var nextNode = edge.OtherNode(toOpen);
                    if (queue.UpdateOrAdd(nextNode,currentPrice))
                        track[nextNode] = toOpen;
                }
            }

            var result = new List<Node>();
            while(end != null)
            {
                result.Add(end);
                end = track[end];
            }
            result.Reverse();
            return result;
        }
    }
}