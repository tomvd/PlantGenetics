using Verse;

namespace PlantGenetics.Comp
{
    public class CompPlantGenetics : ThingComp
    {
        public override string CompInspectStringExtra()
        {
            return "Tier 1 genetics";
        }
    }

    public class CompProperties_PlantGenetics : CompProperties
    {
        public CompProperties_PlantGenetics()
        {
            compClass = typeof(CompPlantGenetics);
        }
    }
}
