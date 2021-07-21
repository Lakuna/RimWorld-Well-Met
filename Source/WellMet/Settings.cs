using Verse;

namespace WellMet {
	public class Settings : ModSettings {
		public float daysToUnlockPerCommonality = 15;

		public override void ExposeData() {
			Scribe_Values.Look(ref this.daysToUnlockPerCommonality, "daysToUnlockPerCommonality");

			base.ExposeData();
		}
	}
}
