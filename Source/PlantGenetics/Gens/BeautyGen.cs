using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Gens;

public static class BeautyGen
{
    
    public static float getBeautyModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'B'));
        if (plant.def.plant.purpose == PlantPurpose.Beauty)
        {
         // beauty plants always start with 1 B gen
            return c switch
            {
                0 => 0.75f,
                1 => 1.0f,
                2 => 2f,
                > 2 => 3f,
                _ => 1.0f
            };
        }
        return c switch
        {
            0 => 1.0f,
            1 => 2f,
            > 1 => 3f,
            _ => 1.0f
        };
    }  
    
    /// <summary>
    /// Alter Beauty of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
    public class GetStatValue
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Thing thing, StatDef stat)
        {
            if (thing is Plant plant && (stat == StatDefOf.Beauty || stat == StatDefOf.BeautyOutdoors))
            {
                __result *= plant.getBeautyModifier();                
            }
        }
    }
}