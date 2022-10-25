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
using Verse;

namespace Lakuna.WellMet.Patches.ThoughtPatches.LabelCapPatches {
	[HarmonyPatch(typeof(Thought), "get_" + nameof(Thought.LabelCap))]
	[HarmonyPatch(typeof(Thought_Memory), "get_" + nameof(Thought_Memory.LabelCap))]
	[HarmonyPatch(typeof(Thought_OpinionOfMyLover), "get_" + nameof(Thought_OpinionOfMyLover.LabelCap))]
	[HarmonyPatch(typeof(Thought_Situational), "get_" + nameof(Thought_Situational.LabelCap))]
	[HarmonyPatch(typeof(Thought_SituationalSocial), "get_" + nameof(Thought_SituationalSocial.LabelCap))]
#if !V1_0
	[HarmonyPatch(typeof(Thought_DecreeUnmet), "get_" + nameof(Thought_DecreeUnmet.LabelCap))]
	[HarmonyPatch(typeof(Thought_MemoryRoyalTitle), "get_" + nameof(Thought_MemoryRoyalTitle.LabelCap))]
	[HarmonyPatch(typeof(Thought_PsychicHarmonizer), "get_" + nameof(Thought_PsychicHarmonizer.LabelCap))]
#if !V1_1
	[HarmonyPatch(typeof(Thought_WeaponTrait), "get_" + nameof(Thought_WeaponTrait.LabelCap))]
#if !V1_2
	[HarmonyPatch(typeof(Thought_AttendedRitual), "get_" + nameof(Thought_AttendedRitual.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoDisrespectedBuilding), "get_" + nameof(Thought_IdeoDisrespectedBuilding.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoLeaderResentment), "get_" + nameof(Thought_IdeoLeaderResentment.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoMissingBuilding), "get_" + nameof(Thought_IdeoMissingBuilding.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoRoleApparelRequirementNotMet), "get_" + nameof(Thought_IdeoRoleApparelRequirementNotMet.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoRoleEmpty), "get_" + nameof(Thought_IdeoRoleEmpty.LabelCap))]
	[HarmonyPatch(typeof(Thought_IdeoRoleLost), "get_" + nameof(Thought_IdeoRoleLost.LabelCap))]
	[HarmonyPatch(typeof(Thought_KilledInnocentAnimal), "get_" + nameof(Thought_KilledInnocentAnimal.LabelCap))]
	[HarmonyPatch(typeof(Thought_MemoryObservationTerror), "get_" + nameof(Thought_MemoryObservationTerror.LabelCap))]
	[HarmonyPatch(typeof(Thought_RelicAtRitual), "get_" + nameof(Thought_RelicAtRitual.LabelCap))]
	[HarmonyPatch(typeof(Thought_Situational_WearingDesiredApparel), "get_" + nameof(Thought_Situational_WearingDesiredApparel.LabelCap))]
	[HarmonyPatch(typeof(Thought_TameVeneratedAnimalDied), "get_" + nameof(Thought_TameVeneratedAnimalDied.LabelCap))]
#endif
#endif
#endif
	public static class ThoughtLabelCapPatch {
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
