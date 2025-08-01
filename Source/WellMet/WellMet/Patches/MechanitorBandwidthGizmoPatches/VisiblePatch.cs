using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;

namespace Lakuna.WellMet.Patches.MechanitorBandwidthGizmoPatches {
	[HarmonyPatch(typeof(MechanitorBandwidthGizmo), nameof(MechanitorBandwidthGizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(MechanitorBandwidthGizmo), "tracker");

		[HarmonyPostfix]
		private static void Postfix(MechanitorBandwidthGizmo __instance, ref bool __result) => __result = __result
			&& (!(TrackerField.GetValue(__instance) is Pawn_MechanitorTracker pawnMechanitorTracker)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnMechanitorTracker.Pawn));
	}
}
