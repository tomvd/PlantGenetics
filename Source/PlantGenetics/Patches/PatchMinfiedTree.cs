using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PatchMinfiedTree
{
    /// <summary>
    /// Inject our own reverse designators when the vanilla ones are initialized
    /// </summary>
    [HarmonyPatch(typeof(MinifiedTree), "GetInspectString")]
    internal static class GetInspectStringPrefixPatch {
        [HarmonyPrefix]
        public static bool GetInspectStringPrefix(ref string __result, MinifiedTree __instance) {
            __result = "PlantWillDieIn".Translate(__instance.ticksTillDeath.ToStringTicksToPeriod().Named("time")) + "\n"
                + "DNA:" + __instance.InnerTree.getDNA();
            return false;
        }
    }
}

/*
public override string GetInspectString()
{
    return "MinifiedTreeWillDieIn".Translate(ticksTillDeath.ToStringTicksToPeriod().Named("time"));
}
*/