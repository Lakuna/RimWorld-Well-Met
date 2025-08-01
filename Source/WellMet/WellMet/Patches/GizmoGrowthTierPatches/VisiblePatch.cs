using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System.Reflection;
using Verse;

namespace Lakuna.WellMet.Patches.GizmoGrowthTierPatches {
	[HarmonyPatch(typeof(Gizmo_GrowthTier), nameof(Gizmo_GrowthTier.Visible), MethodType.Getter)]
	internal static class VisiblePatch {
		private static readonly FieldInfo ChildField = AccessTools.Field(typeof(Gizmo_GrowthTier), "child");

		[HarmonyPostfix]
		private static void Postfix(Gizmo_GrowthTier __instance, ref bool __result) => __result = __result
			&& (!(ChildField.GetValue(__instance) is Pawn pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn));
	}
}
