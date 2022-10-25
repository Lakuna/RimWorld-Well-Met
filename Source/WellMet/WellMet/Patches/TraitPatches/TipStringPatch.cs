#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	public static class TipStringPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Trait __instance, Pawn ___pawn, ref string __result) {
#pragma warning restore CA1707
			if (__instance == null) {
				throw new ArgumentNullException(nameof(__instance));
			}

#if V1_0 || V1_1
			if (!TraitUtilities.TraitIsDiscovered(___pawn, __instance.def)) {
				__result = "UnknownTrait".Translate().CapitalizeFirst();
			}
#else
			if (!TraitUtilities.TraitIsDiscovered(__instance)) {
				__result = "UnknownTrait".Translate().CapitalizeFirst();
			}
#endif
		}
	}
}
