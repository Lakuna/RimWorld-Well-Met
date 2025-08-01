using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	internal static class TipStringPatch {
		[HarmonyPostfix]
		private static void Postfix(Trait __instance, ref string __result) {
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst().EndWithPeriod();
		}
	}
}
