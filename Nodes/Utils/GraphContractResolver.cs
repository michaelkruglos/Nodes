using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Nodes.Models;

namespace Nodes.Utils
{
    internal class GraphContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (objectType == typeof(Graph))
            {
                contract.Converter = new GraphConverter();
            }
            return contract;
        }
    }
}
