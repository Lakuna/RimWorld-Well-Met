#if !(V1_0 || V1_1)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;

namespace Lakuna.WellMet.Patches.MechanitorControlGroupGizmoPatches {
	[HarmonyPatch(typeof(MechanitorControlGroupGizmo), nameof(MechanitorControlGroupGizmo.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(MechanitorControlGroup), "tracker");

		[HarmonyPostfix]
		private static void Postfix(MechanitorControlGroup ___controlGroup, ref bool __result) => __result = __result
			&& (!(TrackerField.GetValue(___controlGroup) is Pawn_MechanitorTracker pawnMechanitorTracker)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnMechanitorTracker.Pawn, true));
	}
}
#endif
