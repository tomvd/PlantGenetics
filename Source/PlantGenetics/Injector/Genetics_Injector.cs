using System;
using System.Linq;
using PlantGenetics.Comp;
using Verse;

namespace PlantGenetics.Injector;

[StaticConstructorOnStartup]
public static class InjectDNA
{
    // all plants get a DNA comp to store their genetic info
    static InjectDNA()
    {
        var defs = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.plant != null).ToList();
        defs.RemoveDuplicates();
        Log.Message(defs.Count + " todo ");
        
        foreach (var def in defs)
        {
            if (def.comps == null) continue;

            if (!def.comps.Any(c => c.GetType() == typeof(CompProperties_PlantGenetics)))
            {
                CompProperties_PlantGenetics prop =
                    (CompProperties_PlantGenetics)Activator.CreateInstance(typeof(CompProperties_PlantGenetics));
                def.comps.Add(prop);
                //Log.Message(def.defName + ": added genetics");
            }
        }
    }
}


