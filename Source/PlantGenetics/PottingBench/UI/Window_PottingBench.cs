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
        private Vector2 scrollPos;
        
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
            //var topBarRect = rect.TakeTopPart(40f);
            //Widgets.DrawHighlight(topBarRect);
            //DoTopBarContents(topBarRect);

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
            //Widgets.Label(inRect,
//                "Pots in use: "+_pottingService.Clones.Where(clone => clone.status is not "done" && clone.status is not "removed").ToList().Count+"/12");

        }
        
        private void OnBreedKeyPressed(CloneData clone)
        {
            //SoundDefOf.Interact_Sow.PlayOneShotOnCamera();
            clone.newName = BreedHelper.GetNameSuggestionFromCloneDataV2(clone, _pottingService.Clones);
            Find.WindowStack.Add(new Dialog_GivePlantName(clone));
            _pottingService.Breed(clone);
        }

        private void DoClonesList(ref Rect inRect)
        {
            var listRect = inRect.TakeBottomPart(inRect.height - 20);
            var titleRect = inRect.TakeTopPart(20f);
            titleRect.x += 15f;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Tiny;
            var nameRect = new Rect(titleRect);
            Widgets.Label(titleRect, "plant");
            titleRect.x += 150f;
            titleRect.width = 120f;
            Text.Anchor = TextAnchor.MiddleCenter;
            var traitRect = new Rect(titleRect);
            Widgets.Label(titleRect, "trait");
            titleRect.x += 100f;
            titleRect.width = 70f;
            var actionRect = new Rect(titleRect);
            Widgets.Label(titleRect, "action");
            titleRect.x += 100f;
            titleRect.width = 70f;
            var removeRect = new Rect(titleRect);
            Widgets.Label(titleRect, "remove");
            var viewRect = new Rect(0f, 0f, listRect.width - 60, 20f*_pottingService.Clones.Count(clone => clone.status is not CloneStatus.Removed) + 20f);
            Widgets.BeginScrollView(listRect, ref scrollPos, viewRect);
            
            GUI.color = Color.white;
            var highlight = true;
            foreach (var clone in _pottingService.Clones.Where(clone => clone.status is not CloneStatus.Removed).ToList())
            {
                nameRect.y += 20f;
                traitRect.y += 20f;
                actionRect.y += 20f;
                removeRect.y += 20f;
                var fullRect = new Rect(nameRect.x - 4f, nameRect.y, nameRect.width + traitRect.width + actionRect.width,
                    20f);
                if (highlight) Widgets.DrawHighlight(fullRect);
                highlight = !highlight;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(nameRect, clone.newName ?? DefDatabase<ThingDef>.GetNamed(clone.PlantDef).label);
                Text.Anchor = TextAnchor.MiddleCenter;
                
                if (clone.Trait != null)
                    Widgets.Label(traitRect, clone.Trait.label);

                if (clone.status == null && Widgets.ButtonText(actionRect, "Breed"))
                {
                    OnBreedKeyPressed(clone);
                }
                if (clone.status is CloneStatus.Breeding)
                {
                    if (DebugSettings.ShowDevGizmos)
                    {
                        if (Widgets.ButtonText(actionRect, "Debug - finish"))
                            _pottingService.Finish(clone);
                    }
                    else
                    {
                        Widgets.Label(actionRect,(clone.finishDays - GenDate.DaysPassed).ToStringDecimalIfSmall() +
                            " days left");
                    }
                }

                if (clone.status is CloneStatus.Done)
                {
                    if (Widgets.ButtonText(actionRect, "Rename"))
                    {
                        Find.WindowStack.Add(new Dialog_GivePlantName(clone));
                    }
                }

                if (Widgets.ButtonText(removeRect, "Remove"))
                {
                    string warning = "";
                    if (clone.defName != null)
                    {
                        ThingDef named = DefDatabase<ThingDef>.GetNamed(clone.defName, false);
                        if (named != null)
                        {
                            if (PottingService.IsExistGrowingZoneForClone(map, named))
                            {
                                warning = "There is a growth zone for this plant. ";
                            }
                            else if (PottingService.IsExistPlantOfCloneType(map, named))
                            {
                                warning = "Plants of this type still exist. ";
                            }
                        }
                    }
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        warning + "You are sure about to remove " + clone.newName + "?", 
                        () => _pottingService.Remove(clone), 
                        true, 
                        "AreYouSure".Translate(), 
                        WindowLayer.Dialog
                    ));
                }
            }
            Widgets.EndScrollView();
        }        
    }
}