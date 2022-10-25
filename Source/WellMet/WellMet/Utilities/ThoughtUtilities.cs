using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet.Utilities {
	public static class ThoughtUtilities {
		public const string UnknownThoughtName = "thought from unknown trait";

		public const string UnknownThoughtDescription = "You haven't discovered the trait that causes this thought yet.";

		public static bool ThoughtIsHidden(Thought thought) => thought == null
			? throw new ArgumentNullException(nameof(thought))
			: !thought.def.requiredTraits.NullOrEmpty()
			&& thought.def.requiredTraits.Any((TraitDef traitDef) => thought.pawn.story.traits.HasTrait(traitDef)
				&& !TraitUtilities.TraitIsDiscovered(thought.pawn, traitDef));
	}
}
