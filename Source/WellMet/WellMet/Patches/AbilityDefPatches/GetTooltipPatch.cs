using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.AbilityDefPatches {
	[HarmonyPatch(typeof(AbilityDef), nameof(AbilityDef.GetTooltip))]
	internal static class GetTooltipPatch {
		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, ref string __result) {
			if (pawn != null && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)) {
				return;
			}

			__result = "";
		}
	}
}
