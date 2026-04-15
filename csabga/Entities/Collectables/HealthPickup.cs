using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    public class HealthPickup : Renderable
    {
        private Vector2 position;
        public Vector2 Position => position;
        public int Value { get; private set; }

        public const int size = 24;

        private int decayTime = 10_000;
        public HealthPickup(Vector2 position, int value)
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
        private void ActualRender(Graphics g, Point windowLocation)
        {
            g.DrawImage(Properties.Resources.HealthIcon, new Rectangle((int)(position.X - size / 2) - windowLocation.X, (int)(position.Y - size / 2) - windowLocation.Y, size, size));
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
