using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using PlantGenetics.Gens;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics;

public static class BreedHelper
{
    public static readonly Dictionary<string, string[]> SowTagsResolverDictionary = [];
    public static string GetNameSuggestionFromCloneDataV1(CloneData cloneData)
    {
        return cloneData.Trait.LabelCap + " " + DefDatabase<ThingDef>.GetNamed(cloneData.PlantDef, true).LabelCap;
    }

    public delegate string GetNameSuggestionFromCloneDataDelegate(CloneData cloneData, List<CloneData> allClones);
    public static GetNameSuggestionFromCloneDataDelegate GetNameSuggestionFromCloneDataGeneral = GetNameSuggestionFromCloneDataV3;
    public static string GetNameSuggestionFromCloneDataV2(CloneData cloneData, List<CloneData> allClones)
    {
        Dictionary<string, CloneData> clonesDict = [];
        foreach (CloneData data in allClones)
        {
            if (data.defName != null)
                clonesDict[data.defName] = data;
        }

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
            .Select(x => $"{FirstCharacters(x.Key, 4)}-{x.Value}");
        string traitsWithCountString = string.Join(" ", traitsWithCount);
        int traitSum = traits.Sum(x => x.Value);
        return $"{thingDef.LabelCap}-GEN{traitSum:000} {traitsWithCountString}";
    }

    private static string FirstCharacters(string str, int count)
    {
        if (str.Length <= count) return str;
        return str.Substring(0, count);
    }
    
    public static string GetNameSuggestionFromCloneDataV3(CloneData cloneData, List<CloneData> allClones)
    {
        Dictionary<string, CloneData> clonesDict = [];
        foreach (CloneData data in allClones)
        {
            if (data.defName != null)
                clonesDict[data.defName] = data;
        }

        Dictionary<string, int> traits = [];
        ThingDef thingDef = null;
        CloneData current = cloneData;
        while (current != null)
        {
            if (!traits.TryAdd(current.Trait.LabelCap, 1))
            {
                traits[current.Trait.LabelCap] += 1;
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
            .Select(x => (Key: FirstCharacters(x.Key, 4), x.Value))
            .Select(x => x.Value == 1 ? x.Key : $"{x.Key}{x.Value}");
        string traitsWithCountString = string.Join(" ", traitsWithCount);
        int traitSum = traits.Sum(x => x.Value);
        return $"{thingDef.LabelCap} G{traitSum:0} {traitsWithCountString}";
    }

    private static ThingDef CreatePlantThingDefFromTemplate(ThingDef template)
    {
        ThingDef thing = new ThingDef();
        // Copy fields
        foreach (FieldInfo fieldInfo in typeof(ThingDef).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            fieldInfo.SetValue(thing, fieldInfo.GetValue(template));
        }
        CopyComps(thing, template);
        thing.plant = new PlantProperties();
        foreach (FieldInfo fieldInfo2 in typeof(PlantProperties).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            fieldInfo2.SetValue(thing.plant, fieldInfo2.GetValue(template.plant));
        }
        thing.plant.sowTags = new List<string>(template.plant.sowTags);
        return thing;
    }

    private static void ApplyTrait(CloneData cloneData, ThingDef clone)
    {
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
                case "glow":
                    var glower = new CompProperties_Glower();
                    glower.glowRadius = cloneData.Trait.statmultiplier;
                    glower.glowColor = new ColorInt(10, 50, 64, 0);
                    clone.comps.Add(glower);
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
    }
    public static ThingDef AddBreedFromClone(CloneData cloneData)
    {

        var clone = CreateThingDefFromCloneData(cloneData, out var alreadyExist);
        cloneData.defName = clone.defName;

        if (!alreadyExist)
        {
            HashSet<ushort> takenHashes = ShortHashGiver.takenHashesPerDeftype[typeof(ThingDef)];
            ShortHashGiver.GiveShortHash(clone, typeof(ThingDef), takenHashes);

            DefDatabase<ThingDef>.Add(clone);
            clone.ResolveReferences();
        }

        return clone;
    }

    public static ThingDef CreateThingDefFromCloneData(CloneData cloneData)
        => CreateThingDefFromCloneData(cloneData, out _);
    public static ThingDef CreateThingDefFromCloneData(CloneData cloneData, out bool isAlreadyExist)
    {
        var defName = cloneData.Trait.defName + "_" + cloneData.PlantDef;
        ThingDef clone = DefDatabase<ThingDef>.GetNamed(defName, false);
        if (clone != null)
        {
            Log.Message("already exists in defdatabase: " + defName);
            isAlreadyExist = true;
            return clone;
        }
        isAlreadyExist = false;

        ThingDef template = DefDatabase<ThingDef>.GetNamed(cloneData.PlantDef);
        clone = CreatePlantThingDefFromTemplate(template);

        // Other properties
        clone.defName = defName;
        clone.label = cloneData.newName;
        clone.shortHash = 0;

        // modify Trait properties
        if (cloneData.Trait.associatedStats != null && cloneData.Trait.associatedStats.Count > 0)
        {
            clone.statBases = new List<StatModifier>(); // otherwise it seems to overwrite stats of the template?
            foreach (var templateStatBase in template.statBases)
            {
                StatModifier statModifier = new()
                {
                    stat = templateStatBase.stat,
                    value = templateStatBase.value
                };
                clone.statBases.Add(statModifier);
            }
            //Log.Message("checking mutation stats", );
            foreach (var statMod in clone.statBases)
            {
                if (cloneData.Trait.associatedStats.Contains(statMod.stat))
                {
                    //Log.Message("changing stat " + statMod);
                    statMod.value *= cloneData.Trait.statmultiplier;
                    //Log.Message("changed stat " + statMod);
                }
            }
        }

        ApplyTrait(cloneData, clone);

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
