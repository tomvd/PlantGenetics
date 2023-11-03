using HarmonyLib;
using RimWorld;
using UnityEngine;
using VendingMachines;
using Verse;

namespace PlantGenetics;

[DefOf]
public static class InternalDefOf
{
	static InternalDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
	}
	public static readonly DesignationDef ClonePlant;
	public static readonly JobDef ClonePlantJob;
	public static readonly JobDef UsePottingBench;
	public static readonly ThingDef PottingBench;

}

public class PlantGeneticsMod : Mod
{
	public static Harmony harmonyInstance;
	public static Settings Settings;
	
	public PlantGeneticsMod(ModContentPack content) : base(content)
	{
		Settings = GetSettings<Settings>();
		//Log.Message("PlantGeneticsMod:start ");
		harmonyInstance = new Harmony("Adamas.PlantGenetics");
		harmonyInstance.PatchAll();        
        
    }
	
	/// <summary>
    /// The (optional) GUI part to set your settings.
    /// </summary>
    /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DoWindowContents(inRect);
    }

    /// <summary>
    /// Override SettingsCategory to show up in the list of settings.
    /// Using .Translate() is optional, but does allow for localisation.
    /// </summary>
    /// <returns>The (translated) mod name.</returns>
    public override string SettingsCategory()
    {
        return "Plant genetics";
    }
}