using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csabga.UI.DeathScreen
{
    internal class RestartButton : Button
    {
        private bool hovered = false;
        private int width = 100;
        private int height = 40;
        private string text = "restart";
        private float fontSize = 20f;

        private Rectangle GetBounds(Rectangle clientRectangle) => new Rectangle(clientRectangle.Width / 2 + 6, clientRectangle.Height - 3 * height, width, height);
        public bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle) => GetBounds(clientRectangle).Contains(mouseLocation);

        public bool CurrentlyHovered() => hovered;

        public void OnClick(System.Windows.Forms.MouseEventArgs e)
        {
            var oldWindow = MainWindow.Instance;
            new MainWindow().Show();
            oldWindow.Hide();
        }

        public void OnHoverEnter() => hovered = true;

        public void OnHoverExit() => hovered = false;

        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            var bounds = GetBounds(clientRectangle);
            var pen = new Pen(Color.White, hovered ? 4 : 2);
            g.DrawRectangle(pen, bounds);
            g.DrawString(text, new Font(MainWindow.Instance.SystemFontName, fontSize), new SolidBrush(Color.White), bounds, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
        }

        public bool ShouldBeDestroyed() => false;

        public void Update() { }
    }
}
