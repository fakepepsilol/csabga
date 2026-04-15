using csabga.UI.DeathScreen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    public class Player : Renderable
    {
        #region input
        bool goingUp = false;
        bool goingLeft = false;
        bool goingDown = false;
        bool goingRight = false;
        bool moving => (goingUp != goingDown) || (goingLeft != goingRight);

        bool shift = false;

        public void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    goingUp = true;
                    goingDown = false;
                    break;
                case Keys.A:
                    goingLeft = true;
                    goingRight = false;
                    break;
                case Keys.S:
                    goingDown = true;
                    goingUp = false;
                    break;
                case Keys.D:
                    goingRight = true;
                    goingLeft = false;
                    break;
                case Keys.ShiftKey:
                    shift = true;
                    break;
            }
        }
        public void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    goingUp = false;
                    break;
                case Keys.A:
                    goingLeft = false;
                    break;
                case Keys.S:
                    goingDown = false;
                    break;
                case Keys.D:
                    goingRight = false;
                    break;
                case Keys.ShiftKey:
                    shift = false;
                    break;
            }
        }
        private Point lastMouseLocation = Point.Empty;
        public void OnMouseDown(MouseEventArgs e, Point windowLocation)
        {
            if(e.Button == MouseButtons.Left)
            {
                var shootPeriod = (int)(50_000 / ShootingSpeed);
                var sinceLastShot = (int)(DateTime.Now - lastShootTime).TotalMilliseconds;
                if (sinceLastShot > shootPeriod)
                {
                    FireGun(new Point(e.Location.X + windowLocation.X, e.Location.Y + windowLocation.Y));
                    sinceLastShot = 0;
                }
                shootingTimer.Change(shootPeriod - sinceLastShot, shootPeriod);
            }
        }
        public void OnMouseMove(MouseEventArgs e, Point windowLocation)
        {
            lastMouseLocation = new Point(e.Location.X + windowLocation.X, e.Location.Y + windowLocation.Y);
        }
        public void OnMouseUp(MouseEventArgs e, Point windowLocation)
        {
            if(e.Button == MouseButtons.Left)
            {
                shootingTimer.Change(int.MaxValue, int.MaxValue);
            }
        }
        #endregion

        Vector2 globalPosition;
        public Vector2 Position => globalPosition;

        public float Coins { get; set; } = 0;
        private int health = 5;
        public int Health
        {
            get => health;
            set
            {
                if(value <= 0 && health > 0)
                {
                    OnDeath();
                }
                health = value;
            }
        }
        public bool Dead => Health <= 0;
        private DateTime lastHitTime = DateTime.MinValue;


        Color playerColor;

        public double Speed { get; set; } = 250 / MainWindow.TargetFrameTime;
        public int BulletDamage { get; set; } = 1;
        public int BulletPiercing { get; set; } = 1;
        public double BulletSpeed { get; set; } = 300 / MainWindow.TargetFrameTime;
        public double ShootingSpeed { get; set; } = 50;
        private DateTime lastShootTime = DateTime.Now;
        private System.Threading.Timer shootingTimer;

        public float PickupRange { get; set; } = 1.0f;

        public const int playerRadius = 10;
        public const int playerBorderWidth = 3;
        public const double shiftSpeedMultiplier = 1.35f;

        public Player(int initialX, int initialY)
        {
            shootingTimer = new System.Threading.Timer((_) => {
                FireGun(lastMouseLocation);
            });
            globalPosition = new Vector2(initialX, initialY);
            playerColor = MainWindow.Instance.playerColor;
        }

        private Vector2 currentSpeed = new Vector2(0, 0);
        private double accelerationFactor = 0.2;
        public Vector2 externalForce = new Vector2(0, 0);
        public void Update()
        {
            var moveSpeed = shift ? Speed * shiftSpeedMultiplier : Speed;

            int moveDelta = (int)(moveSpeed * MainWindow.deltaTime);
            int x = ((goingLeft && !goingRight) || (goingRight && !goingLeft)) ? (goingLeft ? -moveDelta : moveDelta) : 0;
            int y = ((goingUp && !goingDown) || (goingDown && !goingUp)) ? (goingUp ? -moveDelta : moveDelta) : 0;
            
            var actualMove = currentSpeed + new Vector2(x, y) * accelerationFactor;
            if((x > 0 && currentSpeed.X < 0) || (x < 0 && currentSpeed.X < 0))
            {
                actualMove.X += x * accelerationFactor / 2;
            }
            if((y > 0 && currentSpeed.Y < 0) || (y < 0 && currentSpeed.Y < 0))
            {
                actualMove.Y += y * accelerationFactor / 2;
            }
            if(x == 0)
            {
                actualMove.X /= 2;
            } else {
                actualMove.X = Math.Abs(actualMove.X) > Math.Abs(x) ? x : actualMove.X;
            }
            if (y == 0)
            {
                actualMove.Y /= 2;
            } else {
                actualMove.Y = Math.Abs(actualMove.Y) > Math.Abs(y) ? y : actualMove.Y;
            }
            currentSpeed = actualMove + externalForce;
            externalForce *= 0.3 * MainWindow.deltaTime;

            var newPosition = globalPosition + currentSpeed.Normalized() * currentSpeed.Absolute();
            var screenBounds = MainWindow.Instance.ScreenBounds;
            var xValid = newPosition.X > (playerRadius + playerBorderWidth) && (newPosition.X - playerRadius - playerBorderWidth) <= screenBounds.Width;
            var yValid = newPosition.Y > (playerRadius + playerBorderWidth) && (newPosition.Y - playerRadius - playerBorderWidth) <= screenBounds.Height;
            if(!xValid)
            {
                newPosition.X = globalPosition.X;
            }
            if(!yValid)
            {
                newPosition.Y = globalPosition.Y;
            }
            globalPosition = newPosition;
        }
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            int x = (int)globalPosition.X - windowLocation.X;
            int y = (int)globalPosition.Y - windowLocation.Y;

            var actualColor = (shift && moving) ? Color.FromArgb(122, 255, 237) : playerColor;
            if((DateTime.Now - lastHitTime).TotalMilliseconds < 250)
            {
                actualColor = Color.Red;
            }
            g.DrawEllipse(new Pen(actualColor, playerBorderWidth), x - playerRadius, y - playerRadius, playerRadius * 2, playerRadius * 2);

            var opacity = (1 - Math.Min(Math.Max((DateTime.Now - lastHitTime).TotalMilliseconds / 250, 0), 1)) * 0.8f;
            g.FillRectangle(new SolidBrush(Color.FromArgb((int)(opacity * 255), 186, 4, 4)), clientRectangle);

        }

        public void FireGun(Point clickPosition)
        {
            if (MainWindow.Instance.ShopOpened) return;
            MainWindow.Instance.AddRenderable(new Bullet(BulletDamage, BulletSpeed, BulletPiercing, globalPosition + (new Vector2(clickPosition) - globalPosition).Normalized() * 10, clickPosition));
            lastShootTime = DateTime.Now;
        }

        public bool ShouldBeDestroyed() => false;

        public bool CollidesWith(Coin coin)
        {
            int selfR = playerRadius + Coin.Radius / 2;
            return ((globalPosition.X - coin.Position.X) * (globalPosition.X - coin.Position.X) + (globalPosition.Y - coin.Position.Y) * (globalPosition.Y - coin.Position.Y) < selfR * selfR);
        }
        public bool CollidesWith(HealthPickup pickup)
        {
            int selfR = playerRadius - 2;
            return ((globalPosition.X - pickup.Position.X) * (globalPosition.X - pickup.Position.X) + (globalPosition.Y - pickup.Position.Y) * (globalPosition.Y - pickup.Position.Y) < selfR * selfR);
        }

        public void OnHit()
        {
            Health--;
            lastHitTime = DateTime.Now;
        }
        private void OnDeath()
        {
            MainWindow.Instance.Buttons.Add(new DeathScreen());
        }
        public bool CollidesWith(BossBullet bullet)
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
        private bool PointInside(Point point) => (Math.Pow(Position.X - point.X, 2) + Math.Pow(Position.Y - point.Y, 2)) <= ((playerRadius / 2 + playerBorderWidth) * (playerRadius / 2 + playerBorderWidth));
    }
}
