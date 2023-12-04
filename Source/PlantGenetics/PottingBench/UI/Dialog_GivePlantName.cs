using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics.PottingBench.UI;

public class Dialog_GivePlantName : Window
{
	protected string curName;
	public CloneData Plant;

	private float Height
	{
		get
		{
				return 200f;
		}
	}

	public override Vector2 InitialSize => new Vector2(640f, Height);

	protected virtual int FirstCharLimit => 64;

	public Dialog_GivePlantName(CloneData plant)
	{
		forcePause = true;
		closeOnAccept = false;
		closeOnCancel = false;
		absorbInputAroundWindow = true;
		Plant = plant;
		curName = plant.Trait.LabelCap +" " + DefDatabase<ThingDef>.GetNamed(plant.PlantDef).LabelCap;
	}

	public override void DoWindowContents(Rect rect)
	{
		Text.Font = GameFont.Small;
		bool flag = false;
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
		{
			flag = true;
			Event.current.Use();
		}
		Rect rect2;
		Widgets.Label(new Rect(0f, 0f, rect.width, rect.height), "give name for new plant".CapitalizeFirst());
		/*if (nameGenerator != null && Widgets.ButtonText(new Rect(rect.width / 2f + 90f, 80f, rect.width / 2f - 90f, 35f), "Randomize".Translate()))
		{
			curName = nameGenerator();
		}*/
		curName = Widgets.TextField(new Rect(0f, 80f, rect.width / 2f + 70f, 35f), curName, FirstCharLimit);
		rect2 = new Rect(rect.width / 2f + 90f, rect.height - 35f, rect.width / 2f - 90f, 35f);
		if (!(Widgets.ButtonText(rect2, "OK".Translate()) || flag))
		{
			return;
		}
		string text2 = curName?.Trim();
		if (IsValidName(text2))
		{
			Plant.newName = text2;
			Find.WindowStack.TryRemove(this);
		}
		else
		{
			Messages.Message("invalidname".Translate(), MessageTypeDefOf.RejectInput, historical: false);
		}
		Event.current.Use();
	}

	protected bool IsValidName(string s)
	{
		return true;
	}
}