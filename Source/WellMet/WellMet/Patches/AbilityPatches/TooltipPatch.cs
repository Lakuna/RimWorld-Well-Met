using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.AbilityPatches {
	[HarmonyPatch(typeof(Ability), nameof(Ability.Tooltip), MethodType.Getter)]
	internal static class TooltipPatch {
		[HarmonyPostfix]
		public static void Postfix(Ability __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, __instance.pawn)) {
				return;
			}

			__result = "";
		}
	}
}
