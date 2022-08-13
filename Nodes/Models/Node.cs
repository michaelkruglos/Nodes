using System;
using System.Security.Cryptography;
using Avalonia;

namespace Nodes.Models
{
    public class Node
    {
        public Node(string name)
        {
            Name = name;
            Id = RandomNumberGenerator.GetInt32(int.MaxValue);
        }

        public string Name { get; set; }
        public Point Position { get; set; }
        public int Id { get; set; }
    }
}
