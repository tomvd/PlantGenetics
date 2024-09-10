using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PlantGenetics;

public static class CloneStatus
{
    public const string Breeding = "breeding";
    public const string Done = "done";
    public const string Removed = "removed";
}

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

    /// <summary>
    /// DefName of parent plant
    /// </summary>
    public string PlantDef;
    public TraitDef Trait;
    /// <summary>
    /// null or <see cref="CloneStatus"/>
    /// </summary>
    public string status;
    /// <summary>
    /// When the clone will be or was made
    /// </summary>
    public float finishDays;
    /// <summary>
    /// Clone name for game
    /// </summary>
    public string newName;
    /// <summary>
    /// Unique DefName for clone
    /// </summary>
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