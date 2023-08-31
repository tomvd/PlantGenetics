using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantGrowthRate
{
    /// <summary>
    /// Alter growthRate of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRate), MethodType.Getter)]
    public class GrowthRate
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            __result *= __instance.GetComp<CompPlantGenetics>().getGrowthRateModifier();
        }
    }
}