using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace UpgradeTest;

[HarmonyPatch(typeof(Global), nameof(Global.Initialize))]
public class GlobalPatch
{
    static void Postfix()
    {
        TextBlocks.TextBlockGroup group = new TextBlocks.TextBlockGroup(0x8008);
        group.blocks = [
        new TextBlocks.TextBlock("When this upgrade touches <color=#FF000E>EXACTLY</color> one other upgrade, it <color=#FFE4B4>copies it</color>, acting as a 2nd instance of that upgrade.", "copyupgrade0")
        ];
        TextBlocks.strings.Add("copyupgrade", group);

        Debug.Log($"Plugin 'go fuck yourself' is READY TO GOO!");

        Texture2D texture2D = new Texture2D(100, 100);
        for (int h = 0; h < 100; h++)
        {
            for (int j = 0; j < 100; j++)
            {
                texture2D.SetPixel(h, j, new Color(1, 1, 1, 1));
            }
        }
        Sprite sprite = Sprite.Create(texture2D, new Rect(0,0,texture2D.width,texture2D.height), new Vector2(.5f,.5f), 100);

        foreach (IUpgradable upgradable in Global.Instance.AllGear)
        {
            if (upgradable is BounceShotgun)
            {
                Plugin.bounceShotgun = (BounceShotgun)upgradable;
            }
        }

        CopyUpgrade copyupgrade = ScriptableObject.CreateInstance<CopyUpgrade>();
        copyupgrade.Rarity = Rarity.Exotic;
        copyupgrade._name = "Blueprint";
        copyupgrade.id = 0x8007; // :)
        copyupgrade.icon = sprite;
        copyupgrade.pattern = new HexMap(1, 1);
        copyupgrade.pattern.nodes[0].nodes[0].enabled = true;
        Plugin.upgrades.Add(copyupgrade);
        Global.Instance.Info.upgrades = Plugin.AddToArray(Global.Instance.Info.upgrades, copyupgrade);

        int i = 0x80091;
        foreach (IUpgradable gear in Global.Instance.AllGear)
        {
            if (gear is GrenadeGear || (gear is Gun && gear is not IActivatedAbility))
            {
                CopyGearUpgrade upgrade = ScriptableObject.CreateInstance<CopyGearUpgrade>();
                upgrade.Rarity = Rarity.Exotic;
                upgrade._name = "Blueprint";
                upgrade.id = i; // :)
                upgrade.icon = sprite;
                upgrade.pattern.nodes[0].nodes[0].enabled = true;
                Plugin.upgrades.Add(upgrade);
                gear.Info.upgrades = Plugin.AddToArray(gear.Info.upgrades, upgrade);
                i++;
            }
        }
    }
}

[HarmonyPatch(typeof(Roachard), nameof(Roachard.Interact))]
public class RoachardPatch
{
    static void Prefix()
    {
        PlayerResource playerResource;
        if (PlayerResource.TryGetResource("oyster", out playerResource) && PlayerData.Instance.GetResource(playerResource) > 0)
        {
            // 1 oyster about to be removed, grant a random upgrade
            while (true)
            {
                Upgrade upgrade = Plugin.upgrades[UnityEngine.Random.Range(0, Plugin.upgrades.Count)];
                IUpgradable gearFromID = PlayerData.GetGearFromID(PlayerData.FindGearInfo(upgrade).ID);
                if (!PlayerData.IsGearUnlocked(gearFromID))
                    continue;
                var iUpgrade = new UpgradeInstance(upgrade, gearFromID);
                PlayerData.CollectInstance(iUpgrade);
                iUpgrade.Unlock();
                break;
            }
        }
    }
}