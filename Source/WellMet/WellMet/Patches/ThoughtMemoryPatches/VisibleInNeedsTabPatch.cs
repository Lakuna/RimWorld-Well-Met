using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;

namespace Lakuna.WellMet.Patches.ThoughtMemoryPatches {
	[HarmonyPatch(typeof(Thought_Memory), nameof(Thought_Memory.VisibleInNeedsTab), MethodType.Getter)]
	internal static class VisibleInNeedsTabPatch {
		[HarmonyPostfix]
		private static void Postfix(Thought_Memory __instance, ref bool __result) {
			if (__instance is Thought_RelicAtRitual
				|| __instance is Thought_TameVeneratedAnimalDied
				|| __instance is Thought_Counselled
				|| __instance is Thought_AttendedRitual
				|| __instance is Thought_IdeoRoleLost
				|| __instance is Thought_KilledInnocentAnimal) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, __instance.pawn);
				return;
			}

			if (__instance is Thought_WeaponTrait) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, __instance.pawn);
				return;
			}

			if (__instance is Thought_MemoryRoyalTitle) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, __instance.pawn);
			}
		}
	}
}
