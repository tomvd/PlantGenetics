using System;
using System.Text;
using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Patches;

public class PlantLightSensivity
{
    /// <summary>
    /// Alter LightSensitivity of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.GrowthRateFactor_Light), MethodType.Getter)]
    public class GrowthRateFactor_Light
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Plant __instance)
        {
            float mod = __instance.GetComp<CompPlantGenetics>().getLightSensitivityModifier();
            float glow = __instance.Map.glowGrid.GameGlowAt(__instance.Position);
            //return PlantUtility.GrowthRateFactorFor_Light(def, glow);
            if (__instance.def.plant.growMinGlow == __instance.def.plant.growOptimalGlow && glow == __instance.def.plant.growOptimalGlow)
            {
                __result = 1f;
                return;
            }
            __result = GenMath.InverseLerp((__instance.def.plant.growMinGlow * mod)  , Mathf.Clamp01((__instance.def.plant.growOptimalGlow * mod) + 0.01f), glow);
        }
    }
    
    [HarmonyPatch(typeof(Plant), nameof(Plant.GetInspectString))]
    public class GetInspectString
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, Plant __instance)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] lines = __result.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None
            );
            foreach (var line in lines)
            {
                if (line.Contains("PlantNeedsLightLevel".Translate()))
                {
                    float minLightLevel = (__instance.def.plant.growMinGlow * __instance.GetComp<CompPlantGenetics>().getLightSensitivityModifier());
                    stringBuilder.AppendLine("PlantNeedsLightLevel".Translate() + ": " + minLightLevel.ToStringPercent());
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }
            __result = stringBuilder.ToString().TrimEndNewlines();
        }
    }
}