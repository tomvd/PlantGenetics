using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PlantGenetics.Gens;

public static class ImmunityGen
{/*
    public static float getImmunityModifier(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'I'));
        return c switch
        { 
            1 => 1f,
            > 1 => 2f,
            _ => 0f
        };
    }
    
    /// <summary>
    /// Blight immunity (1 I-gen)
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.BlightableNow), MethodType.Getter)]
    public class BlightableNow
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Plant __instance)
        {
            if (__instance.getImmunityModifier() > 0f)
            {
                __result = false;
            }
        }
    }
    
    /// <summary>
    /// Toxic fallout immunity  (2 I-gens)
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
                if (thing is Plant plant && plant.getImmunityModifier() > 1f)
                {
                    return false; // do not affect this cell with toxic fallout
                }
            }
            return true; // continue
        }
    }*/
}