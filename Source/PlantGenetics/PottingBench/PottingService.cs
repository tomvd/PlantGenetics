using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using SeedsPleaseLite;
using UnityEngine;
using Verse;

namespace PlantGenetics
{
    /*
     * has an inventory of clippings/clones we have gathered
     * keeps track of breeding / crossbreeding / mutation jobs
     */
    public class PottingService : MapComponent
    {
      
        private List<CloneData> _clones = new List<CloneData>();
        private Building pottingBench;

        public PottingService(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _clones, "clones", LookMode.Deep);
            Scribe_References.Look(ref pottingBench, "pottingBench");
            _clones ??= new List<CloneData>();            
        }

        public List<CloneData> Clones => _clones;

        public bool AddClone(Plant plant, Building bench)
        {
            pottingBench = bench;
            Messages.Message("Successfully added a clipping to the potting bench.", MessageTypeDefOf.NeutralEvent);
            _clones.Add(new CloneData(plant));
            return true;
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (GenTicks.TicksGame % 500 == 300) RareTick();
        }

        private void RareTick()
        {
            foreach (var clone in Clones.ToList())
            {
                if (clone.status != null && GenDate.DaysPassedFloat > clone.finishDays)
                {
                    if (clone.status.Equals("breeding"))
                    {
                        ThingDef newBreed = BreedHelper.AddBreedFromClone(clone);
                        /*
                         *  check for seedsplease mod and spawn seeds of this new species
                         */
                        if (ModsConfig.IsActive("owlchemist.seedspleaselite"))
                        {
                            newBreed.blueprintDef = null;
                            MethodInfo original = AccessTools.Method("SeedsPleaseLite.SeedsPleaseUtility:Setup");
                            original.Invoke(null, new object[]{true});
                            //SeedsPleaseUtility.Setup(true); // regenerates seeds for the new plant
                            float stackCount = 3;
                            Thing newSeeds = ThingMaker.MakeThing(newBreed.blueprintDef, null);
                            newSeeds.stackCount = Mathf.RoundToInt(stackCount);
                            GenPlace.TryPlaceThing(newSeeds, pottingBench.Position, pottingBench.Map, ThingPlaceMode.Near);
                        }                        
                        Clones.Remove(clone);
                    }
                }
            }
        }
        public void Breed(CloneData clone)
        {
            clone.status = "breeding";
            clone.finishDays = GenDate.DaysPassedFloat + clone.PlantDef.plant.growDays / 2f;
        }

        public void Finish(CloneData clone)
        {
            clone.finishDays = GenDate.DaysPassedFloat;
        }
    }
}