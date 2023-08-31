using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantFertilitySensivity
{
    /// <summary>
    /// Alter GrowthRateFactor_Fertility of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Fertility), MethodType.Getter)]
    public class GrowthRateFactor_Fertility
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.GetComp<CompPlantGenetics>().getFertilitySensitivityModifier();
            __result = __instance.Map.fertilityGrid.FertilityAt(__instance.Position) *
                (__instance.def.plant.fertilitySensitivity * mod) + (1f - (__instance.def.plant.fertilitySensitivity * mod));
        }
    }
}