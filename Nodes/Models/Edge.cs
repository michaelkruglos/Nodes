using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodes.Models
{
    public class Edge
    {
        public Edge(Node from, Node to)
        {
            From = from;
            To = to;
        }

        public Node From { get; set; }
        public Node To { get; set; }
    }
}
