#if V1_0
using Harmony;
#else
using HarmonyLib;

#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.Label))]
	[HarmonyPatch(typeof(Trait), "get_" + nameof(Trait.LabelCap))]
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	[HarmonyPatch(typeof(Trait), nameof(Trait.ToString))]
	public static class TraitLabelPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsTraitKnown(__instance)) { return; }
			__result = "UnknownTrait".Translate().CapitalizeFirst();
		}
	}
}
