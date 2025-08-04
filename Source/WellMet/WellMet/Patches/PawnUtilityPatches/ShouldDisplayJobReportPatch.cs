#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldDisplayJobReport))]
	internal static class ShouldDisplayJobReportPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
#endif
