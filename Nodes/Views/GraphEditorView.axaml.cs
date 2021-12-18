using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Nodes.Models;
using Nodes.ViewModels;
using ReactiveUI;

namespace Nodes.Views
{
    public partial class GraphEditorView : Canvas
    {
        public static DirectProperty<GraphEditorView, Graph> GraphProperty =
            AvaloniaProperty.RegisterDirect<GraphEditorView, Graph>(nameof(Graph),
                g => g.Graph, (g,v)=> g.Graph = v);
        
        private Graph _graph = new();
        private IDictionary<Node, NodeView> _nodesToViews = new Dictionary<Node, NodeView>();
        private IDictionary<Edge, EdgeView> _edgesToViews = new Dictionary<Edge, EdgeView>();


        public Graph Graph
        {
            get => _graph;
            set
            {
                SetAndRaise(GraphProperty, ref _graph, value ?? new Graph());
                UpdateChildren();
            }
        }

        public ICommand AddNode { get; private set; }
        public ICommand RemoveNode { get; private set; }
        public ICommand ConnectNodes { get; private set; }
        public ICommand DisconnectNodes { get; private set; }

        static GraphEditorView()
        {
            AffectsRender<GraphEditorView>(GraphProperty);
        }

        public AvaloniaList<NodeView> SelectedNodes { get; private set; } = new AvaloniaList<NodeView>();

        public GraphEditorView()
        {
            InitializeComponent();
            AddNode = ReactiveCommand.Create(AddNodeCommand);
            RemoveNode = ReactiveCommand.Create(RemoveNodeCommand, SelectedNodes
                .ObservableForProperty(s => s.Count)
                .Select(c => c.Value > 0));
            ConnectNodes = ReactiveCommand.Create(ConnectNodesCommand, SelectedNodes
                .ObservableForProperty(s=> s.Count).Select(c =>
                {
                    var onlyTwoNodes = c.Value == 2;
                    if (!onlyTwoNodes) return false;
                    var fromNode = ((NodeViewModel)SelectedNodes[0].DataContext!).Node;
                    var toNode = ((NodeViewModel)SelectedNodes[1].DataContext!).Node;
                    return Graph.FindEdgeBetweenNodes(fromNode, toNode) == null;
                }));
            DisconnectNodes = ReactiveCommand.Create(DisconnectNodesCommand, SelectedNodes
                .ObservableForProperty(s=> s.Count).Select(c =>
                {
                    var onlyTwoNodes = c.Value == 2;
                    if (!onlyTwoNodes) return false;
                    var fromNode = ((NodeViewModel)SelectedNodes[0].DataContext!).Node;
                    var toNode = ((NodeViewModel)SelectedNodes[1].DataContext!).Node;
                    return Graph.FindEdgeBetweenNodes(fromNode, toNode) != null;
                }));
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            FocusManager.Instance.Focus(null);
            HandleSelection(e);
        }

        private void HandleSelection(PointerPressedEventArgs e)
        {
            if (e.Source is NodeView nv && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var modifier = e.KeyModifiers == KeyModifiers.Shift;
                if (SelectedNodes.Count == 0 || (modifier && !nv.IsSelected))
                {
                    SelectNode(nv);
                    return;
                }
                if (modifier && nv.IsSelected)
                {
                    UnselectNode(nv);
                    return;
                }
                ClearSelectedNodes();
                if (!nv.IsSelected)
                {
                    SelectNode(nv);
                }
                return;
            }
            ClearSelectedNodes();
        }

        private void UnselectNode(NodeView nv)
        {
            SelectedNodes.Remove(nv);
            nv.SetValue(NodeView.IsSelectedProperty, false);
        }

        private void SelectNode(NodeView nv)
        {
            SelectedNodes.Add(nv);
            nv.SetValue(NodeView.IsSelectedProperty, true);
        }

        private void ClearSelectedNodes()
        {
            foreach (var node in SelectedNodes)
            {
                node.SetValue(NodeView.IsSelectedProperty, false);
            }
            SelectedNodes.Clear();
        }

