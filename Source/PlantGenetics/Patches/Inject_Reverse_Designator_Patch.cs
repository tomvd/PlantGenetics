using HarmonyLib;
using Verse;

namespace PlantGenetics.Patches;

public class Inject_Reverse_Designator_Patch
{
    /// <summary>
    /// Inject our own reverse designators when the vanilla ones are initialized
    /// </summary>
    [HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
    internal static class ReverseDesignatorDatabase_Init_Patch {
        [HarmonyPostfix]
        public static void InjectReverseDesignators(ReverseDesignatorDatabase __instance) {
            __instance.desList.Add(new Designator_ClonePlant());
        }
    }
}