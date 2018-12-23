using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLib
{
    public struct DijkstraData
    {
        public Node Previous { get; set; }
        public double Price { get; set; }
    }

    public static class DijkstraExt
    {
        public static List<Node> Dijkstra(this Graph graph, Dictionary<Edge, double> weights, Node start, Node end)
        {
            var notVisited = graph.Nodes.ToList();
            var track = new Dictionary<Node, DijkstraData>
            {
                [start] = new DijkstraData {Price = 0, Previous = null}
            };


            while(true)
            {
                Node toOpen = null;
                var bestPrice = double.PositiveInfinity;

                foreach(var node in notVisited)
                {
                    if (!track.ContainsKey(node) || !(track[node].Price < bestPrice)) continue;
                    bestPrice = track[node].Price;
                    toOpen = node;
                }
            
                if (toOpen == null) return null;
                if (toOpen == end) break;

                foreach(var edge in toOpen.IncidentEdges.Where(z => z.From == toOpen))
                {
                    var currentPrice = track[toOpen].Price + weights[edge];
                    var nextNode = edge.OtherNode(toOpen);
                    if (!track.ContainsKey(nextNode) || track[nextNode].Price > currentPrice)
                        track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice};
                }
            
                notVisited.Remove(toOpen);
            }

            var result = new List<Node>();
            while(end != null)
            {
                result.Add(end);
                end = track[end].Previous;
            }
            result.Reverse();
            return result;
        }
    }
}