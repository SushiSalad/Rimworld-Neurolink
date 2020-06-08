using System.Collections.Generic;
using Verse.AI;

namespace Neurolink {
	class Neurolink_JobDriver_UseMainframe : JobDriver {
		public override bool TryMakePreToilReservations(bool errorOnFailed) {
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils() {
			this.FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn((Toil to) => !((Building_NeurolinkMainframe)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseMainframeNow);
			yield break;
		}
	}
}
