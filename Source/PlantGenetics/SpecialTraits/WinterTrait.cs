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
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
            {
                if (!GenTemperature.TryGetTemperatureForCell(__instance.Position, __instance.Map, out var tempResult))
                {
                    __result = 1f;
                }
                else
                {
                    __result = 1f;
                    
                    if (tempResult < -10f)
                    {
                        __result = Mathf.InverseLerp(-16f, -10f, tempResult);
                    }

                    if (tempResult > 25f)
                    {
                        __result = Mathf.InverseLerp(41f, 25f, tempResult);
                    }
                }
                return false;
            }
            return true; // do the vanilla thang
        }
    }
    
    /*
     * public virtual float GrowthRate
{
	get
	{
		if (Blighted)
		{
			return 0f;
		}
		if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map))
		{
			return 0f;
		}
		return GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light * GrowthRateFactor_NoxiousHaze;
	}
}
     */
    
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRate), MethodType.Getter)]
    public class GrowthRate
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Plant __instance)
        {
            if (__instance.hasWinterTrait())
            {
                if (__instance.Blighted)
                {
                    __result = 0f;
                }
                // removed the sowing season now requirement
                __result = __instance.GrowthRateFactor_Fertility * __instance.GrowthRateFactor_Temperature * __instance.GrowthRateFactor_Light * __instance.GrowthRateFactor_NoxiousHaze;                
                return false;
            }
            return true; // do the vanilla thang
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