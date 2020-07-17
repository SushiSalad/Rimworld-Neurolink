using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace Neurolink {
	class Neurolink_Dialog_Mainframe : Window {

		private Building_NeurolinkMainframe thing = null;
		private Neurolink_Harddrive selectedHarddrive = null;

		private Neurolink_Dialog_Mainframe.InfoCardTab tab;
		private Vector2 scrollPosition = Vector2.zero;
		private float listHeight;

		public override Vector2 InitialSize {
			get {
				return new Vector2(UI.screenWidth * .6f, UI.screenHeight * .6f);
			}
		}

		private enum InfoCardTab : byte {
			Character, Stats, Social, Needs
		}

		//constructor
		public Neurolink_Dialog_Mainframe(Thing thing) {
			if (thing.GetType() == typeof(Building_NeurolinkMainframe)) {
				this.thing = (Building_NeurolinkMainframe)thing;
				this.Setup();
				if (this.thing.GetDirectlyHeldThings().Count > 0) {
					this.selectedHarddrive = this.thing.GetDirectlyHeldThings().RandomElement() as Neurolink_Harddrive; //%TEMP%
				}
			} else {
				Debug.Log("[Neurolink_Dialog_Mainframe ERROR] Not supplied with thing of type Building_NeurolinkMainframe");
			}
		}

		//sets up the windows properties
		private void Setup() {
			this.forcePause = true;
			this.doCloseButton = true;
			this.doCloseX = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = false;
			this.soundAppear = SoundDefOf.InfoCard_Open;
			this.soundClose = SoundDefOf.InfoCard_Close;
			this.draggable = true;
		}

		//draws the harddrive's info tabs
		private void FillInfoTabs(Rect cardRect) {
			if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Character) {
				CharacterCardUtility.DrawCharacterCard(cardRect, this.selectedHarddrive.pawn, null, default(Rect));
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Stats) {
				if (this.selectedHarddrive != null) {
					Thing innerPawn = this.selectedHarddrive.pawn;
					MinifiedThing minifiedThing = (Thing)this.selectedHarddrive as MinifiedThing;
					if (minifiedThing != null) {
						innerPawn = minifiedThing.InnerThing;
					}
					StatsReportUtility.DrawStatsReport(cardRect, innerPawn);
				} 
			//	else if (this.titleDef != null) { //%TODO%
			//		StatsReportUtility.DrawStatsReport(cardRect, this.titleDef, this.faction);
			//	}
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Social) {
				SocialCardUtility.DrawSocialCard(cardRect, this.selectedHarddrive.pawn);
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Needs) {
				Vector2 scrollPos = default(Vector2);
				NeedsCardUtility.DoNeedsMoodAndThoughts(cardRect, this.selectedHarddrive.pawn, ref scrollPos);
			}
		}

		//draws the window's contents
		public override void DoWindowContents(Rect inRect) {
			//Title
			Rect titleRect = new Rect(inRect);
			titleRect = titleRect.ContractedBy(17f);
			titleRect.height = 34f;
			titleRect.x += 34f;
			Text.Font = GameFont.Medium;
			Widgets.Label(titleRect, this.thing.LabelCapNoCount);
			Rect titleIconRect = new Rect(inRect.x + 9f, titleRect.y, 34f, 34f);
			if (this.thing != null) {
				Widgets.ThingIcon(titleIconRect, this.thing, 1f);
			}
			//Inner Rectangle
			Rect innerRect = new Rect(inRect);
			innerRect.yMin = titleRect.yMax;
			innerRect.yMax -= 34f;
			Widgets.DrawLineVertical(innerRect.x + innerRect.width/2, innerRect.y, innerRect.height);
			Text.Font = GameFont.Small;
			//Harddrive Info
			Rect harddriveInfo = new Rect(innerRect.x, innerRect.y, innerRect.width / 2 - 17f, innerRect.height);
			Rect cardRect = new Rect(harddriveInfo);
			cardRect.y += 34f;
			cardRect.yMax = innerRect.yMax;
			if (selectedHarddrive != null) {
				Rect harddriveInfoTabs = harddriveInfo;
				List<TabRecord> tabs = new List<TabRecord>();
				TabRecord stats = new TabRecord("TabStats".Translate(), delegate() {
					this.tab = Neurolink_Dialog_Mainframe.InfoCardTab.Stats;
				}, this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Stats);
				tabs.Add(stats);
				if (selectedHarddrive.pawn.RaceProps.Humanlike) {
					TabRecord character = new TabRecord("TabCharacter".Translate(), delegate () {
						this.tab = Neurolink_Dialog_Mainframe.InfoCardTab.Character;
					}, this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Character);
					tabs.Add(character);
				}
				TabRecord social = new TabRecord("TabSocial".Translate(), delegate () {
					this.tab = Neurolink_Dialog_Mainframe.InfoCardTab.Social;
				}, this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Social);
				tabs.Add(social);
				TabRecord needs = new TabRecord("TabNeeds".Translate(), delegate () {
					this.tab = Neurolink_Dialog_Mainframe.InfoCardTab.Needs;
				}, this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Needs);
				tabs.Add(needs);
				if (tabs.Count > 1) {
					harddriveInfoTabs.yMin += 45f;
					TabDrawer.DrawTabs(harddriveInfoTabs, tabs, cardRect.width);
				}
				this.FillInfoTabs(cardRect.ContractedBy(17f));
			}
			//Harddrives
			Rect harddrivesList = new Rect(innerRect.width/2 + 17f, innerRect.y, innerRect.width / 2 - 17f, innerRect.height);
			Widgets.DrawBoxSolid(harddrivesList, Color.black);
			if (!thing.GetDirectlyHeldThings().NullOrEmpty()) {
				Thing[] contents = ThingOwnerUtility.GetAllThingsRecursively(thing).ToArray();
				Rect[] hdRect = new Rect[contents.Length];
				Color buttonBgColor;
				string buttonText = null;
				Widgets.BeginScrollView(harddrivesList, ref this.scrollPosition, harddrivesList, true); //%TODO%
				for (int i = 0; i < contents.Length; i++) {
					Pawn pawn = ((Neurolink_Harddrive)contents[i]).pawn;
					hdRect[i] = new Rect(harddrivesList.x, harddrivesList.y + 100f * i, harddrivesList.width, 100f);
					buttonBgColor = Mouse.IsOver(hdRect[i]) ? Color.green : Color.gray;
					buttonText = pawn.GetHashCode() + " | " + pawn.Name.ToStringFull + " | " + pawn.story.TitleCap 
						+ " | " + pawn.ageTracker.AgeChronologicalYears;
					if (Widgets.CustomButtonText(ref hdRect[i], buttonText, buttonBgColor, Color.white, Color.black)) {
						this.selectedHarddrive = (Neurolink_Harddrive)contents.GetValue(i);
					}
				}
				Widgets.EndScrollView();
			}
			//Simulation %TODO%
		}
	}
}
