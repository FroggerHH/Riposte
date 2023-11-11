using System.Diagnostics;
using ItemDataManager;

namespace Riposte.Patch;

[HarmonyPatch]
public class KeepArbalestLoaded
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetWeaponLoaded)), HarmonyWrapSafe, HarmonyPostfix]
    public static void LoadWeapon_true(Player __instance, ItemDrop.ItemData weapon)
    {
        if (weapon == null || weapon.m_shared.m_animationState != Crossbow) return;
        __instance.m_weaponLoaded.Data()[$"IsLoaded"] = true.ToString();
        __instance.m_weaponLoaded.Data().Save();
    }

    [HarmonyPatch]
    public static class UnloadWeapon_false
    {
        [HarmonyPatch(typeof(Player), nameof(Humanoid.ResetLoadedWeapon)), HarmonyWrapSafe, HarmonyPrefix]
        public static void When_ResetLoadedWeapon(Humanoid __instance)
        {
            var pl = __instance as Player;
            if (pl?.m_weaponLoaded?.m_shared.m_animationState != Crossbow) return;
            if (new StackTrace().ToString().Contains("OnAttackTrigger") == false) return;
            pl.m_weaponLoaded.Data()[$"IsLoaded"] = false.ToString();
            pl.m_weaponLoaded.Data().Save();
        }

        // [HarmonyPatch(typeof(Player), nameof(Player.SetWeaponLoaded)), HarmonyWrapSafe, HarmonyPrefix]
        // public static bool When_SetWeaponLoaded(Player __instance, ItemDrop.ItemData weapon)
        // {
        //     if (weapon == null && __instance.m_weaponLoaded?.m_shared.m_animationState == Crossbow)
        //     {
        //         bool isLoaded = __instance.m_weaponLoaded.Data()[$"IsLoaded"] == true.ToString();
        //         if (isLoaded == false) return true;
        //
        //         Debug($"Prevented weapon {__instance.m_weaponLoaded.m_shared.m_name} "
        //               + "from being unloaded on SetWeaponLoaded(null)");
        //         return false;
        //     }
        //
        //     return true;
        // }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.IsWeaponLoaded)), HarmonyWrapSafe, HarmonyPostfix]
    public static void Override_IsWeaponLoaded_ForVisual(Player __instance, ref bool __result)
    {
        if (__instance.m_weaponLoaded?.m_shared.m_animationState != Crossbow) return;
        var savedValue = __instance.m_weaponLoaded.Data()[$"IsLoaded"];
        __result = savedValue == true.ToString();
        if (__result) __instance.m_nview.GetZDO().Set(ZDOVars.s_weaponLoaded, true);
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdateWeaponLoading)), HarmonyWrapSafe, HarmonyPrefix]
    public static bool UpdateWeaponLoading(Player __instance, [CanBeNull] ItemDrop.ItemData weapon)
    {
        if (weapon?.m_shared.m_animationState == Crossbow)
        {
            if (!weapon.m_shared.m_attack.m_requiresReload || __instance.IsReloadActionQueued()) return false;
            bool isLoaded = weapon.Data()[$"IsLoaded"] == true.ToString();
            if (isLoaded) __instance.SetWeaponLoaded(weapon);
            else __instance.QueueReloadAction();
            return false;
        }

        return true;
    }
}