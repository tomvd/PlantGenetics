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
    /// Plants with wintertrait grow until 0
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Temperature), MethodType.Getter)]
    public class GrowthRateFactor_Temperature
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
            {
                if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var cellTemp))
                {
                    __result = 1f;
                }
                else
                {
                    __result = 1f;
                    if (cellTemp > 42f)
                    {
                        __result = Mathf.InverseLerp(58f, 42f, cellTemp);
                    }
                }
                return false;
            }
            return true; // do the vanilla thang
        }
    }
    /// <summary>
    /// Plants with wintertrait go leafless at -999
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.LeaflessTemperatureThresh), MethodType.Getter)]
    public class LeaflessTemperatureThresh
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
                __result = -999;
        }
    }
}