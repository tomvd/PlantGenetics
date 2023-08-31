using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PlantGenetics.Clipping;

public class WorkGiver_PlantsClip : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        foreach (Designation item in pawn.Map.designationManager.designationsByDef[InternalDefOf.ClipPlant])
        {
            yield return item.target.Thing;
        }
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !pawn.Map.designationManager.AnySpawnedDesignationOfDef(InternalDefOf.ClipPlant);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.def.category != ThingCategory.Plant)
        {
            return null;
        }
        if (!pawn.CanReserve(t, 1, -1, null, forced))
        {
            return null;
        }
        if (t.IsForbidden(pawn))
        {
            return null;
        }
        if (t.IsBurning())
        {
            return null;
        }
        if (!PlantUtility.PawnWillingToCutPlant_Job(t, pawn))
        {
            return null;
        }
        foreach (Designation item in pawn.Map.designationManager.AllDesignationsOn(t))
        {
            if (item.def == InternalDefOf.ClipPlant)
            {
                return JobMaker.MakeJob(InternalDefOf.ClipPlantDesignated, t);
            }
        }
        return null;
    }
/*
    public override string PostProcessedGerund(Job job)
    {
        if (job.def == InternalDefOf.ClipPlantDesignated)
        {
            return "ClipGerund".Translate();
        }
        return def.gerund;
    }*/
}