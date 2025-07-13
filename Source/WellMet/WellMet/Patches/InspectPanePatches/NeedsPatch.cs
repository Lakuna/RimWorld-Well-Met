using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPanePatches {
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawMechEnergy")]
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawHunger")]
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawMood")]
	internal static class NeedsPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn);
	}
}
