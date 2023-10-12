using System.Linq;
using HarmonyLib;
using PlantGenetics.Utilities;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.Gens;

public static class ChemicalsGen
{
    public static int getChemGen(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'C'));
        if (c == 1) return 1; // chemfuel
        if (c > 1) return 2; // neutroamine
        return 0;
    }
    
    //This patch controls the dropping of chemicals
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantCollected))]
    public class Patch_PlantCollected
    {
        public static void Prefix(Plant __instance, Pawn by)
        {
            int chem = __instance.getChemGen();

            if (chem > 0 &&
                __instance.Growth >= __instance.def.plant.harvestMinGrowth)
            {
                ThingDef chemDef = chem == 1 ? ThingDefOf.Chemfuel : DefDatabase<ThingDef>.GetNamed("Neutroamine");

                float stackCount = 1f;
                Thing newSeeds = ThingMaker.MakeThing(chemDef, null);
                newSeeds.stackCount = Mathf.RoundToInt(stackCount);

                GenPlace.TryPlaceThing(newSeeds, by.Position, by.Map, ThingPlaceMode.Near);

                if (!by.factionInt?.def.isPlayer ?? false) newSeeds.SetForbidden(true);
            }
        }
    }
}