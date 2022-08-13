using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Nodes.Utils;

namespace Nodes.Views
{
    public partial class EdgeView : Canvas
    {
        public static DirectProperty<EdgeView, Point> FromProperty =
            AvaloniaProperty.RegisterDirect<EdgeView, Point>(nameof(From), e=>e.From, (e,v) => e.From = v);

        public static DirectProperty<EdgeView, Point> ToProperty =
            AvaloniaProperty.RegisterDirect<EdgeView, Point>(nameof(To), e=>e.To, (e,v) => e.To = v);

        public static DirectProperty<EdgeView, double> ArrowLengthProperty =
            AvaloniaProperty.RegisterDirect<EdgeView, double>(nameof(ArrowLength), a=>a.ArrowLength, (a,v) => a.ArrowLength = v);

        static EdgeView()
        {
            AffectsRender<EdgeView>(FromProperty, ToProperty, ArrowLengthProperty);
        }

        private Point _from;
        public Point From
        {
            get => _from;
            set => SetAndRaise(FromProperty, ref _from, value);
        }

        private Point _to;
        public Point To 
        {
            get => _to;
            set => SetAndRaise(ToProperty, ref _to, value);
        }

        private double _ArrowLength;
        public double ArrowLength
        {
            get => _ArrowLength;
            set => SetAndRaise(ArrowLengthProperty, ref _ArrowLength, value);
        }

        public EdgeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Render(DrawingContext context)
        {
            var pen = new Pen(Brushes.Black) { Thickness = 2 };
            var from = GetValue(FromProperty);
            var to = GetValue(ToProperty);
            context.DrawLine(pen, from, to);
            DrawArrow(context);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            Console.WriteLine("arrow clicked");
        }

        private void DrawArrow(DrawingContext context)
        {
            var pen = new Pen(Brushes.Black) { Thickness = 2 };
            var from = GetValue(FromProperty);
            var to = GetValue(ToProperty);
            var arrowLength = GetValue(ArrowLengthProperty);
            var vector = (to - from).Normalized();
            Point arrow1 = vector.Rotated(Math.PI/6) * arrowLength;
            context.DrawLine(pen, to, to - arrow1);
            Point arrow2 = vector.Rotated(-Math.PI/6) * arrowLength;
            context.DrawLine(pen, to, to - arrow2);
        }
    }
}
