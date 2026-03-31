using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.Enemies
{
    internal class Triangle : Enemy
    {
        private const double a = 40d;
        private float h = (float)(a * Math.Sqrt(3) / 2);

        private double angle = MainWindow.Instance.R.NextDouble() * Math.PI * 2;
        public int Health { get; set; } = 2;
        public Vector2 Position { get; set; }
        private double movementSpeed = 50d / MainWindow.TargetFrameTime;
        public Triangle(Vector2 spawnPosition)
        {
            Position = spawnPosition;
        }
        public void Update()
        {
            angle += 2 * Math.PI / 60 * MainWindow.deltaTime;

            Vector2 direction = new Vector2(MainWindow.Instance.Player.Position.X - Position.X, MainWindow.Instance.Player.Position.Y - Position.Y);
            double magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            if (magnitude == 0) throw new InvalidOperationException();

            direction = direction / magnitude;

            Position += direction * movementSpeed * MainWindow.deltaTime;
        }


        public Point[] Points => new Point[4] {
            new Point((int)Position.X,           (int)(Position.Y - 2 * h / 3)),
            new Point((int)(Position.X - a / 2), (int)(Position.Y + h / 3)),
            new Point((int)(Position.X + a / 2), (int)(Position.Y + h / 3)),
            new Point((int)Position.X,           (int)(Position.Y - 2 * h / 3)),
        };
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {

            var points = Points;

            Rotator.RotatePoints(points, 4, Position, angle);

            for (int i = 0; i < 4; i++)
            {
                points[i].X -= windowLocation.X;
                points[i].Y -= windowLocation.Y;
            }
            g.DrawLines(new Pen(Color.LightCoral), points);
        }

        public bool ShouldBeDestroyed() => Health <= 0;

        public bool CollidesWith(Bullet bullet)
        {
            var bulletPoints = bullet.Points;
            var trianglePoints = Points;
            for (int i = 0; i < 4; i++)
            {
                if (PointInTriangle(bulletPoints[i], trianglePoints[0], trianglePoints[1], trianglePoints[2]))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CollidesWith(Player player)
        {
            var trianglePoints = Points;
            var playerPosition = player.Position.ToPoint();

            // centar
            if (PointInTriangle(playerPosition, trianglePoints)) return true;

            // gornja tacka
            playerPosition.Y -= Player.playerRadius;
            if (PointInTriangle(playerPosition, trianglePoints)) return true;

            // donja tacka
            playerPosition.Y += Player.playerRadius * 2;
            if (PointInTriangle(playerPosition, trianglePoints)) return true;

            // leva tacka
            playerPosition.Y -= Player.playerRadius;
            playerPosition.X -= Player.playerRadius;
            if (PointInTriangle(playerPosition, trianglePoints)) return true;

            // desna tacka
            playerPosition.X += Player.playerRadius * 2;
            if (PointInTriangle(playerPosition, trianglePoints)) return true;
            return false;
        }
        private bool PointInTriangle(Point c, Point[] trianglePoints) => PointInTriangle(c, trianglePoints[0], trianglePoints[1], trianglePoints[2]);
        private bool PointInTriangle(Point c, Point t1, Point t2, Point t3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(c, t1, t2);
            d2 = sign(c, t2, t3);
            d3 = sign(c, t3, t1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
        float sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public void OnHit(int damage)
        {
            Health -= damage;
        }
        public float KillReward => 1;
    }
}
