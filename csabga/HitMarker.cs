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

        public HitMarker(Vector2 position)
        {
            this.position = position.ToPoint();
        }
        public HitMarker(Point position)
        {
            this.position = position;
            color = Color.White;
        }
        public HitMarker(Vector2 position, Color color)
        {
            this.position = position.ToPoint();
            this.color = color;
        }
        public HitMarker(Point position, Color color)
        {
            this.position = position;
            this.color = color;
        }


        int r = 0;
        const int maxR = 50;
        double growSpeed = 500f / MainWindow.TargetFrameTime;



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
