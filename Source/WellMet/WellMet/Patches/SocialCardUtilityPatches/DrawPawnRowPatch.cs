using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), "DrawPawnRow")]
	internal static class DrawPawnRowPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn selPawnForSocialInfo) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, selPawnForSocialInfo);
	}
}
