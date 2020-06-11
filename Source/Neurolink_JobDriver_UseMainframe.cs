using System.Collections.Generic;
using Verse.AI;

namespace Neurolink {
	class Neurolink_JobDriver_UseMainframe : JobDriver {

		//Trys to reserve the building by the pawn
		public override bool TryMakePreToilReservations(bool errorOnFailed) {
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		//Returns a enumerable toil list where the toil will fail if the pawn can't use the mainframe
		protected override IEnumerable<Toil> MakeNewToils() {
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn((Toil to) => 
				!((Building_NeurolinkMainframe)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseMainframeNow);
			yield break;
		}
	}
}
