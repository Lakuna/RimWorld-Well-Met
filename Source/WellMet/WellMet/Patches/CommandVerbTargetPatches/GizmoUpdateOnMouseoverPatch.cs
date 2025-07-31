using HarmonyLib;
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.CommandVerbTargetPatches {
	// Redundant since `GizmoPatches.VisiblePatch` already hides weapon gizmos.
	[HarmonyPatch(typeof(Command_VerbTarget), nameof(Command_VerbTarget.GizmoUpdateOnMouseover))]
	internal static class GizmoUpdateOnMouseoverPatch {
		[HarmonyPrefix]
		private static bool Prefix(Command_VerbTarget __instance) => !__instance.verb.CasterIsPawn || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.verb.CasterPawn);
	}
}
