using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Gens;

public static class YieldGen
{
    public static float getYieldModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'Y'));
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
    /// Alter yield of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.YieldNow))]
    public class YieldNow
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, Plant __instance)
        {
            __result = GenMath.RoundRandom(__result * __instance.getYieldModifier());
        }
    }
}