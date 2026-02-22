#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.WellMet.Utility;

using RimWorld;

using Verse;

namespace Lakuna.WellMet.Patches.GizmoGrowthTierPatches {
	[HarmonyPatch(typeof(Gizmo_GrowthTier), nameof(Gizmo_GrowthTier.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Pawn ___child, ref bool __result) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, ___child, ControlCategory.Control);
	}
}
#endif
