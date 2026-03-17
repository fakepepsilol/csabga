using System;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csabga.ShopUI
{
    internal static class ShopItemPicker
    {
        public static readonly Dictionary<UpgradeType, UpgradeInfo> UpgradePool = new Dictionary<UpgradeType, UpgradeInfo>
        {
            { UpgradeType.PlayerSpeed, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.BulletDamage, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.BulletSpeed, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.ShootingSpeed, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.WindowSize, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.PickupRange, new UpgradeInfo(0, 10, 1.0f) },
            { UpgradeType.BulletPiercing, new UpgradeInfo(0, 5, 5.0f) }
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
                    var minCost = upgrade.CurrentLevel * upgrade.CurrentLevel + upgrade.CurrentLevel * 5 + 2;
                    var maxCost = upgrade.CurrentLevel * upgrade.CurrentLevel + upgrade.CurrentLevel * 10 + 10;
                    int cost = (int)((r.NextDouble() * (maxCost - minCost) + minCost) * upgrade.PriceFactor);
                    return new ShopItem(type, upgrade.CurrentLevel + 1, cost);
                }
            }
        }
    }

    internal class UpgradeInfo
    {
        public UpgradeInfo(int current, int limit, float priceFactor)
        {
            CurrentLevel = current;
            MaxLevel = limit;
            PriceFactor = priceFactor;
        }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public float PriceFactor { get; set; }
        public bool IsAvailable => CurrentLevel < MaxLevel;
    }
}
