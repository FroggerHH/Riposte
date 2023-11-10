using System.Diagnostics;
using ItemDataManager;

namespace Riposte.Patch;

[HarmonyPatch]
public class KeepArbalestLoaded
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetWeaponLoaded)), HarmonyWrapSafe, HarmonyPostfix]
    public static void SetWeaponLoadedPatch(Player __instance, ItemDrop.ItemData weapon)
    {
        if (__instance.m_weaponLoaded?.m_shared.m_animationState != Crossbow) return;
        __instance.m_weaponLoaded.Data()[$"IsLoaded"] = (weapon != null).ToString();
        __instance.m_weaponLoaded.Data().Save();
    }

    [HarmonyPatch(typeof(Player), nameof(Humanoid.ResetLoadedWeapon)), HarmonyWrapSafe, HarmonyPrefix]
    public static void UnequipItemPatch(Humanoid __instance)
    {
        var pl = __instance as Player;
        if (pl?.m_weaponLoaded?.m_shared.m_animationState != Crossbow) return;
        if (new StackTrace().ToString().Contains("OnAttackTrigger") == false) return;
        pl.m_weaponLoaded.Data()[$"IsLoaded"] = false.ToString();
        pl.m_weaponLoaded.Data().Save();
    }

    [HarmonyPatch(typeof(Player), nameof(Player.IsWeaponLoaded)), HarmonyWrapSafe, HarmonyPostfix]
    public static void IsWeaponLoadedPatch(Player __instance, ref bool __result)
    {
        if (__instance.m_weaponLoaded?.m_shared.m_animationState != Crossbow) return;
        var savedValue = __instance.m_weaponLoaded.Data()[$"IsLoaded"];
        __result = savedValue == true.ToString();
    }

    [HarmonyPatch(typeof(Player), nameof(Player.QueueReloadAction)), HarmonyWrapSafe, HarmonyPrefix]
    public static bool QueueReloadActionPatch(Player __instance)
    {
        var currentWeapon = __instance.GetCurrentWeapon();
        if (currentWeapon != null &&
            currentWeapon.m_shared.m_animationState == Crossbow &&
            currentWeapon.Data()[$"IsLoaded"] == true.ToString())
        {
            __instance.SetWeaponLoaded(currentWeapon);
            return false;
        }

        return true;
    }
}