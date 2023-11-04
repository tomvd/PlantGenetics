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
            return DefDatabase<TraitDef>.AllDefsListForReading.Where(traitDef => traitDef.commonality > 0.0f).RandomElement(); // TODO take into account commonality
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