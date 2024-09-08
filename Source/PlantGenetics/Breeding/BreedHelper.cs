using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using PlantGenetics.Gens;
using RimWorld;
using Verse;

namespace PlantGenetics;

public static class BreedHelper
{
    public static string GetNameSuggestionFromCloneDataV1(CloneData cloneData)
    {
        return cloneData.Trait.LabelCap + " " + DefDatabase<ThingDef>.GetNamed(cloneData.PlantDef, true).LabelCap;
    }
    public static string GetNameSuggestionFromCloneDataV2(CloneData cloneData, List<CloneData> allClones)
    {
        Dictionary<string, CloneData> clonesDict = allClones
            .Where(x => x.defName != null)
            .ToDictionary(x => x.defName, x => x);

        Dictionary<string, int> traits = [];
        ThingDef thingDef = null;
        CloneData current = cloneData;
        while (current != null)
        {
            if (!traits.TryAdd(current.Trait.LabelCap, 1))
            {
                traits[current.Trait.LabelCap]+=1;
            }
            string plantDef = current.PlantDef;
            if (!clonesDict.TryGetValue(plantDef, out current))
            {
                thingDef = DefDatabase<ThingDef>.GetNamed(plantDef, false);
                if (thingDef is null)
                {
                    return GetNameSuggestionFromCloneDataV1(cloneData);
                }
            }
        }
        var traitsWithCount = traits
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key.Substring(0, 4)}-{x.Value}");
        string traitsWithCountString = string.Join(" ", traitsWithCount);
        return $"{thingDef.LabelCap}-GEN{traits.Count:000} {traitsWithCountString}";
    }

    public static ThingDef AddBreedFromClone(CloneData cloneData)
    {
        ThingDef template = DefDatabase<ThingDef>.GetNamed(cloneData.PlantDef);
        string cloneDefName = cloneData.Trait.defName +"_"+ template.defName;
        cloneData.defName = cloneDefName;
        if (DefDatabase<ThingDef>.GetNamed(cloneDefName, false) != null)
        {
            Log.Message("already exists in defdatabase: " + cloneDefName);
            return DefDatabase<ThingDef>.GetNamed(cloneDefName, false);
        }
        string cloneName = cloneData.newName;
        ThingDef clone = new ThingDef();
        var fields = typeof(ThingDef).GetFields(BindingFlags.Public | BindingFlags.Instance);
                
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
        
        // modify Trait properties
        if (cloneData.Trait.associatedStats != null && cloneData.Trait.associatedStats.Count > 0)
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

        if (cloneData.Trait.associatedPlantProperty != null)
        {
            switch (cloneData.Trait.associatedPlantProperty)
            {
                case "harvestYield":
                    clone.plant.harvestYield *= cloneData.Trait.statmultiplier;
                    break;
                case "growDays":
                    clone.plant.growDays *= cloneData.Trait.statmultiplier;
                    break;
                case "fertilityMin":
                    clone.plant.fertilityMin *= cloneData.Trait.statmultiplier;
                    clone.plant.fertilitySensitivity *= cloneData.Trait.statmultiplier;
                    break;
                case "growMinGlow":
                    clone.plant.growMinGlow *= cloneData.Trait.statmultiplier;
                    clone.plant.dieIfNoSunlight = false;
                    clone.plant.neverBlightable = true;
                    clone.plant.dieFromToxicFallout = false;
                    break;
            }
        }

        if (cloneData.Trait.special)
        {
            TraitExtension te = new TraitExtension();
            te.SpecialTrait = cloneData.Trait;
            clone.modExtensions ??= new List<DefModExtension>();
            clone.modExtensions.Add(te);
            /*if (cloneData.Trait.Equals(InternalDefOf.Summer) && !clone.plant.sowTags.Contains("VCE_Sandy"))
            {
                clone.plant.sowTags.Add("VCE_Sandy");
            }*/
        }

        HashSet<ushort> takenHashes = ShortHashGiver.takenHashesPerDeftype[typeof(ThingDef)];
        ShortHashGiver.GiveShortHash(clone, typeof(ThingDef), takenHashes);

        DefDatabase<ThingDef>.Add(clone);
        clone.ResolveReferences();
        return clone;
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