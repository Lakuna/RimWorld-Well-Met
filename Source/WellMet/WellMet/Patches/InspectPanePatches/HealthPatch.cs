using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPanePatches {
	[HarmonyPatch(typeof(InspectPaneFiller), nameof(InspectPaneFiller.DrawHealth))]
	internal static class HealthPatch {
		[HarmonyPrefix]
		private static bool Prefix(Thing t) => !(t is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);
	}
}
