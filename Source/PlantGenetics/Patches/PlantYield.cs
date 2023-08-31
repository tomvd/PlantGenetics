using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantYield
{
    /// <summary>
    /// Alter growthRate of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.YieldNow))]
    public class YieldNow
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, Plant __instance)
        {
            __result = GenMath.RoundRandom(__result * __instance.GetComp<CompPlantGenetics>().getYieldModifier());
        }
    }
}