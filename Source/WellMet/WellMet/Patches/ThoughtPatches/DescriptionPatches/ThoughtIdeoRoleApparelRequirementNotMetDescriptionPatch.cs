#if !(V1_0 || V1_1 || V1_2)
using HarmonyLib;
using Lakuna.WellMet.Utilities;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.ThoughtPatches.DescriptionPatches {
	[HarmonyPatch(typeof(Thought_IdeoRoleApparelRequirementNotMet), "get_" + nameof(Thought_IdeoRoleApparelRequirementNotMet.Description))]
	public static class ThoughtIdeoRoleApparelRequirementNotMetDescriptionPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref string __result) {
#pragma warning restore CA1707
			if (!ThoughtUtilities.ThoughtIsDiscovered(__instance)) {
				__result = "UnknownThought".Translate().CapitalizeFirst();
			}
		}
	}
}
#endif
