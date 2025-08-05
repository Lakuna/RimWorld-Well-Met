#if !(V1_0 || V1_1)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldDisplayLordReport))]
	internal static class ShouldDisplayLordReportPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
#endif
