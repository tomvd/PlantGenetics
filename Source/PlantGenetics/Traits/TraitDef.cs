using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PlantGenetics;

public class TraitDef : Def
{
    /// <summary>
    /// percentage of this mutation to occur in the wild
    /// </summary>
    public float commonality;
    public float statmultiplier; // if value exists
    public List<StatDef> associatedStats = new List<StatDef>();
    public string associatedPlantProperty;
    public bool special;
}