using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csabga.Enemies
{
    internal class Hexagon : Enemy
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public int Health { get; set; } = 4;
        private double movementSpeed = 30d / MainWindow.TargetFrameTime;
        private int a = 30;
        private double r;
        private Point[] points;
        public Hexagon(Vector2 spawnPosition)
        {
            Position = spawnPosition;
            r = a / 1.1755705045;
            points = new Point[6];
            for(int i = 0; i < 6; i++)
            {
                points[i] = new Point((int)(Math.Cos(2 * Math.PI * i / 6 + Math.PI / 2) * r), (int)(Math.Sin(2 * Math.PI * i / 6 + Math.PI / 2) * r));
            };
        }

        public bool CollidesWith(Bullet bullet)
        {
            var bulletPoints = bullet.Points;
            for(int i = 0; i < 4; i++)
            {
                var dx = Position.X - bulletPoints[i].X;
                var dy = Position.Y - bulletPoints[i].Y;
                if (dx * dx + dy * dy <= r * r) return true;
            }
            return false;
        }

        public bool CollidesWith(Player player)
        {
            var dx = Position.X - player.Position.X;
            var dy = Position.Y - player.Position.Y;
            return dx * dx + dy * dy < (r + 3) * (r + 3);
        }

        public void OnHit(int damage)
        {
            Health -= damage;
        }

        private double angle = MainWindow.Instance.R.NextDouble() * Math.PI * 2;
        public void Update()
        {
            angle += Math.PI / 60 * MainWindow.deltaTime;

            Vector2 direction = new Vector2(MainWindow.Instance.Player.Position.X - Position.X, MainWindow.Instance.Player.Position.Y - Position.Y);
            double magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            if (magnitude == 0) throw new InvalidOperationException();

            direction = direction / magnitude;

            Position += direction * movementSpeed * MainWindow.deltaTime;
        }
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            var newPoints = new Point[6];
            for(int i = 0; i < 6; i++)
            {
                newPoints[i] = new Point(points[i].X, points[i].Y);
            }
            Rotator.RotatePoints(newPoints, 6, Vector2.Zero, angle);
            for(int i = 0; i < 6; i++)
            {
                newPoints[i].X = (int)(newPoints[i].X + Position.X - windowLocation.X);
                newPoints[i].Y = (int)(newPoints[i].Y + Position.Y - windowLocation.Y);
            }
            g.DrawPolygon(new Pen(Color.Green), newPoints);
        }

        public bool ShouldBeDestroyed() => Health <= 0;
        public float KillReward => 5;
    }
}
