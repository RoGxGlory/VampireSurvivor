using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    public List<string> purchasedUpgrades = new List<string>();
    List<ShopManager.Upgrade> playerUpgrades;

    public UpgradeData(List<ShopManager.Upgrade> upgrades)
    {
        playerUpgrades = upgrades;
        foreach (var upgrade in upgrades)
        {
            if (upgrade.isPurchased)
            {
                purchasedUpgrades.Add(upgrade.name);
            }
        }
    }

    public void ResetList()
    { purchasedUpgrades.Clear();

        foreach (var upgrade in playerUpgrades)
        {
            upgrade.isPurchased = false;
        }

    }

}