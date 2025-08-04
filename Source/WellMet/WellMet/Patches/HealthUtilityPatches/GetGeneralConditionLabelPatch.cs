#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.HealthUtilityPatches {
	[HarmonyPatch(typeof(HealthUtility), nameof(HealthUtility.GetGeneralConditionLabel))]
	internal static class GetGeneralConditionLabelPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn)) {
				return;
			}

			__result = "Unknown".Translate().CapitalizeFirst();
		}
	}
}
