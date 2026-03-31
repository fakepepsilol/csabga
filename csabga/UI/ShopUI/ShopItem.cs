using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga.ShopUI
{
    public class ShopItem
    {
        public ShopItem(UpgradeType type, int level, int cost)
        {
            Type = type;
            Level = level;
            Cost = cost;
            switch(type)
            {
                case UpgradeType.PlayerSpeed:
                    DisplayName = $"Player Speed Lvl.{level}";
                    Icon = Properties.Resources.PlayerSpeedIcon;
                    break;
                case UpgradeType.BulletDamage:
                    DisplayName = $"Bullet Damage Lvl.{level}";
                    Icon = Properties.Resources.BulletDamageIcon;
                    break;
                case UpgradeType.BulletSpeed:
                    DisplayName = $"Bullet Speed Lvl.{level}";
                    Icon = Properties.Resources.BulletSpeedIcon;
                    break;
                case UpgradeType.ShootingSpeed:
                    DisplayName = $"Shooting Speed Lvl.{level}";
                    Icon = Properties.Resources.ShootingSpeedIcon;
                    break;
                case UpgradeType.WindowSize:
                    DisplayName = $"Window Size Lvl.{level}";
                    Icon = Properties.Resources.WindowSizeIcon;
                    break;
                case UpgradeType.PickupRange:
                    DisplayName = $"Pickup Range Lvl.{level}";
                    Icon = Properties.Resources.PickupRangeIcon;
                    break;
                case UpgradeType.BulletPiercing:
                    DisplayName = $"Bullet Piercing Lvl.{level}";
                    Icon = Properties.Resources.BulletPiercingIcon;
                    break;
                case UpgradeType.None:
                    DisplayName = "No upgrades remaining.";
                    Icon = Properties.Resources.NoUpgradeIcon;
                    break;
            }
        }
        public void Purchase()
        {
            if (Type == UpgradeType.None) return;
            ShopItemPicker.UpgradePool[Type].CurrentLevel++;
            if(MainWindow.Instance.Player.Coins < Cost)
            {
                //throw new Exception("Invalid state exception");
            }
            MainWindow.Instance.Player.Coins -= Cost;

            switch(Type)
            {
                case UpgradeType.PlayerSpeed:
                    MainWindow.Instance.Player.Speed *= 1.2;
                    break;
                case UpgradeType.BulletDamage:
                    MainWindow.Instance.Player.BulletDamage++;
                    break;
                case UpgradeType.BulletSpeed:
                    MainWindow.Instance.Player.BulletSpeed *= 1.2;
                    break;
                case UpgradeType.ShootingSpeed:
                    MainWindow.Instance.Player.ShootingSpeed *= 1.6;
                    break;
                case UpgradeType.WindowSize:
                    MainWindow.Instance.ExpandWindow(40);
                    break;
                case UpgradeType.PickupRange:
                    MainWindow.Instance.Player.PickupRange *= 1.2f;
                    break;
                case UpgradeType.BulletPiercing:
                    MainWindow.Instance.Player.BulletPiercing++;
                    break;
            }
        }
        public UpgradeType Type { get; set; }
        public int Level { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int Cost { get; set; } = -1;
        public Bitmap Icon { get; set; } = null;
    }
}
