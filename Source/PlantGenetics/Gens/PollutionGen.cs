using System.Linq;
using PlantGenetics.Utilities;
using RimWorld;

namespace PlantGenetics.Gens;

public static class PollutionGen
{
    public static float getPollutionResistance(this Plant plant)
    {
        int c = plant.getDNA().Count(f => (f == 'P'));
        if (c == 2) return 1f;
        if (c > 2) return 2f;
        return 0f;
    }
}