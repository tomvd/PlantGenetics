using System.Linq;
using PlantGenetics.Utilities;
using RimWorld;

namespace PlantGenetics.Gens;

public static class ChemicalsGen
{
    public static float getChemGen(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'C'));
        if (c == 2) return 1f; // chemfuel
        if (c > 2) return 2f; // neutroamine
        return 0f;
    }
    
    // TODO
}