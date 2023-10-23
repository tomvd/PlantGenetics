using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PlantGenetics.Gens;

public static class ThornGen
{
    public static float getThornGen(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'T'));
        if (c == 1) return 1f; // thorns (slows down movement)
        if (c > 1) return 2f; // thorns (slows down movement even more)
        return 0f;
    }
    
    [HarmonyPatch(typeof(PathGrid), nameof(PathGrid.CalculatedCostAt))]
    public class CalculatedCostAt
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, PathGrid __instance,  IntVec3 c, bool perceivedStatic, IntVec3 prevCell)
        {
            List<Thing> list = __instance.map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (thing is Plant plant && plant.getDNA() != null)
                {
                    __result += Mathf.RoundToInt(plant.getThornGen() * 14);
                }
                
            }
        }
    }

}