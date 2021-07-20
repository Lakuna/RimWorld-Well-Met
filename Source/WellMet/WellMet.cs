using RimWorld;

namespace WellMet {
	public static class WellMet {
		private const int TicksPerDay = 60000;
		public const string UnknownTraitLabel = "???";

		public static bool TraitDiscovered(Trait trait) {
			// Can't use a switch statement because TraitDefOfs are not constant values.
			if (trait.def == TraitDefOf.Bloodlust) {
				return trait.pawn.records.GetValue(RecordDefOf.Kills) > 0;
			} else if (trait.def == TraitDefOf.Brawler) {
				return trait.pawn.records.GetValue(RecordDefOf.ShotsFired) > 0;
			} else if (trait.def == TraitDefOf.Masochist || trait.def == TraitDefOf.Tough || trait.def == TraitDefOf.Wimp) {
				return trait.pawn.records.GetValue(RecordDefOf.DamageTaken) > 0;
			} else if (trait.def == TraitDefOf.Transhumanist || trait.def == TraitDefOf.BodyPurist) {
				return trait.pawn.records.GetValue(RecordDefOf.OperationsReceived) > 0;
			} else if (trait.def == TraitDefOf.Pyromaniac) {
				return trait.pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 0;
			} else {
				return trait.pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) > trait.def.GetGenderSpecificCommonality(trait.pawn.gender) * (TicksPerDay * 15);
			}
		}
	}
}
