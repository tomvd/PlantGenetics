using System;
using System.Collections.Generic;
using System.Linq;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Utilities;

public static class DNAUtility
{
    public static string getDNA(this Plant plant)
    {
        return plant.GetComp<CompPlantGenetics>().getDNA();
    }
    
    public static void setDNA(this Plant plant, String DNA)
    {
        plant.GetComp<CompPlantGenetics>().SetDNA(DNA);
    }

    /*
     * every wild plant has FGHY or FGHB and two unused gens (X)
     * 
     */
    public static string AddWildDNA(Thing plantOrSeed)
    {
        String Genes = "FGHYBCLIPT";
        String DNA = "FGHYXX";
        ThingDef plantDef = (plantOrSeed.def.plant != null ? plantOrSeed.def : null) ?? DefDatabase<ThingDef>.GetNamed(plantOrSeed.def.descriptionHyperlinks[0].def.defName);

        // depending on the plants purpose some genes are not present in wild plants
        switch (plantDef.plant.purpose)
        {
            case PlantPurpose.Beauty:
                DNA = "FGHBXX";
                break;
        }

        // now add random mutation (TODO lower this chance!!!)
        char[] chars = DNA.ToCharArray();
        for (int i = 0; i < 6; i++)
        {
            if (Rand.Chance(0.1f))
                chars[i] = Genes.ToCharArray().RandomElement();
        }
        DNA = new string(chars);
        return DNA;
    }

    public static void CrossBreed(Plant sourcePlant)
    {
        // gather genetics around the plant
        List<String> genetics = new List<string>();
        char[] sourceDNA = sourcePlant.getDNA().ToCharArray();
        // TODO check if we need a larger radius for trees (they can not be planted adjacent to each other)
        // TODO can be outside map bounds??
        IEnumerable<Thing> things = GenRadial.RadialDistinctThingsAround(sourcePlant.Position, sourcePlant.Map, 4f, false);
        foreach (var thing in things)
        {
            if (thing is Plant plant)
            {
                genetics.Add(plant.GetComp<CompPlantGenetics>().getDNA());
            }
        }
        // there is a 1% chance of adding another gen slot !
        if (Rand.Chance(0.01f))
        {
            sourcePlant.GetComp<CompPlantGenetics>().SetDNA(sourcePlant.getDNA()+"X");
        }
        // go over the slots - every surrounding plant has 10% chance of transferring its gen into the source slot
        for (int i = 0; i < sourceDNA.Length; i++)
        {
            foreach (var gen in genetics)
            {
                if (gen.Length > i && Rand.Chance(0.10f))
                {
                    sourceDNA[i] = gen[i];
                }
            }
        }
        sourcePlant.GetComp<CompPlantGenetics>().SetDNA(new string(sourceDNA));
    }
}