        private void AddNodeCommand()
        {
            var node = new Node("New Node") { Position = new Point(Bounds.Width / 2, Bounds.Height / 2) };
            Graph.AddNode(node);
            Children.Add(new NodeView { DataContext = new NodeViewModel(node) });
        }
        private void RemoveNodeCommand()
        {
            foreach (var node in SelectedNodes)
            {
                if (node.DataContext is NodeViewModel vm)
                {
                    var removeEdges = Graph.RemoveNode(vm.Node);
                    foreach (var edge in removeEdges) Children.Remove(_edgesToViews[edge]);
                }
                Children.Remove(node);
            }
        }

        private void ConnectNodesCommand()
        {
            var fromView = SelectedNodes[0];
            var toView = SelectedNodes[1];
            var fromNode = ((NodeViewModel)fromView.DataContext!).Node;
            var toNode = ((NodeViewModel)toView.DataContext!).Node;
            var edge = new Edge(fromNode, toNode);
            Graph.AddEdge(edge);
            var edgeControl = MakeEdgeControl(edge);
            edgeControl.SetValue(EdgeView.FromProperty, CreateFromPointForNode(fromView));
            edgeControl.SetValue(EdgeView.ToProperty, CreateToPointForNode(toView));
            Children.Add(edgeControl);
            SetupAttachmentPointsTracking(edgeControl, fromView, toView);
            _edgesToViews.Add(edge, edgeControl);
        }
        private void DisconnectNodesCommand()
        {
            var view1 = SelectedNodes[0];
            var view2 = SelectedNodes[1];
            var node1 = ((NodeViewModel)view1.DataContext!).Node;
            var node2 = ((NodeViewModel)view2.DataContext!).Node;
            var edge = Graph.FindEdgeBetweenNodes(node1, node2);
            if (edge == null) return;
            Graph.RemoveEdge(edge);
            var edgeView = _edgesToViews[edge];
            Children.Remove(edgeView);
            _edgesToViews.Remove(edge);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateChildren()
        {
            Children.Clear();
            _nodesToViews = _graph.Nodes.ToDictionary(n => n, n => MakeNodeControl(n));
            _edgesToViews = _graph.Edges.ToDictionary(e => e, e => MakeEdgeControl(e));
            Children.AddRange(_nodesToViews.Values);
            Children.AddRange(_edgesToViews.Values);

            foreach(var (edge, edgeCtrl) in _edgesToViews)
            {
                var from = _nodesToViews[edge.From];
                var to = _nodesToViews[edge.To];
                SetupAttachmentPointsTracking(edgeCtrl, from, to);
            }
        }

        private static void SetupAttachmentPointsTracking(EdgeView edgeCtrl, NodeView from, NodeView to)
        {
            from.GetObservable(BoundsProperty).Subscribe(x => UpdateEdgeAttachmentPoints(edgeCtrl, from, to));
            to.GetObservable(BoundsProperty).Subscribe(x => UpdateEdgeAttachmentPoints(edgeCtrl, from, to));
        }

        private static void UpdateEdgeAttachmentPoints(EdgeView edgeCtrl, NodeView from, NodeView to)
        {
            Point fromPoint = CreateFromPointForNode(from);
            Point toPoint = CreateToPointForNode(to);
            edgeCtrl.SetValue(EdgeView.FromProperty, fromPoint);
            edgeCtrl.SetValue(EdgeView.ToProperty, toPoint);
            edgeCtrl.SetValue(EdgeView.ArrowLengthProperty, 10); // TODO: make this a constant
            edgeCtrl.InvalidateVisual();
        }

        private static Point CreateToPointForNode(NodeView to)
        {
            return new Point(to.Bounds.Left + to.Bounds.Width / 2, to.Bounds.Top);
        }

        private static Point CreateFromPointForNode(NodeView from)
        {
            return new Point(from.Bounds.Left + from.Bounds.Width / 2, from.Bounds.Bottom);
        }

        private NodeView MakeNodeControl(Node node)
        {
            var control = new NodeView { DataContext = new NodeViewModel(node) };
            //control.GetObservable(NodeView.IsSelectedProperty)
            //    .Where(selected => selected)
            //    .Subscribe(_ => SelectedNodes.Add(control));
            return control;
        }

        private EdgeView MakeEdgeControl(Edge edge)
        {
            var control = new EdgeView { DataContext = new EdgeViewModel(edge) };
            return control;
        }
    }
}
