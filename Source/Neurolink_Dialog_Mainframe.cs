using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace Neurolink {
	class Neurolink_Dialog_Mainframe : Window {

		private Building_NeurolinkMainframe thing = null;
		//private Neurolink_HarddriveBase selectedHarddrive = null; //%TODO%
		private Pawn selectedHarddrive = null;

		private Neurolink_Dialog_Mainframe.InfoCardTab tab;

		private enum InfoCardTab : byte {
			Character, Stats, Social, Needs
		}

		//constructor
		public Neurolink_Dialog_Mainframe(Thing thing) {
			if (thing.GetType() == typeof(Building_NeurolinkMainframe)) {
				this.thing = (Building_NeurolinkMainframe)thing;
				this.Setup();
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
			this.closeOnClickedOutside = true;
			this.soundAppear = SoundDefOf.InfoCard_Open;
			this.soundClose = SoundDefOf.InfoCard_Close;
		}

		private void FillInfoTabs(Rect cardRect) {
			if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Character) {
				//CharacterCardUtility.DrawCharacterCard(cardRect, this.selectedHarddrive.pawn, null, default(Rect)); //%TODO%
				CharacterCardUtility.DrawCharacterCard(cardRect, this.selectedHarddrive, null, default(Rect));
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Stats) {
				if (this.selectedHarddrive != null) {
					//Thing innerPawn = this.selectedHarddrive.pawn; //%TODO%
					Thing innerThing = this.selectedHarddrive;
					MinifiedThing minifiedThing = (Thing)this.selectedHarddrive as MinifiedThing;
					if (minifiedThing != null) {
						innerThing = minifiedThing.InnerThing;
					}
					StatsReportUtility.DrawStatsReport(cardRect, innerThing);
				} 
			//	else if (this.titleDef != null) { //%TODO%
			//		StatsReportUtility.DrawStatsReport(cardRect, this.titleDef, this.faction);
			//	}
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Social) {
				//SocialCardUtility.DrawSocialCard(cardRect, this.selectedHarddrive.pawn); //%TODO%
				SocialCardUtility.DrawSocialCard(cardRect, this.selectedHarddrive);
			} else if (this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Needs) {
				Vector2 scrollPos = default(Vector2);
				//NeedsCardUtility.DoNeedsMoodAndThoughts(cardRect, this.selectedHarddrive.pawn, ref scrollPos); //%TODO%
				NeedsCardUtility.DoNeedsMoodAndThoughts(cardRect, this.selectedHarddrive, ref scrollPos);
			}
		}

		//draws the window's contents
		public override void DoWindowContents(Rect inRect) {
			//Title
			Rect titleRect = new Rect(inRect);
			titleRect = titleRect.ContractedBy(18f);
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
			innerRect.yMax -= 38f;
			Widgets.DrawLineVertical(innerRect.x + innerRect.width/2, innerRect.y, innerRect.height);
			Text.Font = GameFont.Small;
			//Harddrive Info
			Rect harddriveInfo = new Rect(innerRect.x, innerRect.y, innerRect.width / 2 - 17f, innerRect.height);
			Widgets.DrawBoxSolid(harddriveInfo, Random.ColorHSV());
			if (selectedHarddrive != null) {
				Rect harddriveInfoTabs = harddriveInfo;
				List<TabRecord> tabs = new List<TabRecord>();
				TabRecord stats = new TabRecord("TabStats".Translate(), delegate() {
					this.tab = Neurolink_Dialog_Mainframe.InfoCardTab.Stats;
				}, this.tab == Neurolink_Dialog_Mainframe.InfoCardTab.Stats);
				tabs.Add(stats);
				//if (selectedHarddrive.pawn.RaceProps.Humanlike) { //%TODO%
				if (selectedHarddrive.RaceProps.Humanlike) {
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
					TabDrawer.DrawTabs(harddriveInfoTabs, tabs, harddriveInfo.width);
				}
				this.FillInfoTabs(harddriveInfo.ContractedBy(18f));
			}
			//Harddrives
			Rect harddrivesList = new Rect(innerRect.width/2 + 17f, innerRect.y, innerRect.width / 2 - 17f, innerRect.height);
			Widgets.DrawBoxSolid(harddrivesList, Random.ColorHSV());
			if (!thing.GetDirectlyHeldThings().NullOrEmpty()) {
				List<Thing> contents = ThingOwnerUtility.GetAllThingsRecursively(thing);
				Log.Message(contents.ToString());
				Rect[] hdRect = new Rect[contents.Count];
				for (int i = 0; i < contents.Count; i++) {
					float hdRectY = (harddrivesList.yMax / contents.Count);
					hdRect[i] = new Rect(harddrivesList.x, harddrivesList.y + hdRectY * i, harddrivesList.width, hdRectY);
					Widgets.DrawBox(hdRect[i]);
				}
			}
			//Simulation
		}
	}
}
