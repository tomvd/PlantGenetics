using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;

namespace PlantGenetics.Gens;

public static class GrowthGen
{
    public static float getGrowthRateModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'G'));
        return c switch
        {
            0 => 0.8f,
            1 => 1.0f,
            2 => 1.2f,
            > 2 => 2f,
            _ => 1.0f
        };
    }
    
    /// <summary>
    /// Alter growthRate of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRate), MethodType.Getter)]
    public class GrowthRate
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            __result *= __instance.getGrowthRateModifier();
        }
    }
}