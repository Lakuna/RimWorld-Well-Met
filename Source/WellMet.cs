using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WellMet {
	public static class WellMet {
		private const int TicksPerDay = 60000;

		// TODO: Hide needs (ITab_Pawn_Needs)
		// TODO: Hide social thoughts.

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
			} else if (trait.def == TraitDefOf.Pyromaniac || trait.def.LabelCap == "TorturedArtist") { // TraitDefOf doesn't contain TorturedArtist for some reason?
				return trait.pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 0;
			} else {
				return trait.pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal) > trait.def.GetGenderSpecificCommonality(trait.pawn.gender) * (TicksPerDay * 15);
			}
		}

		public static List<Trait> FilterDiscovered(List<Trait> traits) => traits.Where((trait) => WellMet.TraitDiscovered(trait)).ToList();
	}
}
