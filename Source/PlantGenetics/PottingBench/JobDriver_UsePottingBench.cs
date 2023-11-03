using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace PlantGenetics
{
    public class JobDriver_UsePottingBench : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);//.FailOn(to => !((Building_PottingBench)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseComputerNow);
            var openComputer = ToilMaker.MakeToil();
            openComputer.initAction = () =>
            {
                var actor = openComputer.actor;
                //if (!((Building_PottingBench)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseComputerNow)
//                    return;
                Find.WindowStack.Add(new Window_PottingBench(actor));
                //actor.jobs.curJob.commTarget.TryOpenComms(actor);
            };
            yield return openComputer;
        }
    }
}