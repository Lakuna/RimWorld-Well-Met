using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPanePatches {
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawAreaAllowed")]
	internal static class AreaPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
	}
}
