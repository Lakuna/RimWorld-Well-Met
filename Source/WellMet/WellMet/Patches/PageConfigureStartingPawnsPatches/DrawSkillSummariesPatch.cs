#if V1_0
using Harmony;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.PageConfigureStartingPawnsPatches {
	[HarmonyPatch(typeof(Page_ConfigureStartingPawns), "DrawSkillSummaries")]
	internal static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		private static bool Prefix() => WellMetMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
#endif
