using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace PlantGenetics
{
    /*
     * has an inventory of clippings/clones we have gathered
     * keeps track of breeding / crossbreeding / mutation jobs
     */
    public class PottingService : WorldComponent
    {

        private List<CloneData> _clones = new List<CloneData>();
        private Building pottingBench;

        public PottingService(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _clones, "clones", LookMode.Deep);
            Scribe_References.Look(ref pottingBench, "pottingBench");
            _clones ??= new List<CloneData>();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                InitClones();
            }
        }

        public List<CloneData> Clones => _clones;

        public bool AddClone(Plant plant, Building bench)
        {
            pottingBench = bench;
            Messages.Message("Successfully added a clipping to the potting bench.", MessageTypeDefOf.NeutralEvent);
            _clones.Add(new CloneData(plant));
            return true;
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (GenTicks.TicksGame % 500 == 300) RareTick();
        }

        private void RareTick()
        {
            foreach (var clone in Clones.ToList())
            {
                if (clone.status is CloneStatus.Breeding
                    && GenDate.DaysPassedFloat > clone.finishDays)
                {
                    ThingDef newBreed = BreedHelper.AddBreedFromClone(clone);
                    Messages.Message("Succesfully created a new plant species: " + clone.newName, MessageTypeDefOf.NeutralEvent);
                    /*
                     *  check for seedsplease mod and spawn seeds of this new species
                     */
                    if (ModsConfig.IsActive("owlchemist.seedspleaselite") || ModsConfig.IsActive("Evyatar108.SeedsPleaseLiteRedux"))
                    {
                        newBreed.blueprintDef = null;
                        MethodInfo original = AccessTools.Method("SeedsPleaseLite.SeedsPleaseUtility:Setup");
                        original.Invoke(null, new object[] { true });
                        //SeedsPleaseUtility.Setup(true); // regenerates seeds for the new plant
                        float stackCount = 3;
                        Thing newSeeds = ThingMaker.MakeThing(newBreed.blueprintDef, null);
                        newSeeds.stackCount = Mathf.RoundToInt(stackCount);
                        GenPlace.TryPlaceThing(newSeeds, pottingBench.Position, pottingBench.Map, ThingPlaceMode.Near);
                    }
                    clone.status = CloneStatus.Done;
                }
            }
        }
        public void Breed(CloneData clone)
        {
            var thing = DefDatabase<ThingDef>.GetNamed(clone.PlantDef);
            clone.status = CloneStatus.Breeding;
            clone.finishDays = GenDate.DaysPassedFloat + thing.plant.growDays / 2f;
        }

        public void Finish(CloneData clone)
        {
            clone.finishDays = GenDate.DaysPassedFloat;
        }

        private static void SoftRemove(CloneData cloneData)
        {
            cloneData.status = CloneStatus.Removed;
        }

        private void SoftRemove(ThingDef thing)
        {
            if (thing is not null && thing.plant is not null)
            {
                BreedHelper.SowTagsResolverDictionary.TryAdd(thing.defName, thing.plant.sowTags.ToArray());
                thing.plant.sowTags = [];
            }
        }

        private static void HardRemove(ThingDef thing)
        {
            if (thing != null)
            {
                DefDatabase<ThingDef>.Remove(thing);
                thing.ResolveReferences();
            }
        }
        private void HardRemove(CloneData cloneData)
        {
            this.Clones.Remove(cloneData);
        }

        public static bool IsExistGrowingZoneForClone(Map map, ThingDef thing)
        {
            return map.zoneManager.allZones.Any(zone => zone is Zone_Growing growingZone && growingZone.GetPlantDefToGrow().Equals(thing));
        }
        public static bool IsExistPlantOfCloneType(Map map, ThingDef thing)
        {
            return map.listerThings.AnyThingWithDef(thing);
        }

        public void Remove(CloneData clone)
        {
            if (clone.defName is null)
            {
                //No defName -> hard remove
                this.HardRemove(clone);
                return;
            }

            ThingDef thing = DefDatabase<ThingDef>.GetNamed(clone.defName, false);

            if (Clones.Exists(x => x.PlantDef.Equals(clone.defName)))
            {
                //If exist reference to this clone -> soft remove
                SoftRemove(clone);
                SoftRemove(thing);
                return;
            }

            if (thing is null)
            {
                //No thingDef exist -> hard remove
                HardRemove(clone);
                return;
            }

            var maps = world.worldObjects.Settlements
                .Where(x => x.HasMap)
                .Select(x => x.Map)
                .ToList();

            if (maps.Any(map => IsExistGrowingZoneForClone(map, thing) || IsExistPlantOfCloneType(map, thing)))
            {
                //thing exist on any map -> soft remove
                SoftRemove(clone);
                SoftRemove(thing);
                return;
            }
            else
            {
                //thing doesn't exist -> hard remove
                HardRemove(clone);
                HardRemove(thing);
            }
        }

        private void ResolveDuplicates()
        {
            var duplicates = Clones
                .Where(x => x.defName is not null)
                .GroupBy(x => x.defName)
                .Where(x => x.Count() > 1)
                .Select(x => (defName: x.Key, records: x.ToList()))
                .ToList();

            foreach (var duplicate in duplicates)
            {

                Log.Warning("2 or more plants with same defName:");
                foreach (var clone in duplicate.records)
                    Log.Message($"--{clone.newName}");

                var statusLookup = duplicate.records.ToLookup(x => x.status);

                //They must not have def name
                foreach (var clone in statusLookup[null])
                    clone.defName = null;
                foreach (var clone in statusLookup[CloneStatus.Breeding])
                    clone.defName = null;

                CloneData used = null;

                var doneCount = statusLookup[CloneStatus.Done].Count();

                if (doneCount >= 2)
                {
                    Log.Error("More than 2 plants with same defName and done status:");

                    foreach (var record in statusLookup[CloneStatus.Done])
                        Log.Error($"--{record.newName}");

                    used = statusLookup[CloneStatus.Done].OrderBy(x => x.finishDays).LastOrDefault();

                    if (used != null)
                        Log.Message($"Used: {used.newName}");

                }
                else if (doneCount == 1)
                {
                    used = statusLookup[CloneStatus.Done].OrderBy(x => x.finishDays).LastOrDefault();
                    if (used != null)
                        Log.Message($"Used: {used.newName}");
                }
                else
                {
                    var removedCount = statusLookup[CloneStatus.Removed].Count();

                    if (removedCount >= 2)
                    {
                        Log.Error("More than 2 plants with same defName and removed status:");

                        foreach (var record in statusLookup[CloneStatus.Removed])
                            Log.Error($"--{record.newName}");

                        used = statusLookup[CloneStatus.Removed].OrderBy(x => x.finishDays).LastOrDefault();

                        if (used != null)
                            Log.Message($"Used: {used.newName}");
                    }
                    else
                    {
                        used = statusLookup[CloneStatus.Removed].OrderBy(x => x.finishDays).LastOrDefault();
                        if (used != null)
                            Log.Message($"Used: {used.newName}");
                    }
                }

                foreach (var clone in statusLookup[CloneStatus.Done].Concat(statusLookup[CloneStatus.Removed]).Except([used]))
                {
                    Clones.Remove(clone);
                }
            }
        }
        public void InitClones()
        {
            ResolveDuplicates();

            //Log.Message("Adding already discovered breed: " + Clones.Count);
            foreach (var clone in Clones.Where(data => data.defName != null).ToList())
            {
                _ = BreedHelper.AddBreedFromClone(clone);
                Log.Message("Adding already discovered breed: " + clone.newName);
            }
            foreach (var clone in Clones.Where(data => data.status is CloneStatus.Removed).ToList())
            {
                Log.Message("Hide removed breed: " + clone.newName ?? clone.defName ?? $"sample of {clone.PlantDef} with trait {clone.Trait.label}");
                Remove(clone);
            }
            ResourceCounter.ResetDefs();
        }
    }
}