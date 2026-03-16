using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.ShopUI
{
    internal static class ShopItemPicker
    {
        public static readonly Dictionary<UpgradeType, ProgressInt> UpgradePool = new Dictionary<UpgradeType, ProgressInt>
        {
            { UpgradeType.PlayerSpeed, new ProgressInt(0, 10) },
            { UpgradeType.BulletDamage, new ProgressInt(0, 10) },
            { UpgradeType.BulletSpeed, new ProgressInt(0, 10) },
            { UpgradeType.ShootingSpeed, new ProgressInt(0, 10) },
            { UpgradeType.WindowSize, new ProgressInt(0, 10) },
            { UpgradeType.PickupRange, new ProgressInt(0, 10) }
        };
        public static ShopItem PickNextItem(Random r, int choiceIndex)
        {
            while (true)
            {
                List<UpgradeType> availableUpgradeTypes = new List<UpgradeType>();
                foreach(var kvp in UpgradePool)
                {
                    if(kvp.Value.IsAvailable)
                    {
                        availableUpgradeTypes.Add(kvp.Key);
                    }
                }
                var currentShopItems = MainWindow.Instance.ShopItems;
                for (int i = 0; i < currentShopItems.Count; i++)
                {
                    if (i == choiceIndex) continue;
                    availableUpgradeTypes.Remove(currentShopItems[i].Item.Type);
                }
                if(availableUpgradeTypes.Count == 0)
                {
                    return new ShopItem(UpgradeType.None, 0, 0);
                }
                var type = availableUpgradeTypes[r.Next(availableUpgradeTypes.Count)];
                var upgrade = UpgradePool[type];
                if (upgrade.IsAvailable)
                {
                    var cost = r.Next(upgrade.Current * upgrade.Current + upgrade.Current * 5 + 2, upgrade.Current * upgrade.Current + upgrade.Current * 10 + 10);
                    return new ShopItem(type, upgrade.Current + 1, cost);
                }
            }
        }
    }

    internal class ProgressInt
    {
        public ProgressInt(int current, int limit)
        {
            Current = current;
            Limit = limit;
        }
        public int Current { get; set; }
        public int Limit { get; set; }
        public bool IsAvailable => Current < Limit;
    }
}
