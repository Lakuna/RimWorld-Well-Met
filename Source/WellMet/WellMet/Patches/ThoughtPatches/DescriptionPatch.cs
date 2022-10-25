#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lakuna.WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.Description))]
#if !V1_0
	[HarmonyPatch(typeof(Thought_DecreeUnmet), "get_" + nameof(Thought_DecreeUnmet.Description))]
	[HarmonyPatch(typeof(Thought_MemoryRoyalTitle), "get_" + nameof(Thought_MemoryRoyalTitle.Description))]
#if !(V1_1 || V1_2)
	[HarmonyPatch(typeof(Thought_AttendedRitual), "get_" + nameof(Thought_AttendedRitual.Description))]
	[HarmonyPatch(typeof(Thought_IdeoDisrespectedBuilding), "get_" + nameof(Thought_IdeoDisrespectedBuilding.Description))]
	[HarmonyPatch(typeof(Thought_IdeoLeaderResentment), "get_" + nameof(Thought_IdeoLeaderResentment.Description))]
	[HarmonyPatch(typeof(Thought_IdeoMissingBuilding), "get_" + nameof(Thought_IdeoMissingBuilding.Description))]
	[HarmonyPatch(typeof(Thought_IdeoRoleApparelRequirementNotMet), "get_" + nameof(Thought_IdeoRoleApparelRequirementNotMet.Description))]
	[HarmonyPatch(typeof(Thought_IdeoRoleEmpty), "get_" + nameof(Thought_IdeoRoleEmpty.Description))]
	[HarmonyPatch(typeof(Thought_IdeoRoleLost), "get_" + nameof(Thought_IdeoRoleLost.Description))]
	[HarmonyPatch(typeof(Thought_RelicAtRitual), "get_" + nameof(Thought_RelicAtRitual.Description))]
	[HarmonyPatch(typeof(Thought_Situational_WearingDesiredApparel), "get_" + nameof(Thought_Situational_WearingDesiredApparel.Description))]
	[HarmonyPatch(typeof(Thought_TameVeneratedAnimalDied), "get_" + nameof(Thought_TameVeneratedAnimalDied.Description))]
#endif
#endif
	public static class DescriptionPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707 // Underscores are required for special Harmony parameters.
		public static void Postfix(Thought __instance, ref string __result) {
#pragma warning restore CA1707
			if (ThoughtUtilities.ThoughtIsHidden(__instance)) {
				__result = ThoughtUtilities.UnknownThoughtDescription;
			}
		}
	}
}
