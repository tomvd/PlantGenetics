using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Gens;

public static class WinterTrait
{
    public static bool hasWinterTrait(this Plant plant)
    {
        if (plant.def.GetModExtension<TraitExtension>() != null &&
            plant.def.GetModExtension<TraitExtension>().SpecialTrait != null)
            return plant.def.GetModExtension<TraitExtension>().SpecialTrait
                .Equals(InternalDefOf.Winter);
        else return false;
    }    
    
    /// <summary>
    /// Plants with wintertrait grow slower below -10, but already above 25
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Temperature), MethodType.Getter)]
    public class GrowthRateFactor_Temperature
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
            {
                float minTemp = -16f;
                float maxTemp = 41f;
                if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var tempResult))
                {
                    __result = 1f;
                }
                else
                {
                    if (tempResult < -10f)
                    {
                        __result = Mathf.InverseLerp(minTemp, -10f, tempResult);
                    }

                    if (tempResult > 25f)
                    {
                        __result = Mathf.InverseLerp(maxTemp, 25f, tempResult);
                    }

                    __result = 1f;
                }
            }
        }
    }
    
    /// <summary>
    /// Plants with wintertrait go leafless at -50
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.LeaflessTemperatureThresh), MethodType.Getter)]
    public class LeaflessTemperatureThresh
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
                __result = -50f;
        }
    }
}