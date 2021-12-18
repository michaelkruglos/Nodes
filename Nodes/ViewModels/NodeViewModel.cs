using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Nodes.Models;
using ReactiveUI;

namespace Nodes.ViewModels
{
    public class NodeViewModel : ViewModelBase
    {
        private Node _node;

        public Node Node => _node;
        public bool IsDragging { get; set; }

        public bool IsSelected { get; set; }

        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                this.RaiseAndSetIfChanged(ref _isEditing, value);
            }
        }

        private string _name;
        public string Name
        {
            get => _node.Name;
            set
            {
                _node.Name = value;
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        private Point _position;
        public Point Position
        {
            get => _node.Position;
            set
            {
                this.RaiseAndSetIfChanged(ref _position, value);
                _node.Position = _position;
            }
        }

        public NodeViewModel()
        {
            _node = new Node("Test Node");
            _name = _node.Name;
        }

        public NodeViewModel(Node n)
        {
            _node = n;
            _name = _node.Name;
        }

    }
}
