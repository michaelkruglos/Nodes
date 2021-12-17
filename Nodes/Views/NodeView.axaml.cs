using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nodes.ViewModels;
using ReactiveUI;

namespace Nodes.Views
{
    public partial class NodeView : UserControl
    {
        public static DirectProperty<NodeView,bool> IsSelectedProperty = AvaloniaProperty.RegisterDirect<NodeView, bool>(nameof(IsSelected),
            o => o.IsSelected, (o,v) => o.IsSelected = v);

        private bool _isSelected;

        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                SetAndRaise(IsSelectedProperty, ref _isSelected, value);
                BorderThickness = value ? new Thickness(3) : new Thickness(2);
            }
        }

        public NodeView()
        {
            InitializeComponent();
            _ = this.GetObservable(DataContextProperty)
                .Select(x => (x as NodeViewModel)?.Node)
                .Subscribe(n =>
                {
                    SetValue(Canvas.LeftProperty, n?.Position.X ?? 0);
                    SetValue(Canvas.TopProperty, n?.Position.Y ?? 0);
                });
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            var input = this.FindControl<TextBox>("input");
            var vm = (DataContext as NodeViewModel)!;
            DoubleTapped += (sender, e) =>
            {
                vm.IsEditing = true;
                input.Focus();
            };

            input.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    vm.Name = input.Text;
                    vm.IsEditing = false;
                }
            };
            input.LostFocus += (sender, e) =>
            {
                vm.IsEditing = false;
            };
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (DataContext is NodeViewModel vm && vm.IsDragging)
            {
                var position = e.GetCurrentPoint(Parent).Position;
                var left = position.X - Bounds.Width / 2;
                var top = position.Y - Bounds.Height / 2;
                if (left < 0) left = 0;
                if (top < 0) top = 0;
                var parentWidth = Parent!.Bounds.Width;
                var parentHeight = Parent!.Bounds.Height;
                if (left >= parentWidth - Bounds.Width) left = parentWidth - Bounds.Width;
                if (top >= parentHeight - Bounds.Height) top = parentHeight - Bounds.Height;
                
                vm.Position = new Point(left, top);
            }

        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (DataContext is NodeViewModel vm && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                vm.IsDragging = true;
                e.Source = this;
            }
        }


        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (DataContext is NodeViewModel vm)
            {
                vm.IsDragging = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
