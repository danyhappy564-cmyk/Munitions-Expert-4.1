using BepInEx;
using EFT.InventoryLogic;
using SPT.Reflection.Patching;
using System.Reflection;

namespace IcyClawz.MagazineInspector;

[BepInPlugin("com.IcyClawz.MagazineInspector", "IcyClawz.MagazineInspector", "1.7.0")]
public class Plugin : BaseUnityPlugin
{
    private void Awake() =>
        new MagazinePatch().Enable();
}

internal class MagazinePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        typeof(Magazine).GetConstructors()[0];

    [PatchPostfix]
    private static void PatchPostfix(ref Magazine __instance) =>
        __instance.AddAmmoCountAttribute();
}
