using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics;

public class Designator_ClonePlant : Designator
{
    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Plants;

    public Designator_ClonePlant()
    {
        defaultLabel = "DesignatorClonePlant".Translate();
        defaultDesc = "DesignatorClonePlantDesc".Translate();
        icon = ContentFinder<Texture2D>.Get("clonePlant");
        useMouseIcon = true;
        soundDragSustain = SoundDefOf.Designate_DragStandard;
        soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
        soundSucceeded = SoundDefOf.Designate_ExtractTree;
        hotKey = KeyBindingDefOf.Misc12;
        //tutorTag = "ExtractTree";
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
        if (!c.InBounds(base.Map))
        {
            return false;
        }
        foreach (Thing thing in c.GetThingList(base.Map))
        {
            if (CanDesignateThing(thing).Accepted)
            {
                return true;
            }
        }
        return false;
    }

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
        if (t is Plant plant 
                             && plant.HasTrait()
                             && Map.listerBuildings.allBuildingsColonist
                                 .Any(building => building.def == InternalDefOf.PottingBench)
                             && Map.designationManager.DesignationOn(plant, InternalDefOf.ClonePlant) == null 
                             && plant.growthInt is >= 0.5f)
        {
            return true;
        }
        return false;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        foreach (Thing thing in c.GetThingList(base.Map))
        {
            if (CanDesignateThing(thing).Accepted)
            {
                DesignateThing(thing);
            }
        }
    }

    public override void DesignateThing(Thing t)
    {
        base.Map.designationManager.AddDesignation(new Designation((Plant)t, InternalDefOf.ClonePlant));
        Designation designation = base.Map.designationManager.DesignationOn(t, DesignationDefOf.CutPlant);
        if (designation != null)
        {
            base.Map.designationManager.RemoveDesignation(designation);
        }
        designation = base.Map.designationManager.DesignationOn(t, DesignationDefOf.HarvestPlant);
        if (designation != null)
        {
            base.Map.designationManager.RemoveDesignation(designation);
        }
    }
}