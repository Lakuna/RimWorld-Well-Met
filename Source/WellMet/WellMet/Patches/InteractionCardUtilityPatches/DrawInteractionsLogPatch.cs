#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.InteractionCardUtilityPatches {
	[HarmonyPatch(typeof(InteractionCardUtility), nameof(InteractionCardUtility.DrawInteractionsLog))]
	internal static class DrawInteractionsLogPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
	}
}
