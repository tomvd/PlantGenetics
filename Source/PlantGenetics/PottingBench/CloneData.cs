using RimWorld;
using Verse;

namespace PlantGenetics;

public class CloneData : IExposable
{
    public CloneData(Plant plant)
    {
        PlantDef = plant.def;
        Trait = plant.TryGetComp<CompPlantGenetics>().Trait;
    }

    public CloneData(ThingDef plantDef, TraitDef trait, string status, float finishDays, string newName)
    {
        PlantDef = plantDef;
        Trait = trait;
        this.status = status;
        this.finishDays = finishDays;
        this.newName = newName;
    }

    public CloneData()
    {
    }
    
    public ThingDef PlantDef;
    public TraitDef Trait;
    public string status;
    public float finishDays;
    public string newName;

    public void ExposeData()
    {
        Scribe_Defs.Look(ref PlantDef, "PlantDef");
        Scribe_Defs.Look(ref Trait, "Trait");
        Scribe_Values.Look(ref status, "status");
        Scribe_Values.Look(ref finishDays, "finishDays");
        Scribe_Values.Look(ref newName, "newName");
    }
}