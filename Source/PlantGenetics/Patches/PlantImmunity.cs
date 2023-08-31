using System.Collections.Generic;
using HarmonyLib;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantImmunity
{
    /// <summary>
    /// Blight immunity
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.BlightableNow), MethodType.Getter)]
    public class BlightableNow
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Plant __instance)
        {
            if (__instance.GetComp<CompPlantGenetics>().getImmunityModifier() > 0f)
            {
                __result = false;
            }
        }
    }
    
    /// <summary>
    /// Toxic fallout immunity 
    /// </summary>
    [HarmonyPatch(typeof(GameCondition_ToxicFallout), nameof(GameCondition_ToxicFallout.DoCellSteadyEffects))]
    public class DoCellSteadyEffects
    {
        [HarmonyPrefix]
        public static bool Prefix(IntVec3 c, Map map)
        {
            if (c.Roofed(map))
            {
                return true; // continue
            }            
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing is Plant && ((ThingWithComps)thing).GetComp<CompPlantGenetics>().getImmunityModifier() > 1f)
                {
                    return false; // do not affect this cell with toxic fallout
                }
            }
            return true; // continue
        }
    }
}