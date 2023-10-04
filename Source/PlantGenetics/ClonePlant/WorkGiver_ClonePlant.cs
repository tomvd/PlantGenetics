using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PlantGenetics;

public class WorkGiver_ClonePlant : WorkGiver_RemoveBuilding
{
    public override DesignationDef Designation => InternalDefOf.ClonePlant;

    public override JobDef RemoveBuildingJob => InternalDefOf.ClonePlantJob;
}