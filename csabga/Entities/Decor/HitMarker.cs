using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    public class HitMarker : Renderable
    {
        Point position;
        Color color;
        int maxR = 50;
        double growSpeed = 500f / MainWindow.TargetFrameTime;

        public HitMarker(Vector2 position)
        {
            this.position = position.ToPoint();
        }
        public HitMarker(Point position)
        {
            this.position = position;
            color = Color.White;
        }
        public HitMarker(Vector2 position, Color color, int maxR = 50, float growSpeed = 500f)
        {
            this.position = position.ToPoint();
            this.color = color;
            this.maxR = maxR;
            this.growSpeed = growSpeed / MainWindow.TargetFrameTime;
        }
        public HitMarker(Point position, Color color, int maxR = 50, float growSpeed = 500f)
        {
            this.position = position;
            this.color = color;
            this.maxR = maxR;
            this.growSpeed = growSpeed / MainWindow.TargetFrameTime;
        }


        int r = 0;


        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {

            g.DrawEllipse(new Pen(color, 2), (position.X - windowLocation.X) - r / 2, (position.Y - windowLocation.Y) - r / 2, r, r);
        }

        public bool ShouldBeDestroyed() => r >= maxR;

        public void Update()
        {
            r += (int)(growSpeed * MainWindow.deltaTime);
        }
    }
}
