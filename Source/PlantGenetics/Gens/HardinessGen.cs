using System.Linq;
using HarmonyLib;
using PlantGenetics.Comp;
using PlantGenetics.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Gens;

public static class HardinessGen
{
    
    public static float getHardinessModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'H'));
        return c switch
        {
            2 => 1f,
            > 2 => 2f,
            _ => 0f
        };
    }    
    
    /// <summary>
    /// Alter Hardiness of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Temperature), MethodType.Getter)]
    public class GrowthRateFactor_Temperature
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.getHardinessModifier();
            float minTemp = 0f - 15f * mod; // 0 -15 or -30
            float maxTemp = 58f + 21f * mod; // 58 79 or 100
            if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var tempResult))
            {
                __result = 1f;
            }
            else
            {
                if (tempResult < 6f)
                {
                    __result = Mathf.InverseLerp(minTemp, 6f, tempResult);
                }
                if (tempResult > 42f)
                {
                    __result = Mathf.InverseLerp(maxTemp, 42f, tempResult);
                }
                __result = 1f;
            }
            
        }
    }
    
    [HarmonyPatch(typeof(Plant), nameof(Plant.LeaflessTemperatureThresh), MethodType.Getter)]
    public class LeaflessTemperatureThresh
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.getHardinessModifier();
            __result *= 1f + mod;
        }
    }
}