#if !V1_0
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.MechanitorBandwidthGizmoPatches {
	[HarmonyPatch(typeof(MechanitorBandwidthGizmo), nameof(MechanitorBandwidthGizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn_MechanitorTracker ___tracker, ref bool __result) => __result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, ___tracker.Pawn, true);
	}
}
#endif
