using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.AbilityCompPatches {
	[HarmonyPatch(typeof(AbilityComp), nameof(AbilityComp.CompInspectStringExtra))]
	internal static class CompInspectStringExtraPatch {
		[HarmonyPostfix]
		public static void Postfix(AbilityComp __instance, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, __instance.parent.pawn)) {
				return;
			}

			__result = null;
		}
	}
}
