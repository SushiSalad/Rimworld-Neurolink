using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Neurolink {
	class Neurolink_JobDriver_UseMainframe : JobDriver {

		//Trys to reserve the building by the pawn
		public override bool TryMakePreToilReservations(bool errorOnFailed) {
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		//Returns a enumerable toil list where the toil will fail if the pawn can't use the mainframe
		protected override IEnumerable<Toil> MakeNewToils() {
			//Go to mainframe
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn((Toil to) => 
				!((Building_NeurolinkMainframe)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseMainframeNow);
			//Use mainframe
			Toil useMainframe = new Toil();
			useMainframe.initAction = delegate () {
				Pawn actor = useMainframe.actor;
				if (((Building_NeurolinkMainframe)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseMainframeNow) {
					Find.WindowStack.Add(new Neurolink_Dialog_Mainframe(actor.jobs.curJob.GetTarget(TargetIndex.A).Thing));
				}
			};
			yield return useMainframe;
			yield break;
		}
	}
}
