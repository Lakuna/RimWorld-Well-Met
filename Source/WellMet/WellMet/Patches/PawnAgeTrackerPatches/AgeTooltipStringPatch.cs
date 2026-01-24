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

			__result = MiscellaneousUtility.EndWithPeriod("BR.Unknown".Translate().CapitalizeFirst());
		}
	}
}
