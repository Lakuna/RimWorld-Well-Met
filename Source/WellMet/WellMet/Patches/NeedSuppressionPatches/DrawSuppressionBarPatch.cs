using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.NeedSuppressionPatches {
	[HarmonyPatch(typeof(Need_Suppression), nameof(Need_Suppression.DrawSuppressionBar))]
	internal static class DrawSuppressionBarPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn ___pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, ___pawn);
	}
}
