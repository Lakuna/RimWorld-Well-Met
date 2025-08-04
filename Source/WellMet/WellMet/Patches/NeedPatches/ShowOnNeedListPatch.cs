﻿using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.NeedPatches {
	[HarmonyPatch(typeof(Need), nameof(Need.ShowOnNeedList), MethodType.Getter)]
	internal static class ShowOnNeedListPatch {
		[HarmonyPostfix]
		private static void Postfix(Need __instance, Pawn ___pawn, ref bool __result) {
			if (__instance is Need_Chemical) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, ___pawn);
				return;
			}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is Need_Deathrest) {
				__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, ___pawn);
			}
#endif
		}
	}
}
