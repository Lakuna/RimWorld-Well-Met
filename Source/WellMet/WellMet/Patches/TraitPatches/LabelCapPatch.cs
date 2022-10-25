#if !V1_0
using HarmonyLib;
using System.Threading.Tasks;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
#if !(V1_0 || V1_1)
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.LabelCap))]
	public static class LabelCapPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore CA1707
			if (!TraitUtilities.TraitIsDiscovered(__instance)) {
				__result = "UnknownTrait".Translate().CapitalizeFirst();
			}
		}
	}
#endif
}
