using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;

namespace Lakuna.WellMet.Patches.MechanitorControlGroupGizmoPatches {
	[HarmonyPatch(typeof(MechanitorControlGroupGizmo), nameof(MechanitorControlGroupGizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		private static readonly FieldInfo ControlGroupField = AccessTools.Field(typeof(MechanitorControlGroupGizmo), "controlGroup");

		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(MechanitorControlGroup), "tracker");

		[HarmonyPostfix]
		private static void Postfix(MechanitorControlGroupGizmo __instance, ref bool __result) => __result = __result
			&& (!(ControlGroupField.GetValue(__instance) is MechanitorControlGroup mechanitorControlGroup)
			|| !(TrackerField.GetValue(mechanitorControlGroup) is Pawn_MechanitorTracker pawnMechanitorTracker)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnMechanitorTracker.Pawn));
	}
}
