using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.UI.Components
{
    internal class Backdrop : Renderable
    {
        private SolidBrush brush;
        public Backdrop(Color color)
        {
            brush = new SolidBrush(color);
        }
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            g.FillRectangle(brush, 0, 0, clientRectangle.Width, clientRectangle.Height);
        }

        public bool ShouldBeDestroyed() => false;

        public void Update() { }
    }
}
