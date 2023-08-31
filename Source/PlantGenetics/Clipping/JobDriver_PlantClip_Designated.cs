using Verse;

namespace PlantGenetics.Clipping;

public class JobDriver_PlantClip_Designated : JobDriver_PlantClip
{
    public override DesignationDef RequiredDesignation => InternalDefOf.ClipPlant;    
}