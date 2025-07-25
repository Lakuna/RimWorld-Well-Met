using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.AbilityPatches {
	[HarmonyPatch(typeof(Ability), nameof(Ability.GizmoExtraLabel), MethodType.Getter)]
	internal static class GizmoExtraLabelPatch {
		[HarmonyPostfix]
		public static void Postfix(Ability __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, __instance.pawn)) {
				return;
			}

			__result = "";
		}
	}
}
