using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using Avalonia;
using Nodes.Models;
using ReactiveUI;

namespace Nodes.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private Graph _graph;

        public MainWindowViewModel()
        {
            // For testing
            var graph = new Graph();
            var node1 = new Models.Node("Some cool node") { Position = new Point(10, 10) };
            var node2 = new Models.Node("Some other cool node") { Position = new Point(300, 30) };
            graph.AddEdge(new Edge(node1, node2));
            _graph = graph;
        }

        public Graph Graph 
        { 
            get => _graph;
            set
            {
                this.RaiseAndSetIfChanged(ref _graph, value);
            }
        }

    }
}
