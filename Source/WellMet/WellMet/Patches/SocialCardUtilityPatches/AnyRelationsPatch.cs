using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.AnyRelations))]
	internal static class AnyRelationsPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn selPawnForSocialInfo, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo);
	}
}
