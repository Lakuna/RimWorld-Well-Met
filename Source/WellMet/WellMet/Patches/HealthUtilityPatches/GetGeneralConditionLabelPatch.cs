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
#pragma warning disable CA1707
		private static void Postfix(Pawn pawn, ref string __result) {
#pragma warning restore CA1707
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst();
		}
	}
}
