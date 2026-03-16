using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    public class Coin : Renderable
    {
        private Vector2 position;
        public Vector2 Position => position;
        public float Value { get; private set; }

        public const int Radius = 5;
        private int decayTime = 10_000;
        public Coin(Vector2 position, float value)
        {
            this.position = position;
            Value = value;
        }
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            if (decayTime <= 0) return;

            if(decayTime < 3000)
            {
                if(decayTime % 400 < 200)
                {
                    ActualRender(g, windowLocation);
                }
            } else
            {
                ActualRender(g, windowLocation);
            }
        }
        Color lowValueColor = Color.Gold;
        Color highValueColor = Color.DarkViolet;
        private float colorWeight => (float)Math.Min(Value / 5, 1.0);
        public Color Color => Color.FromArgb(
            (int)(lowValueColor.R + (highValueColor.R - lowValueColor.R) * colorWeight),
            (int)(lowValueColor.G + (highValueColor.G - lowValueColor.G) * colorWeight),
            (int)(lowValueColor.B + (highValueColor.B - lowValueColor.B) * colorWeight));
        private void ActualRender(Graphics g, Point windowLocation)
        {
            g.FillEllipse(new SolidBrush(Color), (int)(position.X - windowLocation.X) - Radius / 2, (int)(position.Y - windowLocation.Y) - Radius / 2, Radius, Radius);
        }

        public bool ShouldBeDestroyed() => decayTime <= 0;

        public void Update()
        {
            decayTime -= (int)(MainWindow.deltaTime * MainWindow.TargetFrameTime);
            var playerPosition = MainWindow.Instance.Player.Position;
            var dx = playerPosition.X - Position.X;
            var dy = playerPosition.Y - Position.Y;
            var distanceToPlayer = Math.Sqrt(dx * dx + dy * dy);

            var pickupRange = MainWindow.Instance.Player.PickupRange;
            if (distanceToPlayer > 100 * pickupRange) return;
            var direction = new Vector2(dx, dy);
            var move = direction.Normalized() * (100 / distanceToPlayer * pickupRange) * (100 / distanceToPlayer * pickupRange) * MainWindow.deltaTime;

            var newPosition = position + move;

            if((position.X < playerPosition.X && newPosition.X > playerPosition.X) || (position.Y < playerPosition.Y && newPosition.Y > playerPosition.Y ))
            {
                newPosition = playerPosition;
            }
            position = newPosition;
        }

        public void OnPickup()
        {
            decayTime = 0;
        }
    }
}
