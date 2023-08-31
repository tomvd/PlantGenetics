using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantBeauty
{
    /// <summary>
    /// Alter Beauty of plants by genetics
    /// </summary>
    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
    public class GetStatValue
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Thing thing, StatDef stat)
        {
            if (thing.def.plant != null && (stat == StatDefOf.Beauty || stat == StatDefOf.BeautyOutdoors))
            {
                __result *= ((ThingWithComps)thing).GetComp<CompPlantGenetics>().getBeautyModifier();                
            }
        }
    }
}