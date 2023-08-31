using System;
using System.Collections.Generic;
using System.Linq;
using PlantGenetics.Comp;
using RimWorld;
using Verse;

namespace PlantGenetics.Utilities;

public class DNAUtility
{
    /*
     * default wild plant DNA has 4 gens, mutations are rare 
     */
    public static string AddWildDNA(Thing plantOrSeed)
    {
        String Genes = "FGHYCLIP";
        String DNA = "XXXXXX";
        ThingDef plantDef = (plantOrSeed.def.plant != null ? plantOrSeed.def : null) ?? DefDatabase<ThingDef>.GetNamed(plantOrSeed.def.descriptionHyperlinks[0].def.defName);

        // depending on the plants purpose some genes are not present in wild plants
        switch (plantDef.plant.purpose)
        {
            case PlantPurpose.Beauty:
                Genes = "FGHBCXIP";
                break;
            case PlantPurpose.Food:
            case PlantPurpose.Health:
                Genes = "FGHYXXXX";
                break;
            case PlantPurpose.Misc:
                Genes = "FGHYXLIP";
                break;
        }

        char[] chars = DNA.ToCharArray();
        for (int i = 0; i < 6; i++)
        {
            chars[i] = Genes.ToCharArray().RandomElement();
        }
        DNA = new string(chars);
        return DNA;
    }

    public static void CrossBreed(Plant plant)
    {
        // look around the plant
        plant.GetComp<CompPlantGenetics>().AlterDNA(AddWildDNA(plant));
    }
}