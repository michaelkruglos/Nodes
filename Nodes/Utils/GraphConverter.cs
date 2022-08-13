using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nodes.Models;

namespace Nodes.Utils
{
    internal class GraphConverter : JsonConverter<Graph>
    {
        public override Graph? ReadJson(JsonReader reader, Type objectType, Graph? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var exception = new Exception("Invalid Graph Object");
            var graph = new Graph();
            var graphObject = (JObject)JToken.ReadFrom(reader) ?? throw exception;
            var nodesArray = graphObject?.Property("nodes")?.Value as JArray ?? throw exception;
            var edgesArray = graphObject?.Property("edges")?.Value as JArray ?? throw exception;
            graph.Nodes = new HashSet<Node>(nodesArray.Select(x =>
            {
                var node = (JObject)x;
                var id = node.Property("id")?.Value.Value<int>() ?? throw exception;
                var name = node.Property("name")?.Value.Value<string>() ?? throw exception;
                var position = Point.Parse(node.Property("position")?.Value.Value<string>() ?? throw exception);
                return new Node(name!) { Id = id, Position = position };
            }));

            graph.Edges = new HashSet<Edge>(edgesArray.Select(x =>
            {
                var edge = (JObject)x;
                var fromId = edge.Property("from")?.Value.Value<uint>() ?? throw exception;
                var toId = edge.Property("to")?.Value.Value<uint>() ?? throw exception;
                var from = graph.Nodes.Single(x => x.Id == fromId);
                var to = graph.Nodes.Single(x => x.Id == toId);
                return new Edge(from, to);
            }));

            return graph;
        }

        public override void WriteJson(JsonWriter writer, Graph? value, JsonSerializer serializer)
        {
            if (value is null) return;
            JArray nodesArray = new();
            JArray edgesArray = new();
            var graph = value;

            foreach (var n in graph.Nodes)
            {
                nodesArray.Add(new JObject
                {
                    { "id", n.Id },
                    { "name", n.Name },
                    { "position", n.Position.ToString() }
                });
            }

            foreach (var e in graph.Edges)
            {
                edgesArray.Add(new JObject
                {
                    { "from", e.From.Id },
                    { "to", e.To.Id }
                });
            }

            new JObject
            {
                { "nodes", nodesArray },
                { "edges", edgesArray },
            }.WriteTo(writer);
        }
    }
}