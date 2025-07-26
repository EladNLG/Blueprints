using System;
using System.Collections.Generic;
using Pigeon;
using Pigeon.Movement;
using UnityEngine;
namespace UpgradeTest;

public class CopyUpgrade : PlayerUpgrade
{
    public override string DescriptionKey => "copyupgrade";

    public override EffectType EffectType => EffectType.Normal;

    public override Type UpgradeType => Type.Normal;

    public override HexMap Pattern => pattern;

    public override UpgradePropertyList Properties
    {
        get
        {
            return new UpgradePropertyList
            {
                properties = new UpgradeProperty[]
                {
                }
            };
        }
    }

    public static UpgradeInstance GetUpgradeTouchingThis(IUpgradable prefab, UpgradeInstance upgrade)
    {
        UpgradeProperty.GetGearPrefab(ref prefab);
        HashSet<ValueTuple<int, int>> hashSet;
        UpgradeInstance target = null;
        using (PlayerData.GetSurroundingCells(prefab, upgrade, out hashSet))
        {
            foreach (ValueTuple<int, int> valueTuple in hashSet)
            {
                int item = valueTuple.Item1;
                int item2 = valueTuple.Item2;
                UpgradeInstance upgradeInstance = PlayerData.GetEquippedUpgrade(prefab, item, item2);
                if (upgradeInstance != null)
                {
                    if (target != null && target != upgradeInstance) // touching two different upgrades
                        return null;
                    target = upgradeInstance;
                }
            }
        }
        return target;
    }

    public static void AddDynamicProperty(ref string properties, string label, string value, bool isGood)
    {
        StringBuilder stringBuilder = new StringBuilder(Global.charBuffer);
        if (properties != null)
        {
            stringBuilder.Add('\n');
        }

        stringBuilder.Add(isGood ? "<color=#3EFF77>" : "<color=#FF000E>");
        stringBuilder.Add(label);
        stringBuilder.Add(": <b>");
        stringBuilder.Add(value);
        stringBuilder.Add("</b>");
        properties += stringBuilder;
    }

    public override void ModifyDisplayedProperties(ref string properties, UpgradeInstance instance)
    {
        if (instance == null)
            return;
        IUpgradable gearFromID = PlayerData.GetGearFromID(PlayerData.FindGearInfo(this).ID);
        UpgradeInstance target = GetUpgradeTouchingThis(Player.LocalPlayer.Character, instance);
        AddDynamicProperty(ref properties, "Copying", target == null ? "Nothing!" : target.Upgrade.Name, target != null);
    }

    public override void Apply(Player player, UpgradeInstance instance)
    {
        UpgradeInstance target = GetUpgradeTouchingThis(player.Character, instance);
        if (target != null)
            Debug.Log("applying upgrade " + target.Upgrade);
        if (target == null)
            return;
        if (target.Upgrade is PlayerUpgrade)
        {
            Debug.Log("applying upgrade " + target.Upgrade.Name);
            (target.Upgrade as PlayerUpgrade).Apply(player, target);
        }
        else if (target.Upgrade is GearUpgrade) // gear upgrades in player grid? this is a util upgrade - iterate over all gear and apply it lelelel
        {
            foreach (IGear gear in player.Gear)
            {
                Debug.Log(gear);
                Debug.Log("applying upgrade " + target.Upgrade.Name);
                (target.Upgrade as GearUpgrade).Apply(gear, target);
            }
        }
    }

    public override void Remove(Player player, Player prefab, UpgradeInstance instance)
    {
    }
}


public class CopyGearUpgrade : GearUpgrade
{
    public override string DescriptionKey => "copyupgrade";

    public override EffectType EffectType => EffectType.Normal;

    public override Type UpgradeType => Type.Normal;

    public override HexMap Pattern => pattern;

    public HexMap pattern = new HexMap(1, 1);

    public override UpgradePropertyList Properties
    {
        get
        {
            return new UpgradePropertyList
            {
                properties = new UpgradeProperty[]
                {
                }
            };
        }
    }

    public static UpgradeInstance GetUpgradeTouchingThis(IUpgradable prefab, UpgradeInstance upgrade)
    {
        UpgradeProperty.GetGearPrefab(ref prefab);
        HashSet<ValueTuple<int, int>> hashSet;
        UpgradeInstance target = null;
        using (PlayerData.GetSurroundingCells(prefab, upgrade, out hashSet))
        {
            foreach (ValueTuple<int, int> valueTuple in hashSet)
            {
                int item = valueTuple.Item1;
                int item2 = valueTuple.Item2;
                UpgradeInstance upgradeInstance = PlayerData.GetEquippedUpgrade(prefab, item, item2);
                if (upgradeInstance != null)
                {
                    if (target != null && target != upgradeInstance) // touching two different upgrades
                        return null;
                    target = upgradeInstance;
                }
            }
        }
        return target;
    }

    public static void AddDynamicProperty(ref string properties, string label, string value, bool isGood)
    {
        StringBuilder stringBuilder = new StringBuilder(Global.charBuffer);
        if (properties != null)
        {
            stringBuilder.Add('\n');
        }

        stringBuilder.Add(isGood ? "<color=#3EFF77>" : "<color=#FF000E>");
        stringBuilder.Add(label);
        stringBuilder.Add(": <b>");
        stringBuilder.Add(value);
        stringBuilder.Add("</b>");
        properties += stringBuilder;
    }

    public override void ModifyDisplayedProperties(ref string properties, UpgradeInstance instance)
    {
        if (instance == null)
            return;
        IUpgradable gearFromID = PlayerData.GetGearFromID(PlayerData.FindGearInfo(this).ID);
        UpgradeInstance target = GetUpgradeTouchingThis(gearFromID, instance);
        AddDynamicProperty(ref properties, "Copying", target == null ? "Nothing!" : target.Upgrade.Name, target != null);
    }


    public override void Apply(IGear gear, UpgradeInstance instance)
    {
        UpgradeInstance target = GetUpgradeTouchingThis(gear, instance);
        if (target != null)
            Debug.Log("applying upgrade " + target.Upgrade);
        if (target == null)
            return;
        if (target.Upgrade is GearUpgrade)
        {
            Debug.Log("applying upgrade " + target.Upgrade.Name);
            (target.Upgrade as GearUpgrade).Apply(gear, target);
        }
        else // gear upgrades in player grid? this is a util upgrade - iterate over all gear and apply it lelelel
        {
            Debug.LogError("Upgrade " + target.Upgrade + " not supported!");
        }
    }

    public override void Remove(IGear gear, IGear prefab, UpgradeInstance instance)
    {
    }
}