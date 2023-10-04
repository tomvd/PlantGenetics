using HarmonyLib;
using PlantGenetics.Comp;
using PlantGenetics.Utilities;
using RimWorld;
using Verse;

namespace PlantGenetics.Patches;

public class PlantCrossBreed
{
    /// <summary>
    /// Alter DNA by plants surrounding this plant the moment it changes lifestate
    /// </summary>
    [HarmonyPatch(typeof(Plant), nameof(Plant.TickLong))]
    public class TickLong
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance, out PlantLifeStage __state)
        {
            __state = __instance.LifeStage;
        }
        
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, PlantLifeStage __state)
        {
            if (__state != __instance.LifeStage && __instance.LifeStage == PlantLifeStage.Mature)
            {
                DNAUtility.CrossBreed(__instance);
            }
        }
    }
    

}