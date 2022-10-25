using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet.Utilities {
	public static class ThoughtUtilities {
		public static bool ThoughtIsDiscovered(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: thought.def.requiredTraits.NullOrEmpty()
			|| thought.def.requiredTraits.Any((TraitDef def) => TraitUtilities.TraitIsDiscovered(thought.pawn, def));
	}
}
