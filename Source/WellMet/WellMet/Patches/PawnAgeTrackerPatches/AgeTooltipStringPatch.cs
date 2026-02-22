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
#pragma warning disable CA1707
		private static void Postfix(Pawn ___pawn, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, ___pawn)) {
				return;
			}

			__result = MiscellaneousUtility.EndWithPeriod("BR.Unknown".Translate().CapitalizeFirst());
		}
	}
}
