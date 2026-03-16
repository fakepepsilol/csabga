using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    public class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Vector2()
        {
            
        }
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Vector2(Vector2 other)
        {
            X = other.X;
            Y = other.Y;
        }
        public Vector2(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 operator *(Vector2 t, double m) => new Vector2(t.X * m, t.Y * m);
        public static Vector2 operator *(Vector2 t, Vector2 o) => new Vector2(t.X * o.X, t.Y * o.Y);
        public static Vector2 operator /(Vector2 t, double d) => new Vector2(t.X / d, t.Y / d);
        public static Vector2 operator +(Vector2 t, Vector2 o) => new Vector2(t.X + o.X, t.Y + o.Y);
        public static Vector2 operator +(Vector2 t, Point o) => new Vector2(t.X + o.X, t.Y + o.Y);
        public static Vector2 operator -(Vector2 t, Vector2 o) => new Vector2(t.X - o.X, t.Y - o.Y);
        public void Normalize()
        {
            double magnitude = Math.Sqrt(X * X + Y * Y);
            if (magnitude == 0) return;
            X = (X / magnitude);
            Y = (Y / magnitude);
        }

        public Vector2 Normalized()
        {
            Vector2 ret = new Vector2(this);
            ret.Normalize();
            return ret;
        }
        public Vector2 Absolute() => new Vector2(Math.Abs(X), Math.Abs(Y));
        public bool IsInside(Rectangle rect) => X >= rect.Left && X <= rect.Right && Y > rect.Top && Y <= rect.Bottom;
        public Point ToPoint() => new Point((int)X, (int)Y);
        public double Length() => Math.Sqrt(X * X + Y * Y);
    }
}
