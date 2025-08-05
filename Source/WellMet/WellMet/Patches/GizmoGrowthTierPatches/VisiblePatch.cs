#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.GizmoGrowthTierPatches {
	[HarmonyPatch(typeof(Gizmo_GrowthTier), nameof(Gizmo_GrowthTier.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn ___child, ref bool __result) => __result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, ___child, true);
	}
}
#endif
