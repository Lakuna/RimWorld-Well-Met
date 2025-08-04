﻿#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif
using Lakuna.WellMet.Utility;
using RimWorld;
using Verse;

namespace Lakuna.WellMet.Patches.ITabPawnLogUtilityPatches {
	[HarmonyPatch(typeof(ITab_Pawn_Log_Utility), nameof(ITab_Pawn_Log_Utility.GenerateLogLinesFor))]
	internal static class GenerateLogLinesForPatch {
		[HarmonyPrefix]
		private static void Prefix(Pawn pawn, ref bool showCombat, ref bool showSocial) {
			showCombat = showCombat && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Advanced, pawn);
			showSocial = showSocial && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Social, pawn);
		}
	}
}
