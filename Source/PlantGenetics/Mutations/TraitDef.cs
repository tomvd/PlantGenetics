using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PlantGenetics;

public class TraitDef : Def
{
    public int level = 0; // level 1 mutations are only obtained by mutating a level 0 mutation or crossbreeding two the same level 0 mutations
    public float commonality; // percentage of this mutation to occur in the wild
    public float statmultiplier; // if value exists
    public List<StatDef> associatedStats = new List<StatDef>();
    public string associatedPlantProperty;
}