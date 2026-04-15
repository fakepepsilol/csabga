using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    public class BossBullet : Renderable
    {
        public Vector2 position;
        Vector2 direction;

        public const int bulletLength = 10;
        public const int bulletWidth = bulletLength / 2;
        public int RemainingDamage { get; set; }
        public int PiercingLevel { get; set; }
        public double Speed { get; set; }
        public BossBullet(int bulletDamage, double bulletSpeed, int maxPierceCount, Vector2 position, Point clickPosition)
        {
            RemainingDamage = bulletDamage;
            PiercingLevel = maxPierceCount;
            Speed = bulletSpeed;
            this.position = position;

            Vector2 direction = new Vector2(clickPosition.X - position.X, clickPosition.Y - position.Y);
            double magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            if (magnitude == 0) throw new InvalidOperationException();

            this.direction = direction / magnitude;
        }
        public void Update()
        {
            position += direction * Speed * MainWindow.deltaTime;
        }
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            Point[] points = Points;
            for (int i = 0; i < 4; i++)
            {
                points[i].X -= windowLocation.X;
                points[i].Y -= windowLocation.Y;
            }
            g.FillPolygon(new SolidBrush(Color.IndianRed), points);
        }
        public Point[] Points
        {
            get
            {
                Point[] points = new Point[4] {
                    new Point((int)(position.X - bulletLength / 2), (int)(position.Y - bulletWidth / 2)),
                    new Point((int)(position.X + bulletLength / 2), (int)(position.Y - bulletWidth / 2)),
                    new Point((int)(position.X + bulletLength / 2), (int)(position.Y + bulletWidth / 2)),
                    new Point((int)(position.X - bulletLength / 2), (int)(position.Y + bulletWidth / 2))
                };

                Rotator.RotatePoints(points, 4, position, Math.Atan2(direction.Y, direction.X));
                return points;
            }
        }
        private bool alreadyHit = false;
        public bool ShouldBeDestroyed() => alreadyHit || !position.IsInside(MainWindow.Instance.ScreenBounds);
        public void OnHit() => alreadyHit = true;
    }
}
