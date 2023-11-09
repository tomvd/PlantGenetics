using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PlantGenetics.Gens;

public static class PollutionGen
{/*
    public static float getPollutionResistance(this Plant plant)
    {
	    if (plant.getDNA() == null) return 0;
        int c = plant.getDNA().Count(f => (f == 'P'));
        if (c == 1) return 1f; // polluted soil immunity  
        if (c > 1) return 2f; // cleans polluted areas
        return 0f;
    }
    
    
    [HarmonyPatch(typeof(CompPlantable), nameof(CompPlantable.CanPlantAt))]
    public class Patch_CanPlantAtOne
    {
	    [HarmonyPrefix]
	    public static bool Prefix(ref AcceptanceReport __result, CompPlantable __instance, IntVec3 cell, Map map)
	    {
		    if (__instance.parent is Plant plant && plant.getPollutionResistance() > 0)
		    {
			    if (!cell.IsValid || cell.Fogged(map))
			    {
				    __result = false;
				    return false;
			    }
			    Thing blockingThing;
			    AcceptanceReport acceptanceReport = __instance.Props.plantDefToSpawn.CanEverPlantAt(cell, map, out blockingThing, canWipePlantsExceptTree: true);
			    if (!acceptanceReport.Accepted && acceptanceReport.Reason != null && !acceptanceReport.Reason.Equals("MessageWarningPollutedCell".Translate())) // Adamas: Added extra condition here
			    {
				    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + acceptanceReport.Reason.CapitalizeFirst();
				    return false;
			    }
			    if (__instance.Props.plantDefToSpawn.plant.interferesWithRoof && cell.Roofed(map))
			    {
				    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + "BlockedByRoof".Translate().CapitalizeFirst();
				    return false;
			    }
			    Thing thing = PlantUtility.AdjacentSowBlocker(__instance.Props.plantDefToSpawn, cell, map);
			    if (thing != null)
			    {
				    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + "AdjacentSowBlocker".Translate(thing);
				    return false;
			    }
			    if (__instance.Props.plantDefToSpawn.plant.minSpacingBetweenSamePlant > 0f)
			    {
				    foreach (Thing item in map.listerThings.ThingsOfDef(__instance.Props.plantDefToSpawn))
				    {
					    if (item.Position.InHorDistOf(cell, __instance.Props.plantDefToSpawn.plant.minSpacingBetweenSamePlant))
					    {
						    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + "TooCloseToOtherPlant".Translate(item);
						    return false;
					    }
				    }
				    foreach (Thing item2 in map.listerThings.ThingsOfDef(__instance.parent.def))
				    {
					    CompPlantable compPlantable = item2.TryGetComp<CompPlantable>();
					    if (compPlantable == null || compPlantable.PlantCells.NullOrEmpty())
					    {
						    continue;
					    }
					    for (int i = 0; i < compPlantable.PlantCells.Count; i++)
					    {
						    if (compPlantable.PlantCells[i].InHorDistOf(cell, __instance.Props.plantDefToSpawn.plant.minSpacingBetweenSamePlant))
						    {
							    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + "TooCloseToOtherSeedPlantCell".Translate(item2.GetCustomLabelNoCount(includeHp: false));
							    return false;
						    }
					    }
				    }
			    }
			    List<Thing> list = map.thingGrid.ThingsListAt(cell);
			    for (int j = 0; j < list.Count; j++)
			    {
				    if (list[j] is Building_PlantGrower)
				    {
					    __result = "CannotPlantThing".Translate(__instance.parent) + ": " + "BlockedBy".Translate(list[j]).CapitalizeFirst();
					    return false;
				    }
			    }
			    __result = true;
			    return false;
		    }
		    return true; // do the vanilla thang
	    }
    }    
    
    [HarmonyPatch(typeof(PollutionUtility), nameof(PollutionUtility.CanPlantAt))]
    public class Patch_CanPlantAtTwo
    {
	    [HarmonyPrefix]
	    public static bool Prefix(ref bool __result, Plant __instance)
	    {
		    if (__instance.getPollutionResistance() > 0)
		    {
			    __result = true; // we can survive everywhere
			    return false;
		    }
		    return true; // do the vanilla thang
	    }
    }    
    
    //This patch controls the dyin from pollution
    [HarmonyPatch(typeof(Plant), nameof(Plant.DyingFromPollution), MethodType.Getter)]
    public class Patch_DyingFromPollution
    {
	    [HarmonyPrefix]
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
    /// <summary>
    /// Pollution pump plant !!!
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.TickLong))]
    public class TickLong
    {
	    [HarmonyPostfix]
	    public static void Postfix(Plant __instance)
	    {
		    if (__instance.getPollutionResistance() > 1)
		    {
			    int num = GenRadial.NumCellsInRadius(1f);
			    Map map = __instance.Map;
			    if (map == null) return;
			    IntVec3 unpolluteCell = IntVec3.Invalid;
			    for (int i = 0; i < num; i++)
			    {
				    IntVec3 intVec = __instance.Position + GenRadial.RadialPattern[i];
				    if (intVec.InBounds(map) && intVec.CanUnpollute(map))
				    {
					    unpolluteCell = intVec;
				    }
			    }

			    if (unpolluteCell != IntVec3.Invalid)
			    {
				    // pump pollution
				    map.pollutionGrid.SetPolluted(unpolluteCell, isPolluted: false);
			    }
			    
		    }
	    }
    }*/
}