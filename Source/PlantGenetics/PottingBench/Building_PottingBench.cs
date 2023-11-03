using RimWorld;
using Verse;
using Verse.AI;

namespace PlantGenetics
{
    public class Building_PottingBench : Building
    {

        private void UseAct(Pawn myPawn, ICommunicable commTarget)
        {
            var job = JobMaker.MakeJob(InternalDefOf.UsePottingBench, (LocalTargetInfo)(Thing)this);
            job.commTarget = commTarget;
            myPawn.jobs.TryTakeOrderedJob(job);
        }

        private FloatMenuOption GetFailureReason(Pawn myPawn)
        {
            if (!myPawn.CanReach((LocalTargetInfo)(Thing)this, PathEndMode.InteractionCell, Danger.Some))
                return new FloatMenuOption("CannotUseNoPath".Translate(), null);
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                return new FloatMenuOption(
                    "CannotUseReason".Translate(
                        "IncapableOfCapacity".Translate((NamedArgument)PawnCapacityDefOf.Manipulation.label,
                            myPawn.Named("PAWN"))), null);
            Log.Error(myPawn + " could not use potting bench for unknown reason.");
            return new FloatMenuOption("Cannot use now", null);
        }
    }
}