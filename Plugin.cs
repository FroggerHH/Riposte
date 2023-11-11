using BepInEx;
using BepInEx.Configuration;
using LocalizationManager;

namespace Riposte;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings")]
public class Plugin : BaseUnityPlugin
{
    public const string ModName = "Riposte",
        ModVersion = "1.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}",
        ModAuthor = "Frogger";

    public static ConfigEntry<int> timeToDoRiposte;

    public static bool isRiposteReady = false;

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);
        timeToDoRiposte = config("General", "Time to do riposte", 1000, "");


        Localizer.Load();
    }
}