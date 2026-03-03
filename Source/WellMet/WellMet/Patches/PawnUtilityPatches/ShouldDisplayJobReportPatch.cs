#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

using Verse;

namespace Lakuna.WellMet.Patches.PawnUtilityPatches {
	[HarmonyPatch(typeof(PawnUtility), nameof(PawnUtility.ShouldDisplayJobReport))]
	internal static class ShouldDisplayJobReportPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn pawn, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn);
	}
}
#endif
