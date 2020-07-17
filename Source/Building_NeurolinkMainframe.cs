using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Neurolink {
	public class Building_NeurolinkMainframe : Building, IThingHolder {

		protected ThingOwner innerContainer = null;
		private int harddriveSlots = 1;
		private CompPowerTrader powerComp;


		//Checks if the building is usable by the pawn directed to do the job.
		public bool CanUseMainframeNow { 
			get {
				return (!base.Spawned || !base.Map.gameConditionManager.ElectricityDisabled) 
							&& (this.powerComp == null || this.powerComp.PowerOn);
			}
		}

		//Establishes that this building remains after reload and implements the tutor
		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.innerContainer = new ThingOwner<Neurolink_Harddrive>(this);
			//LessonAutoActivator.TeachOpportunity(ConceptDefOf.BuildHarddrive, OpportunityType.GoodToKnow);
			LessonAutoActivator.TeachOpportunity(Neurolink_ConceptDefOf.Neurolink_UsingMainframe, OpportunityType.GoodToKnow);
		}

		//Adds float menu options %TODO%: generify this so that it can accept the menu options
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn) { 
			FloatMenuOption failureReason = this.GetFailureReason(myPawn);
			if (failureReason != null) {
				yield return failureReason;
				yield break;
			}
			//Use mainframe
			Action action = delegate () {
				Job job = JobMaker.MakeJob(Neurolink_JobDefOf.Neurolink_UseMainframe, this);
				myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(Neurolink_ConceptDefOf.Neurolink_UsingMainframe, KnowledgeAmount.Total);
				innerContainer.TryAdd(new Neurolink_Harddrive(myPawn), false);
			};
			yield return new FloatMenuOption("Use neurolink mainframe", action, MenuOptionPriority.Default, null, null, 0f, null, null);
			//Insert harddrive

			//Remove harddrive

			//Simulate...
			yield break;
		}

		//Adds failure float menu options for various reasons
		private FloatMenuOption GetFailureReason(Pawn myPawn) { 
			if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some, false, TraverseMode.ByPawn)) {
				return new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (base.Spawned && base.Map.gameConditionManager.ElectricityDisabled) {
				return new FloatMenuOption("CannotUseSolarFlare".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (this.powerComp != null && !this.powerComp.PowerOn) {
				return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (!myPawn.RaceProps.Humanlike) {
				return new FloatMenuOption("Pawn is not humanlike.", null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (myPawn.Drafted) {
				return new FloatMenuOption("Pawn is drafted.", null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (!(base.Faction == Faction.OfPlayer)) {
				return new FloatMenuOption("Pawn is not part of player's faction.", null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			if (!this.CanUseMainframeNow) {
				Log.Error(myPawn + " could not use mainframe for unknown reason.", false);
				return new FloatMenuOption("Cannot use now", null, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			return null;
		}

		//Adds the children containers (ThingHolders) to the input list
		public void GetChildHolders(List<IThingHolder> outChildren) { 
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		//Returns the container (ThingOwner) that holds the items within the mainframe
		public ThingOwner GetDirectlyHeldThings() { 
			return innerContainer;
		}
	}
}
