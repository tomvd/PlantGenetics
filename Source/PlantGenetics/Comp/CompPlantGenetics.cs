using System;
using System.Linq;
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

        public float getGrowDaysModifier()
        {
            int c = DNA.Count(f => (f == 'G'));
            if (c == 2) return 0.8f;
            if (c > 2) return 0.5f;
            return 1.0f;
        }
        
        public float getYieldModifier()
        {
            int c = DNA.Count(f => (f == 'Y'));
            if (c == 2) return 1.2f;
            if (c > 2) return 2f;
            return 1.0f;
        }
        
        public float getFertilitySensitivityModifier()
        {
            int c = DNA.Count(f => (f == 'F'));
            if (c == 2) return 0.5f;
            if (c > 2) return 0f;
            return 1.0f;
        }


        public override string CompInspectStringExtra()
        {
            return "Tier 1 genetics";
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
