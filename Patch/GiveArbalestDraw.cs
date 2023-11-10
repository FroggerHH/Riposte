namespace Riposte.Patch;

// ReSharper disable once IdentifierTypo
[HarmonyPatch]
public class GiveArbalestDraw
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyPostfix] [HarmonyWrapSafe]
    public static void Postfix()
    {
        var crossbow = ZNetScene.instance.GetItem("CrossbowArbalest").m_itemData.m_shared;
        crossbow.m_attack.m_bowDraw = true;
        crossbow.m_attack.m_drawAnimationState = "bow_aim";
        crossbow.m_attack.m_drawStaminaDrain = 4;
        crossbow.m_attack.m_drawDurationMin = 0f;
    }
}