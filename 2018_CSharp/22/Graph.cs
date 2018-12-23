using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLib
{
    public class Node
    {
        protected readonly List<Edge> edges = new List<Edge>();
        public readonly int NodeNumber;

        public Node(int number)
        {
            NodeNumber = number;
        }

        public IEnumerable<Node> IncidentNodes => edges.Select(e => e.OtherNode(this));

        public IEnumerable<Edge> IncidentEdges => edges.Select(e => e);

        public static Edge Connect(Node node1, Node node2, Graph graph)
        {
            if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
            var edge = new Edge(node1, node2);
            node1.edges.Add(edge);
            node2.edges.Add(edge);
            return edge;
        }

        public static void Disconnect(Edge edge)
        {
            edge.From.edges.Remove(edge);
            edge.To.edges.Remove(edge);
        }
    }

    public class Edge
    {
        public readonly Node From;
        public readonly Node To;

        public Edge(Node from, Node to)
        {
            this.From = from;
            this.To = to;
        }

        public bool IsIncident(Node node)
        {
            return From == node || To == node;
        }

        public Node OtherNode(Node node)
        {
            if (!IsIncident(node)) throw new ArgumentException();
            return From == node ? To : From;
        }
    }

    public class Graph
    {
        private readonly Node[] nodes;

        public Graph(int count)
        {
            nodes = Enumerable.Range(0, count).Select(n => new Node(n)).ToArray();
        }

        public int Length => nodes.Length;

        public Node this[int idx] => nodes[idx];

        public IEnumerable<Node> Nodes => nodes.Select(n => n);

        public Edge Connect(int index1, int index2)
        {
            return Node.Connect(nodes[index1], nodes[index2], this);
        }

        public void Delete(Edge edge)
        {
            Node.Disconnect(edge);
        }

        public IEnumerable<Edge> Edges => nodes.SelectMany(z => z.IncidentEdges).Distinct();

        public static Graph MakeGraph(params int[] incidentNodes)
        {
            var graph = new Graph(incidentNodes.Max() + 1);
            for (var i = 0; i < incidentNodes.Length - 1; i += 2)
                graph.Connect(incidentNodes[i], incidentNodes[i + 1]);
            return graph;
        }

        
    }
}
