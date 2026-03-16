using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    internal static class Rotator
    {
        public static List<Point> RotatePoints(List<Point> points, Vector2 center, double angle)
        {
            double rad = angle;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);
            List<Point> ret = new List<Point>(points);
            for (int i = 0; i < points.Count; i++)
            {
                int x = (int)(cos * (points[i].X - center.X) - sin * (points[i].Y - center.Y) + center.X);
                int y = (int)(sin * (points[i].X - center.X) + cos * (points[i].Y - center.Y) + center.Y);
            }
            return ret;
        }
        public static void RotatePoints(Point[] points, int pointCount, Vector2 center, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            for (int i = 0; i < pointCount; i++)
            {
                int x = (int)(cos * (points[i].X - center.X) - sin * (points[i].Y - center.Y) + center.X);
                int y = (int)(sin * (points[i].X - center.X) + cos * (points[i].Y - center.Y) + center.Y);
                points[i].X = x;
                points[i].Y = y;
            }
        }
    }
}
