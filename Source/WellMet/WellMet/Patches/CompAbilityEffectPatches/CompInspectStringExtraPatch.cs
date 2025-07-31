using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.CompAbilityEffectPatches {
	[HarmonyPatch(typeof(CompAbilityEffect), nameof(CompAbilityEffect.ShouldHideGizmo), MethodType.Getter)]
	internal static class ShouldHideGizmoPatch {
		[HarmonyPostfix]
		public static void Postfix(CompAbilityEffect __instance, ref bool __result) => __result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, __instance.parent.pawn);
	}
}
