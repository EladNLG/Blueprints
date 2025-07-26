using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Pigeon;
using Pigeon.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

namespace UpgradeTest;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static List<Upgrade> upgrades = new List<Upgrade>();
    public static BounceShotgun bounceShotgun;
    public static T[] AddToArray<T>(T[] array, T item)
    {
        int newSize = array.Length + 1;
        T[] newArray = new T[newSize];
        Array.Copy(array, newArray, array.Length);
        newArray[newSize - 1] = item;
        return newArray;
    }
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        var harmony = new Harmony("me.eladnlg.fuck");
        harmony.PatchAll();
        InputActionMap _actionMap = new InputActionMap("giveUpgrade");
        var action = _actionMap.AddAction("fuck");
        action.AddBinding("Keyboard/F1");
        action.performed += Action_performed;
        var action2 = _actionMap.AddAction("fuck2");
        action2.AddBinding("Keyboard/F2");
        action2.performed += Action2_performed;
        _actionMap.Enable();
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            IUpgradable gearFromID = PlayerData.GetGearFromID(PlayerData.FindGearInfo(upgrade).ID);
            var iUpgrade = new UpgradeInstance(upgrade, gearFromID);
            PlayerData.CollectInstance(iUpgrade);
            iUpgrade.Unlock();
        }
    }
    private void Action2_performed(InputAction.CallbackContext obj)
    {
        PlayerResource resource;
        if (PlayerResource.TryGetResource("oyster", out resource))
        {
            PlayerData.Instance.AddResource(resource, 1);
        }
    }
}
