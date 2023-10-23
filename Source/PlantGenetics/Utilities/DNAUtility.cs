using System;
using System.Collections.Generic;
using System.Linq;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Utilities;

public static class DNAUtility
{
    public static String getDNA(this Plant plant)
    {
        if (plant.TryGetComp<CompPlantGenetics>() == null) return "";
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

        // now add random mutation
        char[] chars = DNA.ToCharArray();
        for (int i = 0; i < 4; i++)
        {
            if (Rand.Chance(0.05f))
                chars[i] = Genes.ToCharArray().RandomElement();
        }
        for (int i = 4; i < 6; i++)
        {
            if (Rand.Chance(0.025f))
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
        // TODO can be outside map bounds??
        float pollinationRadius = 2f;
        if (sourcePlant.def.plant.IsTree)
        {
            pollinationRadius *= 2f;
        }
        if (sourcePlant.Map == null) return;
        IEnumerable<Thing> things = GenRadial.RadialDistinctThingsAround(sourcePlant.Position, sourcePlant.Map, pollinationRadius, false);
        foreach (var thing in things)
        {
            if (thing != sourcePlant && thing is Plant plant && plant.def == sourcePlant.def && plant.LifeStage == PlantLifeStage.Mature)
            {
                genetics.Add(plant.GetComp<CompPlantGenetics>().getDNA());
                //Log.Message("added genetics: " + plant.GetComp<CompPlantGenetics>().getDNA());
            }
        }
        // go over the slots - every surrounding plant has 50% chance of transferring a gen in a slot that still contains defaultDNA
        String DefaultDNA = "FGHYXX";
        // depending on the plants purpose some genes are not present in wild plants
        switch (sourcePlant.def.plant.purpose)
        {
            case PlantPurpose.Beauty:
                DefaultDNA = "FGHBXX";
                break;
        }        
        for (int i = 0; i < sourceDNA.Length; i++)
        {
            foreach (var gen in genetics)
            {
                if (Rand.Chance(0.50f) && sourceDNA[i] == DefaultDNA.ToCharArray()[i])
                {
                    sourceDNA[i] = gen[i];
                }
            }
        }
        sourcePlant.GetComp<CompPlantGenetics>().SetDNA(new string(sourceDNA));
    }
}