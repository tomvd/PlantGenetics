using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PlantGenetics;

public static class BreedHelper
{
    public static bool AddBreedFromClone(CloneData cloneData)
    {
        var fields = typeof(ThingDef).GetFields(BindingFlags.Public | BindingFlags.Instance);
        ThingDef template = cloneData.PlantDef;
        string cloneDefName = template.defName + new UniqueId();
        string cloneName = cloneData.newName;
        ThingDef clone = new ThingDef();
                
        // Copy fields
        foreach (var field in fields)
        {
            //if (field.Name == "designationCategory") continue;
            field.SetValue(clone, field.GetValue(template));
        }

        CopyComps(clone, template);

        // Other properties
        clone.defName = cloneDefName;
        clone.label = cloneName;
        clone.shortHash = 0;
        if (cloneData.Trait.associatedStats.Count > 0)
        {
            clone.statBases = new List<StatModifier>(); // otherwise it seems to overwrite stats of the template?
            foreach (var templateStatBase in template.statBases)
            {
                StatModifier statModifier = new StatModifier();
                statModifier.stat = templateStatBase.stat;
                statModifier.value = templateStatBase.value;
                clone.statBases.Add(statModifier);
            }
            Log.Message("checking mutation stats");
            for (int i = 0; i < clone.statBases.Count; i++)
            {
                if (cloneData.Trait.associatedStats.Contains(clone.statBases[i].stat))
                {
                    Log.Message("changing stat " + clone.statBases[i]);
                    clone.statBases[i].value *= cloneData.Trait.statmultiplier;
                    Log.Message("changed stat " + clone.statBases[i]);
                }
            }
        }

        clone.plant = new PlantProperties();
        var fieldsComp = typeof(PlantProperties).GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fieldsComp)
        {
            field.SetValue(clone.plant, field.GetValue(template.plant));
        }

        switch (cloneData.Trait.associatedPlantProperty)
        {
            case "harvestYield":
                clone.plant.harvestYield *= cloneData.Trait.statmultiplier;                          
                break;
        }


        HashSet<ushort> takenHashes = ShortHashGiver.takenHashesPerDeftype[typeof(ThingDef)];
        typeof(ShortHashGiver).GetMethod("GiveShortHash", BindingFlags.NonPublic|BindingFlags.Static).Invoke(null, new object[] {clone, typeof(ThingDef), takenHashes});

        DefDatabase<ThingDef>.Add(clone);
        clone.ResolveReferences();
        Messages.Message("Succesfully created a new plant species: " + cloneName, MessageTypeDefOf.NeutralEvent);
        return true;
    }
    
    private static void CopyComps(ThingDef clone, ThingDef template)
    {
        clone.comps = new List<CompProperties>();
        for (int i = 0; i < template.comps.Count; i++)
        {
            var constructor = template.comps[i].GetType().GetConstructor(Type.EmptyTypes);
            var comp = (CompProperties) constructor.Invoke(null);

            var fieldsComp = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fieldsComp)
            {
                field.SetValue(comp, field.GetValue(template.comps[i]));
                //Log.Message($"Set {field.Name} of {comp.compClass.Name} of {guestBedDef.defName} to {field.GetValue(bedDef.comps[i])}");
            }
            clone.comps.Add(comp);
        }
    }    
}