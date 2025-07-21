using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.CharacterCardPatches {
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkillsOf))]
	internal static class DrawSkillsOfPatch {
		[HarmonyPrefix]
		private static bool Prefix(Pawn p) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, p);
	}
}
