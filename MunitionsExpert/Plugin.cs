using BepInEx;
using BepInEx.Configuration;
using EFT.HandBook;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IcyClawz.MunitionsExpert;

[BepInPlugin("com.IcyClawz.MunitionsExpert", "IcyClawz.MunitionsExpert", "1.7.0")]
public class Plugin : BaseUnityPlugin
{
    private static ConfigEntry<bool> ColorizeConfig { get; set; }
    private static ConfigEntry<ColorName>[] ArmorClassColorConfigs { get; set; }
    private static ConfigEntry<bool> FleaAmmoStatsConfig { get; set; }
    private static ConfigEntry<string> FleaAmmoStatsColorConfig { get; set; }

    private void Awake()
    {
        const string SECTION = "Colorize Icon Backgrounds";
        ColorizeConfig = Config.Bind(SECTION, "", true, new ConfigurationManagerAttributes { Order = 7 });
        ArmorClassColorConfigs = [
            Config.Bind(SECTION, "Unarmored", ColorName.Purple, new ConfigurationManagerAttributes { Order = 6 }),
            Config.Bind(SECTION, "Class 1", ColorName.Blue, new ConfigurationManagerAttributes { Order = 5 }),
            Config.Bind(SECTION, "Class 2", ColorName.Cyan, new ConfigurationManagerAttributes { Order = 4 }),
            Config.Bind(SECTION, "Class 3", ColorName.Green, new ConfigurationManagerAttributes { Order = 3 }),
            Config.Bind(SECTION, "Class 4", ColorName.Yellow, new ConfigurationManagerAttributes { Order = 2 }),
            Config.Bind(SECTION, "Class 5", ColorName.Orange, new ConfigurationManagerAttributes { Order = 1 }),
            Config.Bind(SECTION, "Class 6+", ColorName.Red, new ConfigurationManagerAttributes { Order = 0 }),
        ];

        const string FLEA_SECTION = "Flea Market Ammo Stats";
        FleaAmmoStatsConfig = Config.Bind(FLEA_SECTION, "Show penetration/damage", true,
            new ConfigDescription(
                "Draws [penetration/damage] over ammo and ammo pack icons in flea market offers.",
                null, new ConfigurationManagerAttributes { Order = 1 }));
        FleaAmmoStatsColorConfig = Config.Bind(FLEA_SECTION, "Text color", "#b6c1c7",
            new ConfigDescription(
                "Hex color of the overlay text, the same format the game uses in its own labels.",
                null, new ConfigurationManagerAttributes { Order = 0 }));

        new StaticIconsPatch().Enable();
        new AmmoTemplatePatch().Enable();
        new ItemViewPatch().Enable();
        new EntityIconPatch().Enable();
        new RagfairOfferItemViewPatch().Enable();
    }

    internal static bool Colorize => ColorizeConfig.Value;

    internal static bool ShowFleaAmmoStats => FleaAmmoStatsConfig.Value;

    internal static string FleaAmmoStatsColor => FleaAmmoStatsColorConfig.Value;

    internal static Color GetArmorClassColor(int index)
    {
        index = Mathf.Clamp(index, 0, ArmorClassColorConfigs.Length - 1);
        return ColorCache.Get(ArmorClassColorConfigs[index].Value);
    }
}

internal class StaticIconsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        typeof(StaticIcons).GetMethod("GetAttributeIcon", BindingFlags.Public | BindingFlags.Instance);

    [PatchPrefix]
    private static bool PatchPrefix(ref Sprite __result, Enum id) =>
        (__result = IconCache.Get(id)) is null;
}

internal class AmmoTemplatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        typeof(AmmoTemplate).GetMethod("GetCachedReadonlyQualities", BindingFlags.Public | BindingFlags.Instance);

    [PatchPrefix]
    private static bool PatchPrefix(ref AmmoTemplate __instance) =>
        __instance._cachedQualities is null;

    [PatchPostfix]
    private static void PatchPostfix(ref List<ItemAttribute> __result, ref AmmoTemplate __instance)
    {
        if (__result is null)
            __result = __instance._cachedQualities;
        else
            __instance.AddExtraAttributes();
    }
}

internal class ItemViewPatch : ModulePatch
{
    private static readonly FieldInfo BackgroundColorField =
        typeof(ItemView).GetField("BackgroundColor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    protected override MethodBase GetTargetMethod() =>
        typeof(ItemView).GetMethod("UpdateColor", BindingFlags.Public | BindingFlags.Instance);

    [PatchPrefix]
    private static void PatchPrefix(ref ItemView __instance)
    {
        if (!Plugin.Colorize || __instance.Item is not Ammo ammo || ammo.PenetrationPower <= 0)
            return;
        int armorClass = ammo.AmmoTemplate.GetPenetrationArmorClass();
        BackgroundColorField.SetValue(__instance, Plugin.GetArmorClassColor(armorClass));
    }
}

internal class EntityIconPatch : ModulePatch
{
    private static readonly FieldInfo ColorPanelField =
        typeof(EntityIcon).GetField("_colorPanel", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    protected override MethodBase GetTargetMethod() =>
        typeof(EntityIcon).GetMethod("Show", BindingFlags.Public | BindingFlags.Instance);

    [PatchPostfix]
    private static void PatchPostfix(ref EntityIcon __instance, Item item)
    {
        if (!Plugin.Colorize || item is not Ammo ammo || ammo.PenetrationPower <= 0)
            return;
        if (ColorPanelField.GetValue(__instance) is not Image image)
            return;
        int armorClass = ammo.AmmoTemplate.GetPenetrationArmorClass();
        image.color = Plugin.GetArmorClassColor(armorClass);
    }
}

// Flea market offers draw their item through RagfairOfferItemView, which hides the
// caption label the grid views use for the item name (UpdateInfo turns it off). That
// leaves a text element sitting unused on top of every offer icon, so borrow it to show
// the two numbers that actually decide a purchase.
internal class RagfairOfferItemViewPatch : ModulePatch
{
    // Caption is protected on GridItemView, two levels up from RagfairOfferItemView.
    private static readonly FieldInfo CaptionField =
        typeof(GridItemView).GetField("Caption", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    protected override MethodBase GetTargetMethod() =>
        typeof(RagfairOfferItemView).GetMethod("UpdateInfo", BindingFlags.Public | BindingFlags.Instance);

    [PatchPostfix]
    private static void PatchPostfix(RagfairOfferItemView __instance)
    {
        // UpdateInfo has already switched the caption off for us, so anything that is not
        // ammo simply stays hidden - which is also what keeps pooled views from carrying a
        // stale overlay onto the next item they are reused for.
        if (!Plugin.ShowFleaAmmoStats || !__instance.IsSearched)
            return;
        if (CaptionField?.GetValue(__instance) is not TextMeshProUGUI caption)
            return;
        if (GetAmmo(__instance.Item) is not Ammo ammo || ammo.PenetrationPower <= 0)
            return;
        caption.text = $"<color={Plugin.FleaAmmoStatsColor}>[{ammo.PenetrationPower}/{ammo.Damage}]</color>";
        caption.gameObject.SetActive(true);
    }

    // Most flea ammo is listed as packs, so read the round out of the box as well.
    private static Ammo GetAmmo(Item item) => item switch
    {
        Ammo ammo => ammo,
        // GetItemAtPosition throws past the end of the stack, hence the count check.
        AmmoBox box when box.Count > 0 => box.GetBulletAtPosition(0),
        _ => null,
    };
}
