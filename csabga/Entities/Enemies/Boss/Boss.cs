using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csabga.Enemies
{
    internal class Boss : Enemy
    {
        private const double R = 100d;
        private int _health = 100;
        public int Health
        {
            get => _health;
            set
            {
                if (value <= 0 && _health > 0)
                {
                    OnDeath();
                }
                _health = value;
            }
        }
        private double angle = 0f;

        public Vector2 Position { get; set; }
        private double movementSpeed = 20d / MainWindow.TargetFrameTime;
        public Boss(Vector2 spawnPosition)
        {
            Position = spawnPosition;
        }
        private float lastShotAgoMs = 0f;
        public void Update()
        {

            if(lastShotAgoMs > 1200f)
            {
                Shoot();
                lastShotAgoMs = 0f;
                angle += Math.PI * 2 / MainWindow.Instance.R.Next(20, 60);
            }

            Vector2 direction = new Vector2(MainWindow.Instance.Player.Position.X - Position.X, MainWindow.Instance.Player.Position.Y - Position.Y);
            double magnitude = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            if (magnitude == 0) throw new InvalidOperationException();

            direction = direction / magnitude;

            Position += direction * movementSpeed * MainWindow.deltaTime;
        }

        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            lastShotAgoMs += (float)MainWindow.deltaTime * MainWindow.TargetFrameTime;

            var drawPointX = Position.X - R / 2 - windowLocation.X;
            var drawPointY = Position.Y - R / 2 - windowLocation.Y;
            g.DrawEllipse(new Pen(Color.LightBlue), (int)(drawPointX), (int)(drawPointY), (int)R, (int)R);
        }

        public bool ShouldBeDestroyed() => Health <= 0;

        public bool CollidesWith(Bullet bullet)
        {
            var bulletPoints = bullet.Points;
            for (int i = 0; i < 4; i++)
            {
                if (PointInside(bulletPoints[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CollidesWith(Player player)
        {
            var playerPosition = player.Position.ToPoint();
            var d = Math.Pow(Position.X - playerPosition.X, 2) + Math.Pow(Position.Y - playerPosition.Y, 2);
            var r1 = (R / 2) * (R / 2);
            var r2 = Player.playerRadius * Player.playerRadius;

            return d <= (r1 + r2);
        }
        private bool PointInside(Point point) => (Math.Pow(Position.X - point.X, 2) + Math.Pow(Position.Y - point.Y, 2)) < (R / 2 * R / 2);

        public void OnHit(int damage)
        {
            Health -= damage;
        }

        public void OnDeath()
        {
            var r = new Random();
            int n = 15;
            for (int i = 0; i < n; i++)
            {
                var reward = r.Next((int)(800 / (n + 3)), (int)(800 / (n - 3)));
                int x = r.Next((int)(Position.X - R / 2), (int)(Position.X + R / 2));
                int y = r.Next((int)(Position.Y - R / 2), (int)(Position.Y + R / 2));
                MainWindow.Instance.AddRenderable(new Coin(new Vector2(x, y), reward));
            }
        }
        public float KillReward => 0;


        private void Shoot()
        {
            int bulletCount = 16;

            for(int i = 0; i < bulletCount; i++)
            {
                double rad = angle + Math.PI * 2 * (i / (float)bulletCount);
                var points = new List<Point>() {
                    new Point(0, -(int)(R / 2))
                };
                var direction = new Vector2(Rotator.RotatePoints(points, Vector2.Zero, rad)[0]);

                MainWindow.Instance.AddRenderable(new BossBullet(1, 7d, 1, Position + direction * 1.1f, (Position + direction * 2).ToPoint()));
            }
        }
    }
}
