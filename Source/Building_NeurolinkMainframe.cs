using System.Collections.Generic;
using Verse;

namespace Neurolink {
	public abstract class Building_NeurolinkMainframe : Building, IThingHolder {
		public ThingOwner innerContainer = null;

		public Building_NeurolinkMainframe() {
			this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
		}






		public void GetChildHolders(List<IThingHolder> outChildren) {
			throw new System.NotImplementedException();
		}

		public ThingOwner GetDirectlyHeldThings() {
			throw new System.NotImplementedException();
		}
	}
}
