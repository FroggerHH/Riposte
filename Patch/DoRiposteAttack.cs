﻿namespace Riposte.Patch;

[HarmonyPatch]
public class DoRiposteAttack
{
    [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
    [HarmonyPostfix]
    public static void EffectListCreated(Attack __instance, bool __result)
    {
        if (__result == false || isRiposteReady == false ||
            __instance.m_character != m_localPlayer ||
            __instance.m_zanim.IsOwner() == false) return;

        //TODO: Change damage ShowText to show that attack is Riposte
        //TODO: Change attack animation
    }
}