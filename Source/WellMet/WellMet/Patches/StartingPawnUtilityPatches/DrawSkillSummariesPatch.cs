﻿using HarmonyLib;
using Lakuna.WellMet.Utility;
using Verse;

namespace Lakuna.WellMet.Patches.StartingPawnUtilityPatches {
	[HarmonyPatch(typeof(StartingPawnUtility), nameof(StartingPawnUtility.DrawSkillSummaries))]
	internal static class DrawSkillSummariesPatch {
		[HarmonyPrefix]
		private static bool Prefix() => WellMetMod.Settings.AlwaysKnowStartingColonists || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, PawnType.Colonist);
	}
}
