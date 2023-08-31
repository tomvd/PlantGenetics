using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Patches;

public class PlantHardiness
{
    /// <summary>
    /// Alter Hardiness of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Temperature), MethodType.Getter)]
    public class GrowthRateFactor_Temperature
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.GetComp<CompPlantGenetics>().getHardinessModifier();
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
            float mod = __instance.GetComp<CompPlantGenetics>().getHardinessModifier();
            __result *= 1f + mod;
        }
    }
}