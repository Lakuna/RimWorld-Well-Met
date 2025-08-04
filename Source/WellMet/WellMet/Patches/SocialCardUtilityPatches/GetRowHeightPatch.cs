#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), "GetRowHeight")]
	internal static class GetRowHeightPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn selPawnForSocialInfo, ref float __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo)) {
				return;
			}

			__result = 0;
		}
	}
}
