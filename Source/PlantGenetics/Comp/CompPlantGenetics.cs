using System.Collections.Generic;
using PlantGenetics.Gens;
using RimWorld;
using Verse;

namespace PlantGenetics
{
    public class CompPlantGenetics : ThingComp
    {
        public TraitDef Trait;
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref Trait, "Trait");
        }

        public override void PostPostMake()
        {
            Trait = Traits.GetRandomTrait();
        }

        public override string CompInspectStringExtra()
        {
            if (Trait == null) return "";
            return "Trait: " + Trait.LabelCap;
        }


        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (parent.def.GetModExtension<TraitExtension>() != null &&
                parent.def.GetModExtension<TraitExtension>().SpecialTrait != null &&
                parent.def.GetModExtension<TraitExtension>().SpecialTrait
                    .Equals(InternalDefOf.Winter))
            {
                yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "MinGrowthTemperature".Translate(), (-16f).ToStringTemperature(), "Stat_Thing_Plant_MinGrowthTemperature_Desc".Translate(), 4152);
                yield return new StatDrawEntry(StatCategoryDefOf.Genetics, "MaxGrowthTemperature".Translate(), 41f.ToStringTemperature(), "Stat_Thing_Plant_MaxGrowthTemperature_Desc".Translate(), 4153);
            }            
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            /*if (parent is Plant plant && plant.def.Minifiable
                                      && plant.getDNA().Length > 0
                                      && plant.Map.designationManager.DesignationOn(plant, InternalDefOf.ClonePlant) == null 
                                      && plant.growthInt >= 0.5f)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "Create clone";
                command_Action.defaultDesc = "Create clone";
                //command_Action.icon = CutAllBlightTex;
                command_Action.action = delegate
                {
                    parent.Map.designationManager.AddDesignation(new Designation(parent, InternalDefOf.ClonePlant));
                };
                yield return command_Action;
            }*/
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            Command_Action debug = new Command_Action();
            debug.defaultLabel = "debug - random trait";
            debug.defaultDesc = "debug - random trait";
            debug.action = delegate
            {
                Trait = Traits.GetRandomTrait(true);
            };
            yield return debug;
        }
    }

    public class CompProperties_PlantGenetics : CompProperties
    {
        public CompProperties_PlantGenetics()
        {
            compClass = typeof(CompPlantGenetics);
        }
    }
}
