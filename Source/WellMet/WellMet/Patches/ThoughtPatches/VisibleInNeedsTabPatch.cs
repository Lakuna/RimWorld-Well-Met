using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtPatches {
	[HarmonyPatch(typeof(Thought), nameof(Thought.VisibleInNeedsTab), MethodType.Getter)]
	internal static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
		private static void Postfix(Thought __instance, ref bool __result) {
			bool advanced = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, __instance.pawn);
			bool health = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.pawn);
			bool ideoligion = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn);
			bool social = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, __instance.pawn);
			bool gear = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.pawn);

			__result = __result
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				&& ((__instance.def.requiredGenes?.Count ?? 0) == 0 || advanced)
#endif
#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				&& ((__instance.def.requiredHediffs?.Count ?? 0) == 0 || health)
#endif
				&& (__instance.def.requiredTraits?.TrueForAll((traitDef) => KnowledgeUtility.IsTraitKnown(__instance.pawn, traitDef)) ?? true)
				&& (__instance.def.workerClass != typeof(ThoughtWorker_Pain) && __instance.def.workerClass != typeof(ThoughtWorker_Sick) || health)
#if !(V1_0 || V1_1 || V1_2)
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept)) ?? false) || ideoligion)
				&& (!(__instance.def.workerClass?.IsSubclassOf(typeof(ThoughtWorker_Precept_Social)) ?? false) || ideoligion && social)
#endif
				;

#if !(V1_0 || V1_1 || V1_2)
			if (__instance is Thought_Situational_Precept_SlavesInColony
				|| __instance is Thought_Situational_Precept_HighLife
				|| __instance is Thought_IdeoMissingBuilding
				|| __instance is Thought_IdeoDisrespectedBuilding
				|| __instance is Thought_IdeoRoleEmpty) {
				__result = __result && ideoligion;
				return;
			}

			if (__instance is Thought_Situational_WearingDesiredApparel || __instance is Thought_IdeoRoleApparelRequirementNotMet) {
				__result = __result && ideoligion && gear;
				return;
			}
#endif

			if (__instance is Thought_OpinionOfMyLover || __instance is Thought_BondedAnimalMaster || __instance is Thought_NotBondedAnimalMaster || __instance is Thought_SharedBed) {
				__result = __result && social;
				return;
			}

			if (__instance is Thought_DecreeUnmet
#if !(V1_0 || V1_1 || V1_2 || V1_3)
				|| __instance is Thought_Situational_KillThirst || __instance is Thought_Situational_GeneticChemicalDependency
#endif
				) {
				__result = __result && advanced;
				return;
			}

#if !(V1_0 || V1_1 || V1_2)
			if (__instance is Thought_IdeoLeaderResentment) {
				__result = __result && ideoligion && social;
			}
#endif
		}
	}
}
