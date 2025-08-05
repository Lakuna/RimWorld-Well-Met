#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InspectPaneFillerPatches {
	[HarmonyPatch(typeof(InspectPaneFiller), "DrawHunger")]
	internal static class DrawHungerPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, pawn);
	}
}
#endif
