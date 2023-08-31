using System;
using System.Collections.Generic;
using System.Linq;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Comp
{
    public class CompPlantGenetics : ThingComp
    {
        private String DNA;
        
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

        public void AlterDNA(String DNA)
        {
            this.DNA = DNA;
        }
        
        public float getFertilitySensitivityModifier()
        {
            int c = DNA.Count(f => (f == 'F'));
            if (c == 2) return 0.5f;
            if (c > 2) return 0f;
            return 1.0f;
        }
        
        public float getGrowthRateModifier()
        {
            int c = DNA.Count(f => (f == 'G'));
            if (c == 2) return 1.2f;
            if (c > 2) return 2f;
            return 1.0f;
        }
        
        public float getHardinessModifier()
        {
            int c = DNA.Count(f => (f == 'H'));
            if (c == 2) return 1f;
            if (c > 2) return 2f;
            return 0f;
        }    
        
        public float getBeautyModifier()
        {
            int c = DNA.Count(f => (f == 'B'));
            if (c == 2) return 1.2f;
            if (c > 2) return 2f;
            return 1.0f;
        }  
        
        public float getYieldModifier()
        {
            int c = DNA.Count(f => (f == 'Y'));
            if (c == 2) return 1.2f;
            if (c > 2) return 2f;
            return 1.0f;
        }
        
        public float getImmunityModifier()
        {
            int c = DNA.Count(f => (f == 'I'));
            if (c == 2) return 1f;
            if (c > 2) return 2f;
            return 0f;
        }
        
        public float getLightSensitivityModifier()
        {
            int c = DNA.Count(f => (f == 'L'));
            if (c == 2) return 0.5f;
            if (c > 2) return 0f;
            return 1.0f;
        }
        
        public float getPollutionResistance()
        {
            int c = DNA.Count(f => (f == 'P'));
            if (c == 2) return 1f;
            if (c > 2) return 2f;
            return 0f;
        }
        
        public float getChemGen()
        {
            int c = DNA.Count(f => (f == 'C'));
            if (c == 2) return 1f; // chemfuel
            if (c > 2) return 2f; // neutroamine
            return 0f;
        }
        
        public override string CompInspectStringExtra()
        {
            return "DNA:" + DNA;
        }
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
        {
            yield return new StatDrawEntry(StatCategoryDefOf.Basics, "LightRequirement".Translate(),
                (parent.def.plant.growMinGlow * getLightSensitivityModifier()).ToStringPercent(),
                "Stat_Thing_Plant_LightRequirement_Desc".Translate(), 5000);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent is Plant)
            {
                Designation designation = parent.Map.designationManager.DesignationOn(parent);
                if (designation == null || designation.def != InternalDefOf.ClipPlant)
                {
                    Command_Action command_Action = new Command_Action();
                    command_Action.defaultLabel = "Create clone";
                    command_Action.defaultDesc = "Create clone";
                    //command_Action.icon = CutAllBlightTex;
                    command_Action.action = delegate
                    {
                        parent.Map.designationManager.AddDesignation(new Designation(parent, InternalDefOf.ClipPlant));
                    };
                    yield return command_Action;
                }
            }
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            Command_Action debug = new Command_Action();
            debug.defaultLabel = "debug - change DNA";
            debug.defaultDesc = "debug - change DNA";
            debug.action = delegate
            {
                AlterDNA(DNAUtility.AddWildDNA(parent));
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
