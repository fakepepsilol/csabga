using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.ShopUI
{
    public class ShopItemButton : Button
    {
        public const int ChoiceSpacing = 10;
        public const int ChoicesMargin = 25;
        int choiceIndex;
        public int Index => choiceIndex;
        int choiceCount;

        public ShopItem Item;
        public bool Available => (Item.Type != UpgradeType.None) && (MainWindow.Instance.Player.Coins >= Item.Cost);

        private Random r;
        public ShopItemButton(Random r, int choiceIndex, int choiceCount)
        {
            this.r = r;
            Item = ShopItemPicker.PickNextItem(r, choiceIndex);
            this.choiceIndex = choiceIndex;
            this.choiceCount = choiceCount;
        }
        private int GetChoiceWidth(Rectangle clientRectangle) => ((clientRectangle.Width - 2 * ChoicesMargin) - ChoiceSpacing * (choiceCount - 1)) / choiceCount;
        private Rectangle GetBounds(Rectangle clientRectangle) => new Rectangle(
            ChoicesMargin + choiceIndex * (GetChoiceWidth(clientRectangle) + ChoiceSpacing),
            120,
            GetChoiceWidth(clientRectangle),
            clientRectangle.Height - 120 - ChoicesMargin * 3
            );
        public bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle) => GetBounds(clientRectangle).Contains(mouseLocation);

        public bool CurrentlyHovered() => isHovered;

        public void OnClick(System.Windows.Forms.MouseEventArgs e)
        {
            if (!Available) return;
            Item.Purchase();
            Item = ShopItemPicker.PickNextItem(r, choiceIndex);
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

        private const int upgradeNameMargin = 10;
        private const int upgradeIconSize = 100;
        private const int upgradeCostIconSize = 40;
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            if (!MainWindow.Instance.ShopOpened) return;
            var itemBounds = GetBounds(clientRectangle);

            var actualColor = !Available ? Color.FromArgb(0x67, 0xFF, 0xFF, 0xFF) : Color.White;
            var actualBrush = new SolidBrush(actualColor);

            g.DrawRectangle(new Pen(actualColor, (Item.Type == UpgradeType.None) ? defaultBorderWidth : borderWidth), itemBounds);


            // upgrade icon

            var upgradeIconBounds = itemBounds;
            var iconMargin = (itemBounds.Width - upgradeIconSize) / 2;
            upgradeIconBounds.X += iconMargin;
            upgradeIconBounds.Y += iconMargin;
            upgradeIconBounds.Width = upgradeIconSize;
            upgradeIconBounds.Height = upgradeIconSize;

            
            ColorMatrix matrix = new ColorMatrix();
            matrix.Matrix33 = Available ? 1.0f : 0.35f; // opacity

            ImageAttributes iconAttributes = new ImageAttributes();
            iconAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(
                Item.Icon,
                upgradeIconBounds,
                0, 0, Item.Icon.Width, Item.Icon.Height,
                GraphicsUnit.Pixel, iconAttributes);


            // upgrade name

            var upgradeNameBounds = itemBounds;
            upgradeNameBounds.Y += upgradeIconBounds.Y + upgradeNameMargin;
            upgradeNameBounds.X += upgradeNameMargin;
            upgradeNameBounds.Width -= upgradeNameMargin * 2;
            var upgradeNameFont = new Font(MainWindow.Instance.SystemFontName, 12, FontStyle.Bold);

            g.DrawString(Item.DisplayName, upgradeNameFont, actualBrush, upgradeNameBounds, new StringFormat()
            {
                Alignment = StringAlignment.Center,
            });


            // upgrade cost

            var upgradeCostBounds = itemBounds;
            upgradeCostBounds.Y = itemBounds.Y + itemBounds.Height - upgradeCostIconSize - 10; 
            var upgradeCostFont = new Font(MainWindow.Instance.SystemFontName, 12, FontStyle.Bold);
            var costContentWidth = upgradeCostIconSize + 10 + g.MeasureString(Item.Cost.ToString(), upgradeCostFont).Width;
            var costMargin = (int)((itemBounds.Width - costContentWidth) / 2);
            upgradeCostBounds.X += costMargin;
            upgradeCostBounds.Width -= costMargin * 2;
            upgradeCostBounds.Height = upgradeCostIconSize;
            var upgradeCostIconBounds = upgradeCostBounds;
            upgradeCostIconBounds.Width = upgradeCostIconSize;
            upgradeCostIconBounds.Height = upgradeCostIconSize;
            g.DrawImage(
                Properties.Resources.CoinIcon,
                upgradeCostIconBounds,
                0, 0, Properties.Resources.CoinIcon.Width, Properties.Resources.CoinIcon.Height,
                GraphicsUnit.Pixel,
                iconAttributes);

            var upgradeCostTextBounds = upgradeCostBounds;
            upgradeCostTextBounds.X += upgradeCostIconSize + 5;
            upgradeCostTextBounds.Width -= upgradeCostIconSize + 10;
            g.DrawString(Item.Cost.ToString(), upgradeCostFont, actualBrush, upgradeCostTextBounds, new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
        }

        public bool ShouldBeDestroyed() => false;
        public void Update() { }
    }
}
