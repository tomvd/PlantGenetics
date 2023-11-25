using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Gens;
/*
public static class SummerTrait
{
    public static bool hasSummerTrait(this Plant plant)
    {
        if (plant.def.GetModExtension<TraitExtension>() != null &&
            plant.def.GetModExtension<TraitExtension>().SpecialTrait != null)
            return plant.def.GetModExtension<TraitExtension>().SpecialTrait
                .Equals(InternalDefOf.Summer);
        else return false;
    }    
    
    /// <summary>
    /// Plants with summertrait grow slower below 10, but keeps growing up until 60
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Temperature), MethodType.Getter)]
    public class GrowthRateFactor_Temperature
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            if (__instance.hasSummerTrait())
            {
                float minTemp = 6f;
                float maxTemp = 64f;
                if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var tempResult))
                {
                    __result = 1f;
                }
                else
                {
                    if (tempResult < 10f)
                    {
                        __result = Mathf.InverseLerp(minTemp, 10f, tempResult);
                    }

                    if (tempResult > 60f)
                    {
                        __result = Mathf.InverseLerp(maxTemp, 60f, tempResult);
                    }

                    __result = 1f;
                }
            }
        }
    }
}*/