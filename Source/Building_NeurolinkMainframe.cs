using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Neurolink {
	public class Building_NeurolinkMainframe : Building, IThingHolder {

		protected ThingOwner innerContainer = null;





		public void GetChildHolders(List<IThingHolder> outChildren) {
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public ThingOwner GetDirectlyHeldThings() {
			return innerContainer;
		}
	}
}
