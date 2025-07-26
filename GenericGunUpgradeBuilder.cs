using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UpgradeTest;
public class GenericGunUpgradeBuilder
{
    private static int id = 0xB00B5;
    IUpgradable gear;
    string name;
    Rarity rarity;
    Sprite icon;
    HexMap hexMap;
    List<UpgradeProperty> properties = new List<UpgradeProperty>();
    public void AddProperty(UpgradeProperty property)
    {
        properties.Add(property);
    }
    public void SetGear(IUpgradable gear)
    {
        this.gear = gear;
    }
    public void SetGear(Rarity gear)
    {
        rarity = gear;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public void NewHexMap(int width, int height)
    {
        hexMap = new HexMap(width, height);
    }
    public void SetPos(int x, int y, bool enabled, HexMap.Direction direction)
    {
        hexMap.nodes[y].nodes[x].enabled = enabled;
        hexMap.nodes[y].nodes[x].connections = direction;
    }
    public Upgrade Build()
    {
        if (icon == null)
        {
            icon = Sprite.Create(new Rect(0, 0, 50, 50), new Vector2(25, 25), 100, new Texture2D(50, 50));
        }
        // you know what they say, time to shit myself
        GenericGunUpgrade upgrade = ScriptableObject.CreateInstance<GenericGunUpgrade>();
        upgrade.Rarity = rarity;
        upgrade._name = name;
        upgrade.id = id; // :)
        upgrade.icon = Sprite.Create(new Rect(0, 0, 50, 50), new Vector2(25, 25), 100, new Texture2D(50, 50));
        upgrade.pattern = hexMap;
        upgrade.properties.properties = properties.ToArray();
        id++;
        gear.Info.upgrades = Plugin.AddToArray(gear.Info.upgrades, upgrade);
        return upgrade;
    }

    public static void AddUpgrades()
    {
        UpgradeProperty_Damage damage = new UpgradeProperty_Damage();
        damage.damage.data = new Range<float>(1.04f, 1.06f);
        damage.damage.method = OverrideType.Multiply;

        UpgradeProperty_Spread spread = new UpgradeProperty_Spread();
        spread.spreadSize.data = new Range<Vector2>(new Vector2(0.85f, 0.85f), new Vector2(0.9f, 0.9f));
        spread.spreadSize.method = OverrideType.Multiply;

        UpgradeProperty_MagazineSize magSize = new UpgradeProperty_MagazineSize();
        magSize.magazineSize.data = new Range<float>(1f, 3f);
        magSize.magazineSize.method = OverrideType.Add;

        UpgradeProperty_FireInterval fireRate = new UpgradeProperty_FireInterval();
        fireRate.fireInterval.data = new Range<float>(0.93f, 0.95f);
        fireRate.fireInterval.method = OverrideType.Multiply;

        GenericGunUpgradeBuilder builder = new GenericGunUpgradeBuilder();
        builder.SetName("Damage Dot");
        builder.SetGear(Plugin.bounceShotgun);
        builder.AddProperty(damage);
        builder.NewHexMap(1, 1);
        builder.SetPos(0, 0, true, HexMap.Direction.None);
        Plugin.upgrades.Add(builder.Build());

        builder = new GenericGunUpgradeBuilder();
        builder.SetName("Spread Dot");
        builder.SetGear(Plugin.bounceShotgun);
        builder.AddProperty(spread);
        builder.NewHexMap(1, 1);
        builder.SetPos(0, 0, true, HexMap.Direction.None);
        Plugin.upgrades.Add(builder.Build());

        builder = new GenericGunUpgradeBuilder();
        builder.SetName("Mag Dot");
        builder.SetGear(Plugin.bounceShotgun);
        builder.AddProperty(magSize);
        builder.NewHexMap(1, 1);
        builder.SetPos(0, 0, true, HexMap.Direction.None);
        Plugin.upgrades.Add(builder.Build());

        builder = new GenericGunUpgradeBuilder();
        builder.SetName("Fire Rate Dot");
        builder.SetGear(Plugin.bounceShotgun);
        builder.AddProperty(fireRate);
        builder.NewHexMap(1, 1);
        builder.SetPos(0, 0, true, HexMap.Direction.None);
        Plugin.upgrades.Add(builder.Build());
    }
}

