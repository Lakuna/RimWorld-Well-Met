#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.SkillUiPatches {
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill))]
	internal static class DrawSkillPatch {
		[HarmonyPrefix]
		private static bool Prefix(SkillRecord skill) => KnowledgeUtility.IsSkillKnown(skill);
	}
}
