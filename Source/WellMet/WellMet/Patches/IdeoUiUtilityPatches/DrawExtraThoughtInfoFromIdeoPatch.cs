#if !(V1_0 || V1_1)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.IdeoUiUtilityPatches {
	[HarmonyPatch(typeof(IdeoUIUtility), nameof(IdeoUIUtility.DrawExtraThoughtInfoFromIdeo))]
	internal static class DrawExtraThoughtInfoFromIdeoPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn pawn) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn);
	}
}
#endif
