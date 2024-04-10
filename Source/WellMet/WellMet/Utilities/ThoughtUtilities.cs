using RimWorld;
using System;
using Verse;

namespace Lakuna.WellMet.Utilities {
	public static class ThoughtUtilities {
		public static bool ThoughtIsDiscovered(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: thought.def.requiredTraits.NullOrEmpty()
			|| thought.def.requiredTraits.Any((TraitDef def) => TraitUtilities.TraitIsDiscovered(thought.pawn, def));
	}
}
