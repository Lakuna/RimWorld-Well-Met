#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.SlaveRebellionUtilityPatches {
	[HarmonyPatch(typeof(SlaveRebellionUtility), nameof(SlaveRebellionUtility.GetAnySlaveRebellionExplanation))]
	internal static class GetAnySlaveRebellionExplanationPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn)) {
				return;
			}

			__result = string.Empty;
		}
	}
}
#endif
