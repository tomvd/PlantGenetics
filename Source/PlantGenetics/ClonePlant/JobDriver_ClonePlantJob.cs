using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace PlantGenetics;

public class JobDriver_ClonePlantJob : JobDriver
{
	private float workLeft;

	private float totalNeededWork;

	public const TargetIndex TreeInd = TargetIndex.A;

	protected Thing Target => job.GetTarget(TargetIndex.A).Thing;

	protected Plant Tree => (Plant)Target.GetInnerIfMinified();

	protected DesignationDef Designation => InternalDefOf.ClonePlant;

	protected float TotalNeededWork => Tree.def.plant.harvestWork;

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref workLeft, "workLeft", 0f);
		Scribe_Values.Look(ref totalNeededWork, "totalNeededWork", 0f);
	}

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
	}

	public override IEnumerable<Toil> MakeNewToils()
	{
		this.FailOnThingMissingDesignation(TargetIndex.A, Designation);
		this.FailOnForbidden(TargetIndex.A);
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
		Toil doWork = ToilMaker.MakeToil("MakeNewToils").FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
		doWork.initAction = delegate
		{
			totalNeededWork = TotalNeededWork;
			workLeft = totalNeededWork;
		};
		doWork.tickAction = delegate
		{
			workLeft -= JobDriver_PlantWork.WorkDonePerTick(pawn, Tree);
			if (pawn.skills != null)
			{
				pawn.skills.Learn(SkillDefOf.Plants, 0.085f);
			}
			if (workLeft <= 0f)
			{
				SoundDefOf.Finish_Wood.PlayOneShot(SoundInfo.InMap(Tree));
				doWork.actor.jobs.curDriver.ReadyForNextToil();
			}
		};
		doWork.defaultCompleteMode = ToilCompleteMode.Never;
		doWork.WithProgressBar(TargetIndex.A, () => 1f - workLeft / totalNeededWork);
		doWork.WithEffect(EffecterDefOf.Harvest_Plant, TargetIndex.A);
		doWork.PlaySustainerOrSound(() => SoundDefOf.Designate_CutPlants);
		doWork.activeSkill = () => SkillDefOf.Plants;
		yield return doWork;
		Toil toil = ToilMaker.MakeToil("MakeNewToils");
		toil.initAction = delegate
		{
			SpawnMinifiedClones(Tree, pawn);
			Map.designationManager.RemoveAllDesignationsOn(Target);
			Tree.Destroy(DestroyMode.Vanish);
		};
		toil.defaultCompleteMode = ToilCompleteMode.Instant;
		yield return toil;
	}

	private void SpawnMinifiedClones(Plant plant, Pawn pawn)
	{
		/*
		for (int i = 0; i < Rand.Range(3,6); i++)
		{
			Plant clone = ThingMaker.MakeThing(plant.def) as Plant;
			clone.setDNA(plant.getDNA());
			clone.growthInt = 0.01f;
			GenPlace.TryPlaceThing(clone.MakeMinified(), pawn.Position, base.Map, ThingPlaceMode.Near);
		}*/
		Map.GetComponent<PottingService>().AddClone(plant);
	}
}
