using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), nameof(Thought.VisibleInNeedsTab), MethodType.Getter)]
	internal static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
		private static void Postfix(Thought __instance, ref bool __result) {
			__result = __result
				&& ((__instance.def.requiredGenes?.Count ?? 0) == 0 || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, __instance.pawn))
				&& ((__instance.def.requiredHediffs?.Count ?? 0) == 0 || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.pawn))
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.pawn));

			if (__instance is Thought_Situational_Precept_SlavesInColony
				|| __instance is Thought_Situational_Precept_HighLife
				|| __instance is Thought_IdeoMissingBuilding
				|| __instance is Thought_IdeoDisrespectedBuilding
				|| __instance is Thought_IdeoRoleEmpty) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn);
				return;
			}

			if (__instance is Thought_Situational_WearingDesiredApparel || __instance is Thought_IdeoRoleApparelRequirementNotMet) {
				__result = __result
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn)
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.pawn);
				return;
			}

			if (__instance is Thought_OpinionOfMyLover || __instance is Thought_BondedAnimalMaster || __instance is Thought_NotBondedAnimalMaster || __instance is Thought_SharedBed) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance.pawn);
				return;
			}

			if (__instance is Thought_DecreeUnmet) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, __instance.pawn);
				return;
			}

			if (__instance is Thought_Situational_KillThirst || __instance is Thought_Situational_GeneticChemicalDependency) {
				__result = __result
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, __instance.pawn)
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn);
				return;
			}

			if (__instance is Thought_IdeoLeaderResentment) {
				__result = __result
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn)
					&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance.pawn);
			}
		}
	}
}
