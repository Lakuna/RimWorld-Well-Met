#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
#if !V1_0
using System;
#endif
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

#if V1_0
			__result = new Hediff[] { };
#else
			__result = Array.Empty<Hediff>();
#endif
		}
	}
}
