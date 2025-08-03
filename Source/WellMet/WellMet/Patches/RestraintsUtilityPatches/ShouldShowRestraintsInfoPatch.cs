using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.RestraintsUtilityPatches {
	[HarmonyPatch(typeof(RestraintsUtility), nameof(RestraintsUtility.ShouldShowRestraintsInfo))]
	internal static class ShouldShowRestraintsInfoPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn);
	}
}
