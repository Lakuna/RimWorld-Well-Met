using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetTitleTipString")]
	internal static class GetTitleTipStringPatch {
		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, ref string __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn)) {
				return;
			}

			__result = "";
		}
	}
}
