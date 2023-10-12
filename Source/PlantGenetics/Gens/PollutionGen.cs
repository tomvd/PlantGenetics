using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Gens;

public static class PollutionGen
{
    public static float getPollutionResistance(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'P'));
        if (c == 1) return 1f; // polluted soil immunity  
        if (c > 1) return 2f; // cleans polluted areas
        return 0f;
    }
    
    // TODO patch ? CompPlantableCanPlantAt(IntVec3 cell, Map map)
    // TODO patch
    /*PollutionUtility
     * public static bool CanPlantAt(ThingDef plantDef, IPlantToGrowSettable settable)
{
	if (plantDef.plant.RequiresNoPollution)
	{
		foreach (IntVec3 cell in settable.Cells)
		{
			if (!cell.IsPolluted(settable.Map))
			{
				return true;
			}
		}
		return false;
	}
	if (plantDef.plant.RequiresPollution)
	{
		foreach (IntVec3 cell2 in settable.Cells)
		{
			if (cell2.IsPolluted(settable.Map))
			{
				return true;
			}
		}
		return false;
	}
	return true;
}
     */
    
    //This patch controls the dyin from pollution
    [HarmonyPatch(typeof(Plant), nameof(Plant.DyingFromPollution), MethodType.Getter)]
    public class Patch_DyingFromPollution
    {
	    public static bool Prefix(ref bool __result, Plant __instance)
	    {
		    if (__instance.getPollutionResistance() > 0)
		    {
			    __result = false;
			    return false;
		    }

		    return true;
	    }
    }
    /*
     * TODO plant.ticklong - if p > 1 and mature: unpollute tile
     */
}