using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.ShopUI
{
    internal class ExitShopButton : Button
    {
        public void OnClick(System.Windows.Forms.MouseEventArgs e)
        {
            MainWindow.Instance.ShopOpened = false;
        }
        const int defaultBorderWidth = 2;
        const int hoveredBorderWidth = 4;

        int borderWidth = defaultBorderWidth;
        bool isHovered = false;
        public void OnHoverEnter()
        {
            borderWidth = hoveredBorderWidth;
            isHovered = true;
        }
        public bool CurrentlyHovered() => isHovered;
        public void OnHoverExit()
        {
            borderWidth = defaultBorderWidth;
            isHovered = false;
        }

        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            if (!MainWindow.Instance.ShopOpened) return;
            var bounds = GetBounds(clientRectangle);
            g.DrawRectangle(new Pen(Color.White, borderWidth), bounds);
            g.DrawImage(Properties.Resources.CloseIcon, bounds);
        }

        public bool ShouldBeDestroyed() => false;

        public void Update() { }

        public bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle) => MainWindow.Instance.ShopOpened && GetBounds(clientRectangle).Contains(mouseLocation);
        private Rectangle GetBounds(Rectangle clientRectangle) => new Rectangle(clientRectangle.Width - 20 - 40, 20, 40, 40);
    }
}
