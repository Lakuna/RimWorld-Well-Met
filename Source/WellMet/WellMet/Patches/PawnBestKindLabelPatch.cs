#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches {
	[HarmonyPatch(typeof(GenLabel), nameof(GenLabel.BestKindLabel))]
	public static class PawnBestKindLabelPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst();
		}
	}
}
