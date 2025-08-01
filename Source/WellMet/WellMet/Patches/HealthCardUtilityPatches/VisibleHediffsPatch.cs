using HarmonyLib;
using Lakuna.WellMet.Utility;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Lakuna.WellMet.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), "VisibleHediffs")]
	internal static class VisibleHediffsPatch {
		[HarmonyPostfix]
		private static void Postfix(Pawn pawn, ref IEnumerable<Hediff> __result) {
			if (KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn)) {
				return;
			}

			__result = Array.Empty<Hediff>();
		}
	}
}
