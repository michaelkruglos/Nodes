using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nodes.Models
{
    public class Graph
    {
        public Graph()
        {

        }

        [JsonRequired]
        public ISet<Edge> Edges { get; set; } = new HashSet<Edge>();
        [JsonRequired]
        public ISet<Node> Nodes { get; set; } = new HashSet<Node>();

        public void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Nodes.Add(node);
        }

        public IList<Edge> RemoveNode(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            var removed = Nodes.Remove(node);
            var removedEdges = new List<Edge>();
            if (removed)
            {
                foreach (var edge in Edges)
                {
                    if (edge.From == node || edge.To == node)
                    {
                        removedEdges.Add(edge);
                        Edges.Remove(edge);
                    }
                }
            }

            return removedEdges;
        }

        public void AddEdge(Edge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            if (edge.From == null) throw new ArgumentNullException(nameof(edge.From));
            if (edge.To == null) throw new ArgumentNullException(nameof(edge.To));

            if (!Nodes.Contains(edge.From))
            {
                AddNode(edge.From);
            }
            if (!Nodes.Contains(edge.To))
            {
                AddNode(edge.To);
            }
            Edges.Add(edge);
        }

        public bool RemoveEdge(Edge edge)
        {
            if (edge == null) throw new ArgumentNullException(nameof(edge));
            return Edges.Remove(edge);
        }

        public Edge? FindEdgeBetweenNodes(Node n1, Node n2)
        {
            if (n1 == null) throw new ArgumentNullException(nameof(n1));
            if (n2 == null) throw new ArgumentNullException(nameof(n2));
            foreach (var edge in Edges)
            {
                if ((edge.From == n1 && edge.To == n2) || (edge.From == n2 && edge.To == n1))
                {
                    return edge;
                }
            }
            return null;
        }
    }
}
