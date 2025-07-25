using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.BackstoryDefPatches {
	[HarmonyPatch(typeof(BackstoryDef), nameof(BackstoryDef.FullDescriptionFor))]
	internal static class FullDescriptionForPatch {
		[HarmonyPostfix]
		public static void Postfix(Pawn p, ref TaggedString __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, p)) {
				return;
			}

			__result = "";
		}
	}
}
