using System.Linq;
using PlantGenetics.PottingBench.UI;
using RimWorld;
using UnityEngine;
using Verse;

namespace PlantGenetics
{
    public class Window_PottingBench : Window
    {
        public readonly Pawn actor;

        private readonly Map map;
        private readonly PottingService _pottingService;
        
        public Window_PottingBench(Pawn actor)
        {
            map = actor.Map;
            this.actor = actor;
            _pottingService = Find.World.GetComponent<PottingService>();
            closeOnCancel = true;
            forcePause = true;
            closeOnAccept = true;
        }

        public override Vector2 InitialSize => new Vector2(800, Mathf.Min(740, UI.screenHeight));
        public override float Margin => 5f;

        public override void DoWindowContents(Rect inRect)
        {
            var anchor = Text.Anchor;
            var font = Text.Font;
            var rect = new Rect(inRect);
            var buttonBarRect = rect.TakeBottomPart(40f);
            Widgets.DrawHighlight(buttonBarRect);
            var topBarRect = rect.TakeTopPart(40f);
            Widgets.DrawHighlight(topBarRect);
            DoTopBarContents(topBarRect);

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;

            DoClonesList(ref rect);
            Text.Anchor = anchor;
            Text.Font = font;

            if (Widgets.ButtonText(buttonBarRect.TakeRightPart(120f), "Close".Translate())) OnCancelKeyPressed();
            Text.Anchor = anchor;
            Text.Font = font;
        }

        public void ShutDownComputer()
        {
            base.Close();
        }

        /*
         * Pots in use: x/12
         */
        private void DoTopBarContents(Rect inRect)
        {
            inRect.width = 150f;
            Widgets.Label(inRect,
                "Pots in use: "+_pottingService.Clones.Where(clone => clone.status is not "done").ToList().Count+"/12");

        }
        
        private void OnBreedKeyPressed(CloneData pawn)
        {
            //SoundDefOf.Interact_Sow.PlayOneShotOnCamera();
            Find.WindowStack.Add(new Dialog_GivePlantName(pawn));
            _pottingService.Breed(pawn);
        }

        private void DoClonesList(ref Rect inRect)
        {
            var rect = inRect.TopPartPixels(Mathf.Max(20f + _pottingService.Clones.Count * 30f, 120f));
            inRect.yMin += rect.height;
            var titleRect = rect.TakeTopPart(20f);
            var iconRect = rect.LeftPartPixels(105f).ContractedBy(5f);
            titleRect.x += 15f;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Tiny;
            var nameRect = new Rect(titleRect);
            Widgets.Label(titleRect, "plant");
            titleRect.x += 150f;
            titleRect.width = 120f;
            Text.Anchor = TextAnchor.MiddleCenter;
            var valueRect = new Rect(titleRect);
            Widgets.Label(titleRect, "trait");
            titleRect.x += 100f;
            titleRect.width = 200f;
            var numRect = new Rect(titleRect);
            Widgets.Label(titleRect, "action");
            GUI.color = Color.white;
            var highlight = true;
            foreach (var clone in _pottingService.Clones.Where(clone => clone.status is not "done").ToList())
            {
                nameRect.y += 20f;
                valueRect.y += 20f;
                numRect.y += 20f;
                var fullRect = new Rect(nameRect.x - 4f, nameRect.y, nameRect.width + valueRect.width + numRect.width,
                    20f);
                if (highlight) Widgets.DrawHighlight(fullRect);
                highlight = !highlight;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(nameRect, clone.newName ?? clone.PlantDef.label);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(valueRect, clone.status != null ? "" : clone.Trait.label);


                if (clone.status == null && Widgets.ButtonText(numRect, "Breed"))
                {
                        OnBreedKeyPressed(clone);
                }
                if (clone.status != null)
                {
                    Widgets.Label(numRect, clone.status + " " + (clone.finishDays - GenDate.DaysPassed).ToStringDecimalIfSmall() + " days");
                }
                if (DebugSettings.ShowDevGizmos && clone.status is "breeding" && Widgets.ButtonText(numRect, "Debug - finish"))
                {
                    _pottingService.Finish(clone);
                }
            }
        }        
    }
}