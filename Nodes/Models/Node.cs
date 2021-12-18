using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Nodes.Models
{
    public class Node
    {
        private static readonly RNGCryptoServiceProvider  prng = new();

        public Node(string name)
        {
            Name = name;
            var bytes = new byte[sizeof(uint)];
            prng.GetBytes(bytes);
            Id = BitConverter.ToUInt32(bytes);
        }

        public string Name { get; set; }
        public Point Position { get; set; }
        public uint Id { get; set; }
    }
}
