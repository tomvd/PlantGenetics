using System;
using System.Collections.Generic;
using System.Linq;
using PlantGenetics.Gens;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Comp
{
    public class CompPlantGenetics : ThingComp
    {
        private String DNA;
        public String getDNA() => DNA;
        
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref DNA, "DNA");
        }

        public override void PostPostMake()
        {
            DNA = DNAUtility.AddWildDNA(parent);
            // it can also come from the clone
        }

        public void SetDNA(String DNA)
        {
            this.DNA = DNA;
        }
        
        public override string CompInspectStringExtra()
        {
            return "DNA:" + DNA;
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            if (parent is Plant plant) {
                yield return new StatDrawEntry(StatCategoryDefOf.Basics, "LightRequirement".Translate(),
                (plant.def.plant.growMinGlow * plant.getLightSensitivityModifier()).ToStringPercent(),
                "Stat_Thing_Plant_LightRequirement_Desc".Translate(), 5000);
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
            debug.defaultLabel = "debug - change DNA";
            debug.defaultDesc = "debug - change DNA";
            debug.action = delegate
            {
                SetDNA(DNAUtility.AddWildDNA(parent));
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
