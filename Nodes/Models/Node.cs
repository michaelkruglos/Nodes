using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Nodes.Models
{
    public class Node
    {
        public Node(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Point Position { get; set; }
    }
}
