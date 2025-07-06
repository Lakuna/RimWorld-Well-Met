using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPane {
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawHunger")]
	internal static class HungerPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn);
	}
}
