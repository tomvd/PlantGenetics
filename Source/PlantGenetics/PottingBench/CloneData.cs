using RimWorld;
using Verse;

namespace PlantGenetics;

public class CloneData : IExposable
{
    public CloneData(Plant plant)
    {
        PlantDef = plant.def.defName;
        Trait = plant.TryGetComp<CompPlantGenetics>().Trait;
    }

    public CloneData(string plantDef, TraitDef trait, string status, float finishDays, string newName, string defName)
    {
        PlantDef = plantDef;
        Trait = trait;
        this.status = status;
        this.finishDays = finishDays;
        this.newName = newName;
        this.defName = defName;
    }

    public CloneData()
    {
    }
    
    public string PlantDef;
    public TraitDef Trait;
    public string status;
    public float finishDays;
    public string newName;
    public string defName;

    public void ExposeData()
    {
        Scribe_Values.Look(ref PlantDef, "PlantDef");
        Scribe_Defs.Look(ref Trait, "Trait");
        Scribe_Values.Look(ref status, "status");
        Scribe_Values.Look(ref finishDays, "finishDays");
        Scribe_Values.Look(ref newName, "newName");
        Scribe_Values.Look(ref defName, "defName");
    }
}