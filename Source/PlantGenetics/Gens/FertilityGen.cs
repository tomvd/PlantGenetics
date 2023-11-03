using System.Linq;
using HarmonyLib;
using RimWorld;

namespace PlantGenetics.Gens;

public static class FertilityGen
{/*
    public static float getFertilitySensitivityModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'F'));
        switch (c)
        {
            case 0:
                return 1.2f; // missing the F gen!
            case 1:
                return 1f; // normal
            case 2:
                return 0.5f;
            default: // more than 2
                return 0f;
        }
    }
    
    /// <summary>
    /// Alter GrowthRateFactor_Fertility of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Fertility), MethodType.Getter)]
    public class GrowthRateFactor_Fertility
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.getFertilitySensitivityModifier();
            __result = __instance.Map.fertilityGrid.FertilityAt(__instance.Position) *
                (__instance.def.plant.fertilitySensitivity * mod) + (1f - (__instance.def.plant.fertilitySensitivity * mod));
        }
    }*/
}