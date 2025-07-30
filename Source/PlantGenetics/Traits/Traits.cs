using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PlantGenetics;

public static class Traits
{
    public static TraitDef GetRandomTrait(bool debug = false)
    {
        if (Rand.Chance(0.05f) || debug)
        {
            var traits = DefDatabase<TraitDef>.AllDefsListForReading
                .Where(t => t.commonality > 0f)
                .ToList();

            return traits
                .RandomElementByWeight(t => t.commonality);
        }
        else
        {
            return null;
        }
    }

    public static bool HasTrait(this Plant plant)
    {
        return plant.TryGetComp<CompPlantGenetics>().Trait != null;
    }
    
}
