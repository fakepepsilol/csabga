using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga.ShopUI
{
    internal class RerollButton : Button
    {
        public double Cost { get; set; } = 0;
        public bool Available => MainWindow.Instance.Player.Coins >= Cost && !MainWindow.Instance.ShopItems.All(item => item.Item.Type == UpgradeType.None);

        Font font = new Font(MainWindow.Instance.SystemFontName, 16, FontStyle.Bold);
        private Random r;
        private Graphics measureGraphics;
        public RerollButton(Random r, Graphics graphics)
        {
            measureGraphics = graphics;
            this.r = r;
            Reroll();
            Cost = r.Next(1, 6);
        }
        private const int bottomMargin = 25;
        private const int buttonHeight = 30;
        private const int buttonWidth = 180;
        private Rectangle GetBounds(Rectangle clientRectangle, Graphics g) => new Rectangle(
            (clientRectangle.Width - buttonWidth) / 2,
            clientRectangle.Height - bottomMargin - buttonHeight,
            Math.Max(contentMargin + (int)g.MeasureString(text, font).Width + iconSize + (int)g.MeasureString(((int)Cost).ToString(), font).Width + contentMargin * 2 + 10, buttonWidth),
            buttonHeight);
        public bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle) => GetBounds(clientRectangle, measureGraphics).Contains(mouseLocation);

        public bool CurrentlyHovered() => isHovered;

        public void OnClick(MouseEventArgs e)
        {
            if (!Available) return;
            Reroll();
        }
        bool isHovered = false;
        public void OnHoverEnter()
        {
            isHovered = true;
            borderWidth = hoveredBorderWidth;
        }

        public void OnHoverExit()
        {
            isHovered = false;
            borderWidth = defaultBorderWidth;
        }

        private int borderWidth = defaultBorderWidth;
        private const int defaultBorderWidth = 2;
        private const int hoveredBorderWidth = 4;

        private const int contentMargin = 10;

        private const string text = "Reroll";
        private const int iconSize = 30;

        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            if (!MainWindow.Instance.ShopOpened) return;
            var itemBounds = GetBounds(clientRectangle, g);

            var actualColor = !Available ? Color.FromArgb(0x67, 0xFF, 0xFF, 0xFF) : Color.White;
            var actualBrush = new SolidBrush(actualColor);

            // border
            g.DrawRectangle(new Pen(actualColor, Available ? borderWidth : defaultBorderWidth), itemBounds);


            ColorMatrix matrix = new ColorMatrix();
            matrix.Matrix33 = Available ? 1.0f : 0.35f; // opacity

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            
            var rerollTextBounds = itemBounds;
            rerollTextBounds.X += contentMargin;
            rerollTextBounds.Width = (int)g.MeasureString(text, font).Width + 10;
            g.DrawString(text, font, actualBrush, rerollTextBounds, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });

            var coinIconBounds = itemBounds;
            coinIconBounds.X += contentMargin + rerollTextBounds.Width;
            coinIconBounds.Width = iconSize;
            g.DrawImage(
                Properties.Resources.CoinIcon, coinIconBounds,
                0, 0, Properties.Resources.CoinIcon.Width, Properties.Resources.CoinIcon.Height,
                GraphicsUnit.Pixel, imageAttributes);


            var costTextBounds = itemBounds;
            costTextBounds.X += contentMargin + rerollTextBounds.Width + iconSize - 5;
            costTextBounds.Width = itemBounds.Width - rerollTextBounds.Width - iconSize - contentMargin;
            g.DrawString(((int)Cost).ToString(), font, actualBrush, costTextBounds, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
        }

        public bool ShouldBeDestroyed() => false;
        public void Update() { }
        int rerollCount = 0;
        public void Reroll()
        {
            MainWindow.Instance.Player.Coins -= (float)Cost;
            var itemCount = MainWindow.Instance.ShopItems.Count;
            if (itemCount == 0) itemCount = 4;
            MainWindow.Instance.ShopItems.RemoveAll(_ => true);
            for(int i = 0; i < itemCount; i++)
            {
                MainWindow.Instance.ShopItems.Add(new ShopItemButton(r, i, itemCount));
            }

            Cost *= (r.NextDouble() / 2 + 1);
            rerollCount++;
        }
    }
}