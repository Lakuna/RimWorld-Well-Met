#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.SocialCardUtilityPatches {
	[HarmonyPatch(typeof(SocialCardUtility), nameof(SocialCardUtility.DrawPawnCertainty))]
	internal static class DrawPawnCertaintyPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);
	}
}
#endif
