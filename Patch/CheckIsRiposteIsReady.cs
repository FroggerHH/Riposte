namespace Riposte.Patch;

[HarmonyPatch]
public class CheckIsRiposteIsReady
{
    [HarmonyPatch(typeof(EffectList), nameof(EffectList.Create))]
    [HarmonyPostfix]
    public static async void EffectListCreated(EffectList __instance)
    {
        if (!m_localPlayer || __instance != m_localPlayer.m_perfectBlockEffect) return;
        isRiposteReady = true;
        await Task.Delay(timeToDoRiposte.Value);
        isRiposteReady = false;
    }
}