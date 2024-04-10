#if !(V1_0 || V1_1)
using HarmonyLib;
using Lakuna.WellMet.Utilities;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.Label))]
	public static class TraitLabelPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore CA1707
			if (!TraitUtilities.TraitIsDiscovered(__instance)) {
				__result = "UnknownTrait".Translate().CapitalizeFirst();
			}
		}
	}
}
#endif
