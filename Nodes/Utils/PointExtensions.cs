using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Nodes.Utils
{
    internal static class PointExtensions
    {
        public static double Length(this Point point)
        {
            return Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        public static double DotProduct(this Point p1, Point p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        public static double Angle(this Point p1)
        {
            return Math.Atan(p1.Y/p1.X);
        }

        public static Point Normalized(this Point p)
        {
            return p / p.Length();
        }

        public static Point Rotated(this Point p, double angle)
        {
            return new Point(p.X * Math.Cos(angle) - p.Y * Math.Sin(angle),  p.X * Math.Sin(angle) + p.Y * Math.Cos(angle));
        }
    }
}
