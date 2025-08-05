#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using HarmonyLib;
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawSkillSummaries))]
	internal static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		private static bool Prefix() => WellMetMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
#endif
