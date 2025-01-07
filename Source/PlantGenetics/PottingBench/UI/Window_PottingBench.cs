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
            clone.newName = BreedHelper.GetNameSuggestionFromCloneDataGeneral(clone, _pottingService.Clones);
            Find.WindowStack.Add(new Dialog_GivePlantName(clone));
            _pottingService.Breed(clone);
        }

        public const float BasicHeight = 20;
        private void DoClonesList(ref Rect inRect)
        {
            
            Rect NewRect(Rect rect, float offset, float width) {
                return new Rect(rect.x + rect.width + offset, rect.y, width, rect.height);
            }

            var listRect = inRect.TakeBottomPart(inRect.height - BasicHeight);
            var baseRect = inRect.TakeTopPart(20f);
            baseRect.x += 15;
            baseRect.width = 0;

            var infoRect = NewRect(baseRect, 0, 20);

            var nameRect = NewRect(infoRect, 10, 200);

            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Tiny;
            Widgets.Label(nameRect, "ColumnPlant".Translate());

            var traitRect = NewRect(nameRect, 10, 120);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(traitRect, "ColumnTrait".Translate());

            var actionRect = NewRect(traitRect, 10, 120);

            Widgets.Label(actionRect, "ColumnAction".Translate());

            var removeRect = NewRect(actionRect, 10, 120);

            Widgets.Label(removeRect, "ColumnRemove".Translate());

            var totalWidth = removeRect.width + removeRect.x - baseRect.x;

            var viewRect = new Rect(0f, 0f, listRect.width - 60, BasicHeight * (_pottingService.Clones.Count(clone => clone.status is not CloneStatus.Removed) + 1));
            Widgets.BeginScrollView(listRect, ref scrollPos, viewRect);
            
            GUI.color = Color.white;
            var highlight = true;
            foreach (var clone in _pottingService.Clones.Where(clone => clone.status is not CloneStatus.Removed).ToList())
            {
                nameRect.y += BasicHeight;
                traitRect.y += BasicHeight;
                actionRect.y += BasicHeight;
                removeRect.y += BasicHeight;
                infoRect.y += BasicHeight;
                var fullRect = new Rect(baseRect.x - 4f, nameRect.y, totalWidth + 4, BasicHeight);
                if (highlight) Widgets.DrawHighlight(fullRect);
                highlight = !highlight;
                Text.Anchor = TextAnchor.MiddleLeft;
                var fullname = clone.newName ?? DefDatabase<ThingDef>.GetNamed(clone.PlantDef).label;
                string name = fullname;
                //Fix gui
                if (name.Length > 30)
                    name = name.Substring(0, 27) + "...";

                Widgets.Label(nameRect, name);
                Text.Anchor = TextAnchor.MiddleCenter;
                
                if (clone.Trait != null)
                    Widgets.Label(traitRect, clone.Trait.label);

                if (clone.status == null) {

                    if (Widgets.ButtonText(actionRect, "ButtonBreed".Translate())) {
                        OnBreedKeyPressed(clone);
                    }

                    var plantDef = BreedHelper.CreateThingDefFromCloneData(clone);
                    plantDef.label = BreedHelper.GetNameSuggestionFromCloneDataGeneral(clone, _pottingService.Clones);
                    _ = Widgets.InfoCardButton(infoRect, plantDef);

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
                        Widgets.Label(actionRect,"DaysLeft".Translate((clone.finishDays - GenDate.DaysPassed).ToStringDecimalIfSmall()));
                    }

                    var plantDef = BreedHelper.CreateThingDefFromCloneData(clone);
                    plantDef.label = clone.newName;
                    _ = Widgets.InfoCardButton(infoRect, plantDef);
                }

                if (clone.status is CloneStatus.Done)
                {
                    if (Widgets.ButtonText(actionRect, "ButtonRename".Translate()))
                    {
                        Find.WindowStack.Add(new Dialog_GivePlantName(clone));
                    }
                    var plantDef = DefDatabase<ThingDef>.GetNamed(clone.defName, false);
                    if (plantDef is not null && Widgets.InfoCardButton(infoRect, plantDef))
                    {

                    }
                }

                if (Widgets.ButtonText(removeRect, "ButtonRemove".Translate()))
                {
                    string warning = "";
                    if (clone.defName != null)
                    {
                        ThingDef named = DefDatabase<ThingDef>.GetNamed(clone.defName, false);
                        if (named != null)
                        {
                            if (PottingService.IsExistGrowingZoneForClone(map, named))
                            {
                                warning = "WarningGrowthZone".Translate() + " ";
                            }
                            else if (PottingService.IsExistPlantOfCloneType(map, named))
                            {
                                warning = "WarningStillExists".Translate() + " ";
                            }
                        }
                    }
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                        warning + "AboutToRemove".Translate(clone.newName), 
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