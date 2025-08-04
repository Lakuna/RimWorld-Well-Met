#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
using RimWorld;
#else
using Verse;
#endif

namespace Lakuna.WellMet.Patches.StartingPawnUtilityPatches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DrawSkillSummaries")]
#else
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawSkillSummaries))]
#endif
	internal static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		private static bool Prefix() => WellMetMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
