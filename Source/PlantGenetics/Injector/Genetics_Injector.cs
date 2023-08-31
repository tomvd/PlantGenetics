using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PlantGenetics.Clipping;
using PlantGenetics.Comp;
using RimWorld;
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
                Log.Message(def.defName + ": added genetics");
            }
        }
        
        //Type bed = typeof(Building_Bed);
        var seedDefs = DefDatabase<ThingDef>.AllDefsListForReading
            .Where(def => def.thingCategories != null && def.thingCategories.Contains(DefDatabase<ThingCategoryDef>.GetNamed("SeedsCategory"))).ToArray();

        CreateClippingDefs(seedDefs);
    }
        private static void CreateClippingDefs(ThingDef[] seedDefs)
        {
            var sb = new StringBuilder("Created clippings for the following seeds: ");
            var fields = typeof(ThingDef).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var seedDef in seedDefs)
            {
                /*if (seedDef.comps == null || !seedDef.comps.Any(c => typeof(CompAssignableToPawn_Bed).IsAssignableFrom(c.compClass)))
                {
                    string mod = seedDef.modContentPack != null ? seedDef.modContentPack.Name : "an unknown mod";
                    Log.Warning($"Not creating guest beds for {seedDef.label} from {mod}. It does not have a CompAssignableToPawn_Bed.");
                    continue;
                }*/
                var clippingDef = new ThingDef();
                
                // Copy fields
                foreach (var field in fields)
                {
                    if (field.Name == "designationCategory") continue;
                    field.SetValue(clippingDef, field.GetValue(seedDef));
                }

                CopyComps(clippingDef, seedDef);

                // Other properties
                clippingDef.defName += "Clipping";
                clippingDef.label = "GuestBedFormat".Translate(clippingDef.label);
                clippingDef.thingClass = typeof(ClippingDef);
                clippingDef.shortHash = 0;
                clippingDef.minifiedDef = seedDef.minifiedDef;
                clippingDef.tradeability = Tradeability.None;
                clippingDef.scatterableOnMapGen = false;
                clippingDef.tickerType = TickerType.Long;
                //guestBedDef.modContentPack = GuestUtility.relaxDef.modContentPack;

                HashSet<ushort> takenHashes = ShortHashGiver.takenHashesPerDeftype[typeof(ThingDef)];
                typeof(ShortHashGiver).GetMethod("GiveShortHash", BindingFlags.NonPublic|BindingFlags.Static).Invoke(null, new object[] {clippingDef, typeof(ThingDef), takenHashes});

                DefDatabase<ThingDef>.Add(clippingDef);
                sb.Append(seedDef.defName + ", ");
            }
            //Log.Message(sb.ToString().TrimEnd(' ', ','));
        }

        private static void CopyComps(ThingDef guestBedDef, ThingDef bedDef)
        {
            guestBedDef.comps = new List<CompProperties>();
            for (int i = 0; i < bedDef.comps.Count; i++)
            {
                var constructor = bedDef.comps[i].GetType().GetConstructor(Type.EmptyTypes);
                var comp = (CompProperties) constructor.Invoke(null);

                var fieldsComp = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fieldsComp)
                {
                    field.SetValue(comp, field.GetValue(bedDef.comps[i]));
                    //Log.Message($"Set {field.Name} of {comp.compClass.Name} of {guestBedDef.defName} to {field.GetValue(bedDef.comps[i])}");
                }
                guestBedDef.comps.Add(comp);
            }
        }
    
}
/*
    private static void InjectTab(Type tabType, Func<ThingDef, bool> qualifier)
    {
        var defs = DefDatabase<ThingDef>.AllDefs.Where(qualifier).ToList();
        defs.RemoveDuplicates();

        var tabBase = InspectTabManager.GetSharedInstance(tabType);

        foreach (var def in defs)
        {
            if (def.inspectorTabs == null || def.inspectorTabsResolved == null) continue;

            if (!def.inspectorTabs.Contains(tabType))
            {
                def.inspectorTabs.Add(tabType);
                def.inspectorTabsResolved.Add(tabBase);
                //Log.Message(def.defName+": "+def.inspectorTabsResolved.Select(d=>d.GetType().Name).Aggregate((a,b)=>a+", "+b));
            }
        }
    }
*/

