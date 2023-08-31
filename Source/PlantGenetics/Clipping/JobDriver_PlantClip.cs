using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PlantGenetics.Clipping;

public class JobDriver_PlantClip : JobDriver_PlantWork
{
    public override PlantDestructionMode PlantDestructionMode => PlantDestructionMode.Cut;

    public override void Init()
    {
        if (base.Plant?.def.plant.harvestedThingDef != null && base.Plant.CanYieldNow())
        {
            xpPerTick = 0.085f;
        }
        else
        {
            xpPerTick = 0f;
        }
    }

    public override Toil PlantWorkDoneToil()
    {
        Toil toil = ToilMaker.MakeToil("MakeNewToils");
        toil.initAction = delegate
        {
            IntVec3 position = Plant.Position;
            bool num = Find.Selector.IsSelected(Plant);
            Thing thing = GenSpawn.Spawn(Plant.MakeMinified(), position, pawn.Map);
            if (num && thing != null)
            {
                Find.Selector.Select(thing, playSound: false, forceDesignatorDeselect: false);
            }
            base.Map.designationManager.RemoveAllDesignationsOn(Plant);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        return toil;        
    }
}