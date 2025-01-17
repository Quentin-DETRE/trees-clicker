using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : BaseManager
{
    public static UpgradeManager Instance;

    public List<UpgradeObject> availableUpgrades { get; private set; } = new List<UpgradeObject>();
    void Awake() 
    {
        if (!CheckSingletonInstance(this, ref Instance))
        {
            return; // Instance already exists, so the new one is destroyed
        }
        LoadUpgradesFromJSON();
        InitializeUpgrades();
    }

private void LoadUpgradesFromJSON()
{
    TextAsset jsonFile = Resources.Load<TextAsset>("Upgrades");
    if (jsonFile != null)
    {
        string jsonContent = jsonFile.text;
        UpgradeArray upgradeArray = JsonUtility.FromJson<UpgradeArray>(jsonContent);
        availableUpgrades.Clear();
        availableUpgrades.AddRange(upgradeArray.upgrades);
    }
    else
    {
        Debug.LogError("Unable to load upgrades.json!");
    }
}


    private void InitializeUpgrades()
    {
        foreach (var upgrade in availableUpgrades)
        {
            InventoryManager.Instance.ownedUpgrades[upgrade.upgradeName] = 0;
        }
    }

    public void BuyUpgrade(string upgradeName)
    {
        if (!InventoryManager.Instance.ownedUpgrades.ContainsKey(upgradeName))
            return;

        var upgrade = availableUpgrades.Find(u => u.upgradeName == upgradeName);
        if (InventoryManager.Instance.Seeds >= upgrade.cost)
        {
            InventoryManager.Instance.RemoveSeeds(upgrade.cost);
            InventoryManager.Instance.ownedUpgrades[upgradeName]++;
            CheckForUpgradeSteps(upgrade);
            upgrade.cost = upgrade.cost*1.15;
            EconomyManager.Instance.UpdateSeedsPerSecond();
            UIManager.Instance.UpdateUpgradeButtonData(upgradeName);
        }
    }

    private void CheckForUpgradeSteps(UpgradeObject upgrade)
    {
        foreach (var step in upgrade.upgradeSteps)
        {
            if (InventoryManager.Instance.ownedUpgrades[upgrade.upgradeName] >= step.threshold)
            {
                EconomyManager.Instance.ApplyModifier(step.modifier);
            }
        }
    }

    public UpgradeObject GetUpgradeObject(string upgradeName)
    {
        return availableUpgrades.Find(u => u.upgradeName == upgradeName);
    }
}
