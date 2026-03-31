using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.ShopUI
{
    internal class ShopTitle : Renderable
    {
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {

            g.DrawString("Shop", new Font(MainWindow.Instance.SystemFontName, 36, FontStyle.Bold | FontStyle.Underline), new SolidBrush(Color.White), new RectangleF(0, 40, clientRectangle.Width, 80), new StringFormat()
            {
                Alignment = StringAlignment.Center,
            });
        }

        public bool ShouldBeDestroyed() => false;

        public void Update() { }
    }
}
