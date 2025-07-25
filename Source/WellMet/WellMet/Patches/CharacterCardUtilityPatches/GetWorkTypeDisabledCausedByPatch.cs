using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetWorkTypeDisabledCausedBy")]
	internal static class GetWorkTypeDisabledCausedByPatch {
		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn)) {
				return;
			}

			__result = "";
		}
	}
}
