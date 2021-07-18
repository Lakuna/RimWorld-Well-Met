using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace WellMet {
	public static class WellMet {
		public static bool TraitDiscovered(Trait trait) =>
			// trait.pawn.records.GetValue(RecordDefOf.TimeAsColonistOrColonyAnimal);

			false; // TODO

		public static List<Trait> FilterDiscovered(List<Trait> traits) => traits.Where((trait) => WellMet.TraitDiscovered(trait)).ToList();
	}
}
