#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.FactionDesc))]
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	public static class PawnBasicPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Pawn __instance, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance)) { return; }
			__result = "Unknown".Translate().CapitalizeFirst();
		}
	}
}
