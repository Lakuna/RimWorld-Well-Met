#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.PawnAgeTrackerPatches {
	[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTooltipString), MethodType.Getter)]
	internal static class AgeTooltipStringPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn ___pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, ___pawn)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst()
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				+ ".";
#else
				.EndWithPeriod();
#endif
		}
	}
}
